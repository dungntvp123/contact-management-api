
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN_ProjectAPI.Configs;
using PRN_ProjectAPI.DTOs;
using PRN_ProjectAPI.Models;
using System.Reflection.Emit;
using System.Security.Claims;


namespace PRN_ProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly PhoneBookDbContext _context;
        private readonly SystemConfig _config;

        public ContactController(PhoneBookDbContext context, SystemConfig config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetList()
        {
            try
            {
                string phone = null;
                var currentUser = HttpContext.User;
                if (currentUser.HasClaim(c => c.Type == ClaimTypes.MobilePhone))
                {
                    phone = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone).Value;
                }
                if (phone == null) { 
                    return Unauthorized();
                }
                var result = await _context.Contacts
                    .Include(c => c.Creator)
                    .Where(c => c.Creator.PhoneNumber1.Replace("-", "").Equals(phone.Replace("-", "")))
                    .Select(c => new
                    {
                        ContactId = c.Id,
                        ContactName = c.ContactName,
                        Email = c.Email,
                        Phone = c.ContactNumber,
                        Label = c.Label,
                        Image = (c.Image != null ? _config.ImageUrl + c.Image : null),
                    })

                    .ToListAsync();
                return Ok(result);
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                Contact contact = await _context.Contacts.SingleOrDefaultAsync(c => c.Id == id);
                if (contact == null)
                {
                    return NotFound();
                }

                return Ok(new
                {
                    ContactId = contact.Id,
                    ContactName = contact.ContactName,
                    ContactNumber = contact.ContactNumber,
                    Email = contact.Email,
                    Note = contact.Note,
                    Label = contact.Label,
                    Image = (contact.Image != null ? _config.ImageUrl + contact.Image : null),
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Add([FromForm] SaveContactRequestDto requestDto)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    
					string phone = null;
					var currentUser = HttpContext.User;
					if (currentUser.HasClaim(c => c.Type == ClaimTypes.MobilePhone))
					{
						phone = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone).Value;
					}
                    if (phone == null)
                    {
                        return Unauthorized();
                    }
                    var creator = _context.PhoneNumbers.SingleOrDefaultAsync(c => c.PhoneNumber1.Replace("-", "").Equals(phone.Replace("-", "")));
                    if (creator == null)
                    {
                        return NotFound();
                    }
                    int creatorId = creator.Id;
					if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    Contact contact = requestDto.CovertAsContact();
                    contact.CreatorId = creatorId;
                    var file = requestDto.File;
                    if (file == null || file.Length == 0)
                    {
                        contact.Image = null;
                    }
                    else
                    {
                        string fileExtension = file.FileName.Split('.')[1];
                        string filename = DateTime.Now.Ticks.ToString() + "." + fileExtension;
                        var filepath = Path.Combine("E:\\Self-Project\\Upload_File\\user\\image", filename);
                        using (var stream = new FileStream(filepath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        contact.Image = filename;
                    }


                    _context.Contacts.Add(contact);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return CreatedAtAction(nameof(GetById), new { id = contact.Id }, new
                    {
                        ContactName = contact.ContactName,
                        ContactNumber = contact.ContactNumber,
                        Email = contact.Email,
                        Note = contact.Note,
                        Label = contact.Label,
                        Image = (contact.Image != null ? _config.ImageUrl + contact.Image : null),
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, ex.Message);
                }
            }
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromForm] SaveContactRequestDto requestDto)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    Contact contact = await _context.Contacts.SingleOrDefaultAsync(c => c.Id == id);
                    if (contact == null)
                    {
                        return NotFound();
                    }
                    contact = requestDto.CovertAsContact(contact);
                    var file = requestDto.File;
                    if (file != null && file.Length != 0)
                    {
                        if (contact.Image != null)
                        {
                            if (System.IO.File.Exists($"{_config.UploadFilePath}\\{contact.Image}"))
                            {
                                // Delete the file
                                System.IO.File.Delete($"{_config.UploadFilePath}\\{contact.Image}");
                            }
                            else
                            {
                                await transaction.RollbackAsync();
                                return StatusCode(500, "Server data sync error");
                            }
                        }
                        string fileExtension = file.FileName.Split('.')[1];
                        string filename = DateTime.Now.Ticks.ToString() + "." + fileExtension;
                        var filepath = Path.Combine(_config.UploadFilePath, filename);
                        using (var stream = new FileStream(filepath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        contact.Image = filename;
                    }

                    _context.Contacts.Update(contact);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok(new
                    {
                        ContactName = contact.ContactName,
                        ContactNumber = contact.ContactNumber,
                        Email = contact.Email,
                        Note = contact.Note,
                        Label = contact.Label,
                        Image = (contact.Image != null ? _config.ImageUrl + contact.Image : null),
                    });
                } catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, ex.Message);
                }
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            using(var transaction = await _context.Database.BeginTransactionAsync())
            {
				try
                {
					var contact = await _context.Contacts.SingleOrDefaultAsync(x => x.Id == id);
                    if (contact == null)
                    {
                        return NotFound();
                    }

                    _context.Contacts.Remove(contact);
                    await transaction.CommitAsync();
                    await _context.SaveChangesAsync(); 
                    return NoContent();
				} catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, ex.Message);
                }


			}
        }

    }
}

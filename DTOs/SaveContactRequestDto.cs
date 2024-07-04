using PRN_ProjectAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace PRN_ProjectAPI.DTOs
{
    public class SaveContactRequestDto
    {
        [Required]
        [RegularExpression("^([0-9]{10}|([0-9]{3}-[0-9]{3}-[0-9]{4}))$")]
        public string ContactNumber { get; set; } = null!;
        
        public string? ContactName { get; set; }

        public string? Note { get; set; }

        [Required]
        [RegularExpression("^(mobi|comp|home)$")]
        public string? Label { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public IFormFile? File { get; set; }

        public Contact CovertAsContact()
        {
            return new Contact
            {
                ContactNumber = (ContactNumber.Length == 10 ? ContactNumber.Substring(0, 3) + "-" + ContactNumber.Substring(3, 3) + "-" + ContactNumber.Substring(6) : ContactNumber),
                ContactName = ContactName,
                Email = Email,
                Label = Label,
                Note = Note,
            };
        }

        public Contact CovertAsContact(Contact contact)
        {
            contact.ContactNumber = (ContactNumber.Length == 10 ? ContactNumber.Substring(0, 3) + "-" + ContactNumber.Substring(3, 3) + "-" + ContactNumber.Substring(6) : ContactNumber);
            contact.ContactName = (ContactName == null ? contact.ContactName : ContactName);
            contact.Email = (Email == null ? contact.Email : Email);
            contact.Label = (Label == null ? contact.Label : Label);
            contact.Note = (Note == null ? contact.Note : Note);
            return contact;
        }
    }
}

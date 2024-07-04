using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PRN_ProjectAPI.Models;

public partial class PhoneNumber
{
    public int Id { get; set; }

    public string PhoneNumber1 { get; set; } = null!;
    [JsonIgnore]
    public string? Password { get; set; }
    [JsonIgnore]
    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}

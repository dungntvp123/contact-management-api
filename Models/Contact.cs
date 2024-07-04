using System;
using System.Collections.Generic;

namespace PRN_ProjectAPI.Models;

public partial class Contact
{
    public int Id { get; set; }

    public int CreatorId { get; set; }

    public string ContactNumber { get; set; } = null!;

    public string? ContactName { get; set; }

    public string? Note { get; set; }

    public string? Label { get; set; }

    public string? Email { get; set; }

    public string? Image { get; set; }

    public virtual PhoneNumber Creator { get; set; } = null!;
}

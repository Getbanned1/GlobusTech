using System;
using System.Collections.Generic;

namespace GlobusTech;

public partial class User
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public string FullName { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int CompanyId { get; set; }

    public string Email { get; set; } = null!;

    public virtual ICollection<ApplicationModel> Applications { get; set; } = new List<ApplicationModel>();

    public virtual Company Company { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}

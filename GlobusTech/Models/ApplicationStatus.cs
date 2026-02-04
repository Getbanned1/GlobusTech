using System;
using System.Collections.Generic;

namespace GlobusTech;

public partial class ApplicationStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ApplicationModel> Applications { get; set; } = new List<ApplicationModel>();
}

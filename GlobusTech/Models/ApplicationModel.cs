using System;
using System.Collections.Generic;

namespace GlobusTech;

public partial class ApplicationModel
{
    public int Id { get; set; }

    public int ServiceId { get; set; }

    public int UserId { get; set; }

    public DateTime Date { get; set; }

    public int StatusId { get; set; }

    public int SpecialistAmount { get; set; }

    public int TotalCost { get; set; }

    public string? Comment { get; set; }

    public virtual Service Service { get; set; } = null!;

    public virtual ApplicationStatus Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

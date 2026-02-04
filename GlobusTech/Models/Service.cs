using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace GlobusTech;

public partial class Service
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int ServiceSphereId { get; set; }

    public int Duration { get; set; }

    public DateTime StartDate { get; set; }

    public int Price { get; set; }

    public int TeamTypeId { get; set; }

    public int AvalibleSlots { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath => string.IsNullOrEmpty(FileName)? "Images/default_service_image.jpg" : $"Images/{FileName}";

    public bool Lowslots => AvalibleSlots < TeamType.Capacity * 0.1; 

    public bool Offer =>  AvalibleSlots > TeamType.Capacity * 0.85;

    public string BackgroundBrush => Lowslots ? "#FFB6C1" : Offer ? "#FFD700" : "#E0E0E0";

    public virtual ICollection<ApplicationModel> Applications { get; set; } = new List<ApplicationModel>();

    public virtual ServiceSphere ServiceSphere { get; set; } = null!;

    public virtual TeamType TeamType { get; set; } = null!;
}

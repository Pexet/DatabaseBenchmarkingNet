using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseBenchmarking.DBConnection.Entities;

[Table("events", Schema = "dbo")]
public class Event : BaseEntity
{
    [Column("title", TypeName = "varchar(255)")]
    public string Title { get; set; } = null!;

    [Column("description", TypeName = "varchar(4096)")]
    public string? Description { get; set; }

    [Column("place", TypeName = "varchar(4096)")]
    public string? Place { get; set; }

    [Column("starttime")]
    public DateTime StartTime { get; set; }

    [Column("endtime")]
    public DateTime EndTime { get; set; }

    public EventStyle Style { get; set; } = null!;

    [Column("styleid")]
    public int StyleId { get; set; }

    [ForeignKey("scheduleid")]
    public int ScheduleId { get; set; }
}

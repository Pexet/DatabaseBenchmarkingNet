using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseBenchmarking.DBConnection.Entities;

[Table("eventstyles", Schema = "dbo")]
public class EventStyle : BaseEntity
{
    [Column("title", TypeName = "varchar(255)")]
    public string Title { get; set; } = null!;

    [Column("foregroundcolor", TypeName = "varchar(6)")]
    public string ForegoundColor { get; set; } = null!;

    [Column("backgroundcolor", TypeName = "varchar(6)")]
    public string BackgroundColor { get; set; } = null!;

    [Column("studentId")]
    [ForeignKey("student")]
    public int StudentId { get; set; }
}

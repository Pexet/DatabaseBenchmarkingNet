using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseBenchmarking.DBConnection.Entities;

[Table("links", Schema = "dbo")]
public class Link : BaseEntity
{
    [Column("address", TypeName = "varchar(1024)")]
    public string Address { get; set; } = null!;

    [Column("eventid")]
    [ForeignKey("event")]
    public int EventId { get; set; }
}

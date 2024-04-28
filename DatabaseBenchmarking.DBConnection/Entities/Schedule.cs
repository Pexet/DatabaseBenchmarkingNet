using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseBenchmarking.DBConnection.Entities;

[Table("schedules", Schema = "dbo")]
public class Schedule : BaseEntity
{
    [Column("title", TypeName = "varchar(255)")]
    public string Title { get; set; } = null!;

    [Column("studentId")]
    [ForeignKey("student")]
    public int StudentId { get; set; }
}

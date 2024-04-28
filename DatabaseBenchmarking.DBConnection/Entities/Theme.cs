using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseBenchmarking.DBConnection.Entities;

[Table("themes", Schema = "dbo")]
public class Theme : BaseEntity
{
    [Required]
    [Column("title", TypeName = "varchar(255)")]
    public string Title { get; set; } = null!;

    [Column("foregoundcolor", TypeName = "varchar(6)")]
    public string ForegoundColor { get; set; } = null!;

    [Column("backgroundcolor", TypeName = "varchar(6)")]
    public string BackgroundColor { get; set; } = null!;

    [Column("fontname", TypeName = "varchar(255)")]
    public string FontName { get; set; } = null!;

    [Column("fontsize")]
    public int FontSize { get; set; }

    [Column("studentid")]
    public int StudentId { get; set; }

    [ForeignKey("StudentId")]
    public StudentInserted? Student { get; set; }
}
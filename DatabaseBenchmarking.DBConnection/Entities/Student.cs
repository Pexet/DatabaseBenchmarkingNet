using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseBenchmarking.DBConnection.Entities;

[Table("students", Schema = "dbo")]
public class Student : BaseEntity
{
    [Column("isnotifiable")]
    public bool IsNotifiable { get; set; }

    [Column("emailaddress", TypeName = "varchar(255)")]
    public string EmailAddress { get; set; } = null!;

    [Column("password", TypeName = "varchar(255)")]
    public string Password { get; set; } = null!;

    [Column("phonenumber", TypeName = "varchar(10)")]
    public string PhoneNumber { get; set; } = null!;
}

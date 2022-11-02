using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Xarajat.Data.Enttities;


// First Way how to write table configuration. 

[Table("user")]
public class User
{
    [Key]
    public int Id { get; set; }
    [Required]
    public long ChatId { get; set; }
    public int Step { get; set; }
    [Column(TypeName="nvarchar(35)")]
    [Required]
    public string? Name { get; set; }
    [Column(TypeName = "nvarchar(35)")]
    public string? UserName { get; set; }
    [NotMapped]
    public string? FullName => UserName ?? Name;
    [Column(TypeName="nvarchar(15)")]
    public string? Phone { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;

    public int? RoomId { get; set; }
    [ForeignKey(nameof(RoomId))]
    public virtual Room? Room { get; set; }
    public bool IsAdmin { get; set; }

    public virtual List<Outlay>? Outlays { get; set; }
}


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

[Table("Products")]
public class Product
{
    // Primary key indentity column
    [Key]
    public int Id { get; set; }
    //Nvarchar(255) not null
    [Required]
    [StringLength(255)]
    [Column(TypeName = "nvarchar(255)")]
    public string Name { get; set; }
    [Column(TypeName = "nvarchar(255)")]
    [AllowNull]
    public string? Alias { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    //Cho phép null, nếu null thì sẽ lưu vào database là null
    [AllowNull]
    [Column(TypeName = "nvarchar(255)")]
    public string? Description { get; set; }
    [Column(TypeName = "nvarchar(255)")]
    public string ImageUrl { get; set; }
    [DefaultValue(false)]
    public bool Deleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

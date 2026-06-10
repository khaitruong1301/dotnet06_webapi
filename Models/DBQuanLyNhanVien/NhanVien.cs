using System;
using System.Collections.Generic;

namespace backend_netcore_dotnet06.Models.DBQuanLyNhanVien;

public partial class NhanVien
{
    public int Id { get; set; }

    public string TenNv { get; set; } = null!;

    public DateOnly? NgaySinh { get; set; }

    public string? DiaChi { get; set; }

    public string? SoDienThoai { get; set; }

    public int? MaPb { get; set; }

    public DateTime? NgayNhamChuc { get; set; }

    public int? MaTruongPhong { get; set; }

    public virtual ICollection<NhanVien> InverseMaTruongPhongNavigation { get; set; } = new List<NhanVien>();

    public virtual PhongBan? MaPbNavigation { get; set; }

    public virtual NhanVien? MaTruongPhongNavigation { get; set; }

    public virtual ICollection<NhamChuc> NhamChucs { get; set; } = new List<NhamChuc>();

    public virtual ICollection<NhanVienDuan> NhanVienDuans { get; set; } = new List<NhanVienDuan>();
}

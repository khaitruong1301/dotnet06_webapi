using System;
using System.Collections.Generic;

namespace backend_netcore_dotnet06.Models.DBQuanLyNhanVien;

public partial class ViewDanhSachDuAnCuaNhanVien
{
    public int Id { get; set; }

    public string TenNv { get; set; } = null!;

    public string? SoDienThoai { get; set; }

    public string? DanhSachDuAn { get; set; }
}

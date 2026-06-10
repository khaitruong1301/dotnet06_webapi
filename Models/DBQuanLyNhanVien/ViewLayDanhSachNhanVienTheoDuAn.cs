using System;
using System.Collections.Generic;

namespace backend_netcore_dotnet06.Models.DBQuanLyNhanVien;

public partial class ViewLayDanhSachNhanVienTheoDuAn
{
    public int Id { get; set; }

    public string TenDuAn { get; set; } = null!;

    public string? NhanVienThamGia { get; set; }
}

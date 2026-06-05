using System;
using System.Collections.Generic;

namespace backend_netcore_dotnet06.Models.DBQuanLyNhanVien;

public partial class NhamChuc
{
    public int Id { get; set; }

    public string TenChucVu { get; set; } = null!;

    public int MaNv { get; set; }

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}

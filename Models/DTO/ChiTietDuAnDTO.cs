using System.Text.Json;
public class ChiTietDuAnDTO
    {
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; }
        public int SoNhanVien { get; set; }
        public string? DanhSachNhanVien { get; set; }
        public List<TTNhanVienDuAnDTO>? DanhSachNhanVienChiTiet { get; set; } =  new List<TTNhanVienDuAnDTO>();

        public void ConvertJsonNhanVienDuAn (){
            if(!string.IsNullOrEmpty(DanhSachNhanVien))
            {
                DanhSachNhanVienChiTiet = JsonSerializer.Deserialize<List<TTNhanVienDuAnDTO>>(DanhSachNhanVien);
            }
        }
    }
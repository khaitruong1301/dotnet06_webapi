//api-controller-async
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using backend_netcore_dotnet06.Models.DBQuanLyNhanVien;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
//using backend_netcore_dotnet06.Models;

namespace backend_netcore_dotnet06.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuanLyNhanVienController : ControllerBase
    {
        private readonly QuanLyNhanVienContext _context;

        public QuanLyNhanVienController(QuanLyNhanVienContext context)
        {
            _context = context;
        }

        [HttpGet("LayDanhSachNhanVienTheoDuan")]
        public async Task<IActionResult> LayDanhSachNhanVienTheoDuan([FromQuery] int maDuAn)
        {
            
            var lstNhanVien = await _context.ViewLayDanhSachNhanVienTheoDuAns.Where(item => item.Id == maDuAn).ToListAsync();


            return Ok(lstNhanVien);
        }



        [HttpGet("LayDanhSachDuAnTheoMaNhanVien")]
        public async Task<IActionResult> LayDanhSachDuAnTheoMaNhanVien ([FromQuery] int MaNhanVien){

            var lstRes = await _context.ViewDanhSachDuAnCuaNhanViens.Where(n => n.Id == MaNhanVien).ToListAsync();
            if (lstRes.Count() == 0)
            {
                return NotFound("Mã nhân viên không tồn tại!");
            }

            
            
            return Ok(lstRes);

        }


        [HttpGet("LayThongTinNhanVienPhongBan")]
        public async Task<ActionResult> LayThongTinNhanVienPhongBan()
        {

            //Cách 1: Kết trên thuộc tính khoá ngoại bằng linq Include
            // var lstNhanVienPB = await _context.NhanViens.Include(item => item.MaPbNavigation).Select(item => new
            // {
            //     MaNv = item.Id,
            //     TenNv = item.TenNv,
            //     TenPb = item.MaPbNavigation.TenPb
            // }).ToListAsync();
            //Cách 2: Dùng phép join (dữ liệu bảng B không nhất thiết là dữ liệu từ db chỉ cần Collection List)
            var lstNhanVienPB = await _context.NhanViens.Join(_context.PhongBans,
            nv => nv.MaPb,
            pb => pb.Id,
            (nv, pb) => new NhanVienPhongBanDTO
            {
                MaNV = nv.Id,
                TenNV = nv.TenNv,
                TenPB = pb.TenPb
            }
            ).ToListAsync();

            //Cách 3: Dùng câu lệnh SQL thuần 
            var lstNhanVienPBSQLRaw = await _context.Database.SqlQueryRaw<NhanVienPhongBanDTO>("SELECT NhanVien.Id AS MaNV, NhanVien.TenNV, PhongBan.TenPB FROM NhanVien JOIN PhongBan ON NhanVien.MaPb = PhongBan.Id").ToListAsync();
            return Ok(lstNhanVienPBSQLRaw);
        }

        [HttpGet("LayThongTinNhanVienDuAn")]
        public async Task<ActionResult> LayThongTinNhanVienDuAn([FromQuery] string tenDuAn)
        {
            var lstNhanVienDuAn = await _context.NhanVienDuans.Select(item => new
            {
                MaNV = item.MaNhanVien,
                MaDuAn = item.MaDuAn,
                TenDuAn = item.MaDuAnNavigation.TenDuAn,
                TenNhanVien = item.MaNhanVienNavigation.TenNv,
                DiaDiem = item.MaDuAnNavigation.MaDiaDiemNavigation.TenDiaDiem,
                TenPB = item.MaNhanVienNavigation.MaPbNavigation.TenPb
            }).Where(n => n.TenDuAn.Contains(tenDuAn)).ToListAsync();
            return Ok(lstNhanVienDuAn);
        }
        [HttpGet("LayThongTinNhanVienDuAnSQLRaw")]
        public async Task<ActionResult> LayThongTinNhanVienDuAnSQLRaw([FromQuery] string tenDuAn)
        {
            SqlParameter paramTenDuAn = new SqlParameter("@tenDuAn", $"%{tenDuAn}%");
            var lstNhanVienDuAnSQLRaw = await _context.Database.SqlQueryRaw<NhanVienDuanDTO>("SELECT NhanVien.Id AS MaNV, NhanVien.TenNV, PhongBan.TenPB, DuAn.TenDuAn, DiaDiem.TenDiaDiem as 'DiaDiem' FROM NhanVien_DuAn JOIN NhanVien ON NhanVien_DuAn.MaNhanVien = NhanVien.Id JOIN PhongBan ON NhanVien.MaPb = PhongBan.Id JOIN DuAn ON NhanVien_DuAn.MaDuAn = DuAn.Id JOIN DiaDiem ON DuAn.MaDiaDiem = DiaDiem.Id WHERE DuAn.TenDuAn LIKE @tenDuAn", paramTenDuAn).ToListAsync();
            return Ok(lstNhanVienDuAnSQLRaw);
        }

        [HttpGet("LayThongTinNhanVienTheoDuAn/{maDuAn}")]
        public async Task<ActionResult> LayThongTinNhanVienTheoDuAn(int maDuAn)
        {
            SqlParameter paramMaDuAn = new SqlParameter("@MaDuAn", DbType.Int32) { Value = maDuAn };

            var lstNhanVienTheoDuAn = await _context.Database.SqlQueryRaw<ChiTietDuAnDTO>("EXEC sp_layDanhSachNhanVienTheoDuAn @MaDuAn", paramMaDuAn).ToListAsync();
            foreach (var duAn in lstNhanVienTheoDuAn)
            {
                duAn.ConvertJsonNhanVienDuAn();
            }
            return Ok(lstNhanVienTheoDuAn);
        }
        [HttpGet("LayDanhSachNhanVienTheoDuAnLinQ/{maDuAn}")]
        public async Task<ActionResult> LayDanhSachNhanVienTheoDuAnLinQ(int maDuAn)
        {
            var lstNhanVienTheoDuAn = await _context.NhanVienDuans.Where(nvda => nvda.MaDuAn == maDuAn)
                .Select(nvda => new
                {
                    MaDuAn = nvda.MaDuAn,
                    TenDuAn = nvda.MaDuAnNavigation.TenDuAn,
                    MaNhanVien = nvda.MaNhanVien,
                    TenNV = nvda.MaNhanVienNavigation.TenNv,
                    NgaySinh = nvda.MaNhanVienNavigation.NgaySinh.Value.ToString("dd/MM/yyyy"),
                    SoDienThoai = nvda.MaNhanVienNavigation.SoDienThoai
                }).ToListAsync();
            return Ok(lstNhanVienTheoDuAn);
        }


        //Định nghĩa lấy danh sách dự án của từng nhân viên tham gia (tham số mã nhân viên)

        [HttpPost("ThemNhanVienNhanh")]
        public async Task<ActionResult> ThemNhanVienNhanh([FromBody] ThemNhanVienNhanhDTO model)
        {
            InsertDynamicStoreProcedure<ThemNhanVienNhanhDTO>(model);
            var res = await _context.NhanViens.Select(nv => new
            {
                MaNV = nv.Id,
                TenNV = nv.TenNv,
                SoDT = nv.SoDienThoai
            }).ToListAsync();
            return Ok(res);
        }


        [HttpPost("ThemNhanhDiaDiem")]
        public async Task<ActionResult> ThemNhanhDiaDiem([FromBody] ThemNhanhDiaDiemDTO model)
        {
             InsertDynamicStoreProcedure<ThemNhanhDiaDiemDTO>(model);
        
            return Ok(await _context.DiaDiems.ToListAsync());

        }


        private void InsertDynamicStoreProcedure<T>(T model) where T : class
        {
             /*
                @TableName: NhanVien
                @DanhSachTenCot: TenNV, SoDienThoai, MaPB
                @DanhSachGiaTri: ['Nguyễn văn tèo','09090909',2]
            */
            //Lấy table name
            string tableName = typeof(T).GetCustomAttribute<TableAttribute>()?.Name ?? typeof(T).Name;
            //Lấy tên cột động
            PropertyInfo[] danhSachCot = typeof(T).GetProperties();
            string danhSachTenCot = $@"{string.Join(", ", danhSachCot.Select(cot => cot.Name))}";
            //Lấy giá trị động
            string danhSachGiaTri = $"[{string.Join(", ", danhSachCot.Select(cot => $"\"{cot.GetValue(model)}\""))}]"; //N'["Nguyễn Văn Tèo", "09090909", "2"]'

            Console.WriteLine($@"danh sach gia tri: {danhSachGiaTri}");
            Console.WriteLine($@"danh sach ten cot: {danhSachTenCot}");


            SqlParameter paramTableName = new SqlParameter("@TableName", DbType.String) { Value = tableName };
            SqlParameter paramDanhSachTenCot = new SqlParameter("@DanhSachTenCot", DbType.String) { Value = danhSachTenCot };
            SqlParameter paramDanhSachGiaTri = new SqlParameter("@DanhSachGiaTri", DbType.String) { Value = danhSachGiaTri };
            var ketQua =  _context.Database.SqlQueryRaw<int>("EXEC InsertDynamicData_JSON @TableName, @DanhSachTenCot, @DanhSachGiaTri", paramTableName, paramDanhSachTenCot, paramDanhSachGiaTri).ToListAsync();
           
        }


    }






}

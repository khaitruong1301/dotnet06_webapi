using System;
using System.Collections.Generic;

namespace backend_netcore_dotnet06.Models.DBQuanLyNhanVien;

public partial class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public int DepartmentId { get; set; }
}

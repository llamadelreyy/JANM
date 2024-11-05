using System;
using System.Collections.Generic;

namespace PBTPro.Shared.Models;

public partial class Department
{
    public int Deptid { get; set; }

    public string? Deptname { get; set; }

    public string? Deptdescription { get; set; }

    public bool Isactive { get; set; }
}

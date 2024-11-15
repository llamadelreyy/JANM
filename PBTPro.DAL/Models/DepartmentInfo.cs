using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class DepartmentInfo
{
    public int DeptId { get; set; }

    public string DeptCode { get; set; } = null!;

    public string DeptDepartName { get; set; } = null!;

    public string? DeptDescription { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public string DeptStatus { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class CompoundAct
{
    public long ActId { get; set; }

    public string ActCode { get; set; } = null!;

    public string? ActName { get; set; }

    public string ActOffenceCode { get; set; } = null!;

    public string? ActOffenceName { get; set; }

    public string? ActTransactionCode { get; set; }

    public string? ActTransactionName { get; set; }

    public string? ActDeptCode { get; set; }

    public string? ActDeptName { get; set; }

    public decimal? ActAmount1 { get; set; }

    public decimal? ActPeriod1 { get; set; }

    public decimal? ActAmount2 { get; set; }

    public decimal? ActPeriod2 { get; set; }

    public decimal? ActAmount3 { get; set; }

    public decimal? ActPeriod3 { get; set; }

    public decimal? ActNoticeAmount { get; set; }

    public decimal? ActNoticePeriod { get; set; }

    public decimal? ActFnoticeAmount { get; set; }

    public decimal? ActFnoticePeriod { get; set; }

    public decimal? ActCourtAmount { get; set; }

    public decimal? ActPbtCode { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}

using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class CompoundInfo
{
    public long CompoundId { get; set; }

    public string CompoundNo { get; set; } = null!;

    public decimal? CompoundPbtCode { get; set; }

    public string? CompoundActCode { get; set; }

    public string? CompoundOffenceCode { get; set; }

    public string? CompoundTransCode { get; set; }

    public string? CompoundOffenderId { get; set; }

    public string? CompoundOffenderName { get; set; }

    public string? CompoundOffenderAddr1 { get; set; }

    public string? CompoundOffenderAddr2 { get; set; }

    public string? CompoundOffenderAddr3 { get; set; }

    public string? CompoundOffenderArea { get; set; }

    public decimal? CompoundOffenderPcode { get; set; }

    public string? CompoundOffenderState { get; set; }

    public DateOnly? CompoundDate { get; set; }

    public decimal? CompoundAmount { get; set; }

    public string? CompoundLicenseNo { get; set; }

    public string? CompoundVehiclePlate { get; set; }

    public string? CompoundRoadTax { get; set; }

    public string? CompoundStatus { get; set; }

    public string? CompoundPayStatus { get; set; }

    public DateOnly? CompoundPayDate { get; set; }

    public string? CompoundVehicleType { get; set; }

    public string? CompoundVehicleBrand { get; set; }

    public string? CompoundVehicleModel { get; set; }

    public DateOnly? CompoundCourtDate { get; set; }

    public decimal? CompoundPayAmount { get; set; }

    public string? CompoundDesc { get; set; }

    public string? CompoundOfficerCode { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}

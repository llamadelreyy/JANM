using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBTPro.Shared.Models.RequestPayLoad
{
    public class UserProfileViewModel
    {
        public int? ProfileId { get; set; }

        public string? UserId { get; set; }

        public string? PhotoUrl { get; set; }

        public string? Name { get; set; }

        public DateOnly? Dob { get; set; }

        public string? Icno { get; set; }

        public int? NatId { get; set; }

        public int? RaceId { get; set; }

        public bool? IsMarried { get; set; }

        public string? AddrLine1 { get; set; }

        public string? AddrLine2 { get; set; }

        public char? PostCode { get; set; }

        public int? CityId { get; set; }

        public string? CityView { get; set; }

        public int? DistrictId { get; set; }

        public string? DistrictView { get; set; }

        public int? StateId { get; set; }

        public string? StateView { get; set; }

        public int? CountryId { get; set; }

        public string? CountryView { get; set; }

        public char? AcceptTerms1 { get; set; }

        public char? AcceptTerms2 { get; set; }

        public char? AcceptTerms3 { get; set; }

        public string? Email { get; set; }

        public string? EmployeeNo { get; set; }

        public int? DepartmentId { get; set; }

        public string? DepartmentView { get; set; }

        public int? SectionId { get; set; }

        public string? SectionView { get; set; }

        public int? UnitId { get; set; }

        public string? UnitView { get; set; }

    }
}

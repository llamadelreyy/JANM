/*
Project: PBT Pro
Description: Setup Borang Model to handel payload to/from API
Author: Ismail
Date: November 2024
Version: 1.0

Additional Notes:
- 

Changes Logs:
11/11/2024 - initial create
*/
using System.ComponentModel.DataAnnotations;

namespace PBTPro.DAL.Models.PayLoads
{
    public class config_form_field_view
    {
        public int field_id { get; set; }

        public string field_form_type { get; set; } = null!;

        [Required(ErrorMessage = "Ruangan Name diperlukan.")]
        [StringLength(50, ErrorMessage = "Melebihi had maksimum aksara")]
        public string field_name { get; set; } = null!;

        [Required(ErrorMessage = "Ruangan Label diperlukan.")]
        [StringLength(50, ErrorMessage = "Melebihi had maksimum aksara.")]
        public string field_label { get; set; } = null!;

        [Required(ErrorMessage = "Ruangan Jenis diperlukan.")]
        public string field_type { get; set; } = null!;

        [RequiredIfApiSeededFalseAndTypeDropdown(ErrorMessage = "Ruangan Pilihan diperlukan.")]
        public string? field_option { get; set; }

        [RequiredIfApiSeededTrue(ErrorMessage = "Ruangan URL Sumber diperlukan.")]
        public string? field_source_url { get; set; }

        public bool field_required { get; set; }

        public bool field_api_seeded { get; set; }

        public int field_orders { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class RequiredIfApiSeededTrueAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var model = (config_form_field_view)validationContext.ObjectInstance;

            // Check if ApiSeeded is true and SourceUrl is null or empty
            if (model.field_api_seeded && string.IsNullOrEmpty((string?)value))
            {
                return new ValidationResult(ErrorMessage ?? "Ruangan URL Sumber diperlukan.", new List<string> { "field_source_url" });
            }

            return ValidationResult.Success;
        }
    }


    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class RequiredIfApiSeededFalseAndTypeDropdownAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var model = (config_form_field_view)validationContext.ObjectInstance;

            if (!model.field_api_seeded && model.field_type == "dropdown" && string.IsNullOrEmpty((string?)value))
            {
                return new ValidationResult(ErrorMessage ?? "Ruangan Pilihan diperlukan.", new List<string> { "field_option" });
            }

            return ValidationResult.Success;
        }
    }
}

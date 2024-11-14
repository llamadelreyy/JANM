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

namespace PBTPro.Shared.Models.RequestPayLoad
{
    public class SetupBorangInputModel
    {
        public int RecId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Melebihi had maksimum aksara")]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(50, ErrorMessage = "Melebihi had maksimum aksara")]
        public string Label { get; set; } = null!;

        [Required]
        public string Type { get; set; } = null!;

        public string? Option { get; set; }

        public string? SourceUrl { get; set; }

        public bool Required { get; set; }

        public bool ApiSeeded { get; set; }

        public int Orders { get; set; }
    }

    public class SetupBorangListModel
    {
        public string FormType { get; set; } = null!;

        public int RecId { get; set; }

        [Required(ErrorMessage = "Ruangan Name diperlukan.")]
        [StringLength(50, ErrorMessage = "Melebihi had maksimum aksara")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Ruangan Label diperlukan.")]
        [StringLength(50, ErrorMessage = "Melebihi had maksimum aksara.")]
        public string Label { get; set; } = null!;

        [Required(ErrorMessage = "Ruangan Jenis diperlukan.")]
        public string Type { get; set; } = null!;

        [RequiredIfApiSeededFalseAndTypeDropdown(ErrorMessage = "Ruangan Pilihan diperlukan.")]
        public string? Option { get; set; }

        [RequiredIfApiSeededTrue(ErrorMessage = "Ruangan URL Sumber diperlukan.")]
        public string? SourceUrl { get; set; }

        public bool Required { get; set; }

        public bool ApiSeeded { get; set; }

        public int Orders { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class RequiredIfApiSeededTrueAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var model = (SetupBorangListModel)validationContext.ObjectInstance;

            // Check if ApiSeeded is true and SourceUrl is null or empty
            if (model.ApiSeeded && string.IsNullOrEmpty((string?)value))
            {
                return new ValidationResult(ErrorMessage ?? "Ruangan URL Sumber diperlukan.", new List<string> { "SourceUrl" });
            }

            return ValidationResult.Success;
        }
    }


    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class RequiredIfApiSeededFalseAndTypeDropdownAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var model = (SetupBorangListModel)validationContext.ObjectInstance;

            if (!model.ApiSeeded && model.Type == "dropdown" && string.IsNullOrEmpty((string?)value))
            {
                return new ValidationResult(ErrorMessage ?? "Ruangan Pilihan diperlukan.", new List<string> { "Option" });
            }

            return ValidationResult.Success;
        }
    }
}

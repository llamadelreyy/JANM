using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PBTPro.DAL.Models
{
    public partial class user_password
    {
        public int profile_id { get; set; }

        public string profile_user_id { get; set; } = null!;

        public string userOldPassword { get; set; } = "";

        [Required(ErrorMessage = "Medan Katalaluan lama perlu diisi.")]
        [Compare("userOldPassword", ErrorMessage = "Katalaluan yang dimasukkan tidak sama dengan katalaluan yang asal.")]
        public string userNowPassword { get; set; } = "";

        [Required(ErrorMessage = "Medan Katalaluan baru perlu diisi."), RegularExpression(@"^(?=.*\d)(?=.*[~`!@#$%^&*()--+={}\[\]|\\:;""'<>,.?/_])(?=.*[a-z])(?=.*[A-Z]).{8,}$", ErrorMessage = "Kata laluan tidak sah!")]
        [DataType(DataType.Password)]
        [StringLength(255, ErrorMessage = "Min panjang adalah 8 aksara campuran abjad, nombor dan aksara khas.", MinimumLength = 8)]
        public string userNewPassword { get; set; } = "";

        [Required(ErrorMessage = "Medan Sahkan Katalaluan perlu diisi.")]
        [DataType(DataType.Password)]
        [StringLength(255, ErrorMessage = "Min panjang adalah 8 aksara.", MinimumLength = 8)]
        [Compare("userNewPassword", ErrorMessage = "Katalaluan yang dimasukkan tidak sama.")]
        public string userConfirmPassword { get; set; } = "";
    }
}

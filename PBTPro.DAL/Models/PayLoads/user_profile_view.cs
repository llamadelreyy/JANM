using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBTPro.DAL.Models.PayLoads
{
    public class user_profile_view
    {
        public int user_id { get; set; }
        public string? user_name { get; set; }
        public string? full_name { get; set; }
        public string? idno { get; set; }
        public string? email { get; set; }
        public string? phone_number { get; set; }
        public DateTime? pwd_update_at { get; set; }
        public DateTime? last_login { get; set; }
        public string? photo_filename { get; set; }
        public string? sign_filename { get; set; }
        public int? dept_id { get; set; }
        public string? dept_name { get; set; }
        public int? div_id { get; set; }
        public string? div_name { get; set; }
        public int? unit_id { get; set; }
        public string? unit_name { get; set; }
        public string? photo_path_url { get; set; }
        public string? sign_path_url { get; set; }
        public List<user_profile_role>? user_roles { get; set; }
    }

    public class user_profile_role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDefaultRole { get; set; }
        public bool IsMobileUser { get; set; }
        public string? DefaultPage { get; set; }
    }

    public class update_signature_input_model
    {
        public int user_id { get; set; }
        public IFormFile? sign_image { get; set; }
    }

    public class update_avatar_input_model
    {
        public int user_id { get; set; }
        public IFormFile? avatar_image { get; set; }
    }

    public class update_password_input_model
    {
        [Required(ErrorMessage = "Medan kata laluan semasa perlu diisi.")]
        [DataType(DataType.Password)]
        [StringLength(255, ErrorMessage = "Min panjang adalah 8 aksara campuran abjad, nombor dan aksara khas.", MinimumLength = 8)]
        public string? current_password { get; set; }

        [Required(ErrorMessage = "Medan kata laluan baharu perlu diisi."), RegularExpression(@"^(?=.*\d)(?=.*[~`!@#$%^&*()--+={}\[\]|\\:;""'<>,.?/_])(?=.*[a-z])(?=.*[A-Z]).{8,}$", ErrorMessage = "Kata laluan baharu tidak sah!")]
        [DataType(DataType.Password)]
        [StringLength(255, ErrorMessage = "Min panjang adalah 8 aksara campuran abjad, nombor dan aksara khas.", MinimumLength = 8)]
        public string? new_password { get; set; }

        [Required(ErrorMessage = "Medan sahkan katalaluan perlu diisi.")]
        [DataType(DataType.Password)]
        [StringLength(255, ErrorMessage = "Min panjang adalah 8 aksara.", MinimumLength = 8)]
        [Compare("new_password", ErrorMessage = "Katalaluan yang dimasukkan tidak sama.")]
        public string? valid_new_password { get; set; }

    }

    public class update_profile_input_model
    {
        public int user_id { get; set; }
        public string? user_name { get; set; }
        public string? full_name { get; set; }
        public string? idno { get; set; }
        public string? email { get; set; }
        public string? phone_number { get; set; }
        public string? photo_filename { get; set; }
        public string? sign_filename { get; set; }
        public int? dept_id { get; set; }
        public int? div_id { get; set; }
        public int? unit_id { get; set; }
        public string? photo_path_url { get; set; }
        public string? sign_path_url { get; set; }
        public int? selected_role { get; set; }

    }
}

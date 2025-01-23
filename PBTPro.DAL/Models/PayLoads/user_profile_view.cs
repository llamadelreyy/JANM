using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
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
        public string? new_password { get; set; }
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

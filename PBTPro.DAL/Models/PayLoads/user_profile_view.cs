using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBTPro.DAL.Models.PayLoads
{
    public class user_profile_view : user_profile
    {
        public string? profile_employee_no { get; set; }

        public string? profile_role { get; set; }

        public string? dept_name { get; set; }

        public string? div_name { get; set; }

        public string? unit_name { get; set; }

        public string? profile_photo_url { get; set; }

        public string? profile_signature_url { get; set; }

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
}

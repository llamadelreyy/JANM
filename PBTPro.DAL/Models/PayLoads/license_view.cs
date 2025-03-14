using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBTPro.DAL.Models.PayLoads
{
    public class license_view
    {
        public long license_hist_id { get; set; }
        public long hist_id_info { get; set; }
        public string license_hist_account { get; set; } = null!;

        public string? license_hist_holder { get; set; }

        public DateOnly? license_hist_startd { get; set; }

        public DateOnly? license_hist_endd { get; set; }

        public string? license_hist_addr1 { get; set; }

        public string? license_hist_addr2 { get; set; }

        public string? license_hist_addr3 { get; set; }

        public string? license_hist_area { get; set; }

        public decimal? license_hist_pcode { get; set; }

        public string? license_hist_state { get; set; }

        public DateTime? created_date { get; set; }

        public DateTime? updated_date { get; set; }

        public long license_id { get; set; }

        public decimal? license_pbt_origin { get; set; }

        public string license_account_number { get; set; } = null!;

        public string? license_type { get; set; }

        public string? license_risk_status { get; set; }

        public DateOnly? license_apply_date { get; set; }

        public DateOnly? license_approved_date { get; set; }

        public DateOnly? license_start_date { get; set; }

        public DateOnly? license_end_date { get; set; }

        public decimal? license_period { get; set; }

        public string? license_status { get; set; }

        public string? license_business_name { get; set; }

        public string? license_business_addr1 { get; set; }

        public string? license_business_addr2 { get; set; }

        public string? license_business_addr3 { get; set; }

        public string? license_business_area { get; set; }

        public decimal? license_business_pcode { get; set; }

        public string? license_business_state { get; set; }

        public decimal? license_amount { get; set; }

        public decimal? license_amount_balance { get; set; }

        public string? license_payment_status { get; set; }

        public string? license_period_status { get; set; }

        public string? license_longitud { get; set; }

        public string? license_latitude { get; set; }
    }
    public class mst_licensee_view
    {
        public string? nama_pemilik { get; set; }
        public string? emel_pemilik { get; set; }
        public string? notel_pemilk { get; set; }        
        public string icno_pemilik { get; set; }
        public string? nama_perniagaan { get; set; }
        public string alamat_perniagaan { get; set; }
        public int lesen_id { get; set; }
        public string? lesen_acc_no { get; set; }
        public DateOnly? tarikh_mula_isu { get; set; }
        public DateOnly? tarikh_tamat_isu { get; set; }
        public DateOnly? tarikh_renewal { get; set; }
        public string ops_name { get; set; }       
        public string? statename { get; set; }
        public string? districtname { get; set; }
        public string? townname { get; set; }
        public string? status_lesen { get; set; }
        public string? dokumen_sokongan { get; set; }
        public string? g_activity_1 { get; set; }
        public string? g_activity_2 { get; set; }
        public string? g_activity_3 { get; set; }
    }
}

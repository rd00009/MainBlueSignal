using System;
using System.ComponentModel.DataAnnotations;

namespace BlueSignalCore.Models
{
    public partial class MarketData : BaseEntity
    {
        [Key]
        public long Id { get; set; }

        public string ProductTypeID { get; set; }

        public string SymbolName { get; set; }

        public string SymbolCode { get; set; }

        public string QuantTradingType { get; set; }

        public decimal? Price { get; set; }

        public decimal? EntryPrice { get; set; }

        public DateTime? EntryDate { get; set; }

        public DateTime? ExitDate { get; set; }

        public string Result { get; set; }
    }




    public class WP_User
    {

        public string ID { get; set; }
        public string user_login { get; set; }
        public string user_password { get; set; }
        public string user_nicename { get; set; }
        public string user_email { get; set; }
        public string user_registered { get; set; }
        public string display_name { get; set; }
        public WP_UserBundle user_bundle { get; set; }
    }


    public enum WP_UserBundle
    {
        BluFractal = 0,
        BluNeural = 1,
        BluQuant = 2,
        BluCombo = 3,
    }
}

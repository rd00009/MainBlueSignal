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
}

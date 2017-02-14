using AutoMapper;
using BlueSignalCore.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueSignalCore.Dto
{
    [NotMapped]
    public class MarketDataDto : MarketData
    {
        public string ProductName { get; set; }
    }
}



using AutoMapper;
using BlueSignalCommon;
using BlueSignalCore.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueSignalCore.Dto
{
    [NotMapped]
    public class MarketDataDto : MarketData
    {
        public string ProductName { get; set; }
        public string StrEntryDate
        {
            get
            {
                return EntryDate.GetShortDateString();
            }
        }
        public string StrExitDate {
            get
            {
                return ExitDate.GetShortDateString();
            }
        }
    }
}



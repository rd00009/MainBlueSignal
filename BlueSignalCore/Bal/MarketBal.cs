using AutoMapper;
using BlueSignalCore.Dto;
using BlueSignalCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueSignalCore.Bal
{
    public class MarketBal : BaseBal
    {
        public MarketBal()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<MarketData, MarketDataDto>());
        }

        public async Task<IEnumerable<MarketDataDto>> GetMarketData()
        {
            try
            {
                var list = new List<MarketDataDto>();
                using (var rep = uw.MarketDataRepository)
                {
                    var m = rep.Where(a => a.IsActive).ToList();
                    if (m.Any())
                        list.AddRange(m.Select(a => Mapper.Map<MarketDataDto>(a)));
                }
                return await Task.FromResult(list);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int SaveMarketData(MarketDataDto vm)
        {
            var result = -1;
            using (var rep = uw.MarketDataRepository)
            {
                var model = Mapper.Map<MarketData>(vm);
                if (model.Id > 0)
                {
                    var current = rep.GetSingle(model.Id);
                    current.CreatedBy = vm.CreatedBy;
                    current.CreatedDate = vm.CreatedDate;
                    current.EntryDate = vm.EntryDate;
                    current.EntryPrice = vm.EntryPrice;
                    current.ExitDate = vm.ExitDate;
                    current.IsActive = vm.IsActive;
                    current.ModifiedBy = vm.ModifiedBy;
                    current.ModifiedDate = vm.ModifiedDate;
                    current.Price = vm.Price;
                    current.ProductTypeID = vm.ProductTypeID;
                    current.QuantTradingType = vm.QuantTradingType;
                    current.Result = vm.Result;
                    current.SymbolCode = vm.SymbolCode;
                    current.SymbolName = vm.SymbolName;

                    result = Convert.ToInt32(rep.Update(current));
                }
                else
                    result = Convert.ToInt32(rep.Create(model));
            }
            return result;
        }

        public async Task<IEnumerable<MarketDataDto>> GetMarketData(string productTypeId)
        {
            try
            {
                var list = new List<MarketDataDto>();
                using (var rep = uw.MarketDataRepository)
                {
                    var m = rep.Where(a => a.IsActive && a.ProductTypeID.Equals(productTypeId)).ToList();
                    if (m.Any())
                        list.AddRange(m.Select(a => Mapper.Map<MarketDataDto>(a)));
                }
                return await Task.FromResult(list);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

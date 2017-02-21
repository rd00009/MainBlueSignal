using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using BlueSignalCore.Dto;
using BlueSignalCore.Models;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;


namespace BlueSignalCore.Bal
{
    public class MarketBal : BaseBal
    {
        public MarketBal()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<MarketData, MarketDataDto>();
                cfg.CreateMap<MarketDataDto, MarketData>();
            });
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

        public async Task<int> SaveMarketData(MarketDataDto vm)
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
            return await Task.FromResult(result);
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

        public async Task<MarketDataDto> GetMarketDataById(long id)
        {
            try
            {
                var vm = new MarketDataDto();
                using (var rep = uw.MarketDataRepository)
                {
                    var m = rep.Where(a => a.Id == id).FirstOrDefault();
                    if (m != null)
                        vm = Mapper.Map<MarketDataDto>(m);
                }
                return await Task.FromResult(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public WP_User GetWpUser(string un, string pwd)
        {


            var user = new WP_User();

            MySqlConnection myConnection = new MySqlConnection(ConfigurationSettings.AppSettings["Wb_ConnectionString"]);
            string strSQL = "SELECT * FROM wp_g3b4k2u7_users where user_login='" + un + "' and user_pass='" + pwd + "'";
            MySqlDataAdapter myDataAdapter = new MySqlDataAdapter(strSQL, myConnection);
            DataSet myDataSet = new DataSet();
            myDataAdapter.Fill(myDataSet, "my_users");


            if (myDataSet != null && myDataSet.Tables.Count > 0 && myDataSet.Tables[0].Rows.Count > 0)
            {
                //var objInList = myDataSet.Tables[0].ToList<WP_User>();
                var objInList = myDataSet.Tables[0].Rows[0];
                user.ID = Convert.ToString(objInList["ID"]).Trim();
                user.user_login = Convert.ToString(objInList["user_login"]).Trim();
                user.user_password = Convert.ToString(objInList["user_pass"]).Trim();
                user.user_nicename = Convert.ToString(objInList["user_nicename"]).Trim();
                user.user_email = Convert.ToString(objInList["user_email"]).Trim();
                user.user_registered = Convert.ToString(objInList["user_registered"]).Trim();
                user.display_name = Convert.ToString(objInList["display_name"]).Trim();
                if (user != null && user.ID != "0" && user.ID != "")
                {
                    return user;
                }

            }


            return new WP_User();
        }
    }

    public static class Extensions
    {
        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }

            return result;
        }

        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(System.DayOfWeek))
                {
                    DayOfWeek day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), row[property.Name].ToString());
                    property.SetValue(item, day, null);
                }
                else
                {
                    property.SetValue(item, row[property.Name], null);
                }
            }
            return item;
        }
    }
}

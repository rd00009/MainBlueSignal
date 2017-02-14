using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BlueSignal.Common;
using BlueSignal.Models;
using BluSignalHelpMethod;
using Comman.DBAccess;
using Newtonsoft.Json;
using BlueSignalCore.Dto;

namespace BlueSignal.Controllers
{
    public class HomeController : BaseController
    {
        public string apiKey = BluSignalComman.APIkey;
        WebClientHelp webClientHelp = new WebClientHelp();
        public async Task<ActionResult> Index()
        {
            return await Task.FromResult(View());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public ActionResult GoogleFinance()
        {
            ViewBag.Message = "Google Finance";
            return View();
        }


        public ActionResult MarketData()
        {
            return View();
        }


        public string GetGoogleFinanceInfo(string q)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string s = client.DownloadString("http://finance.google.com/finance/info?client=ig&q=" + q);
                    return s.Replace("//", "");
                }
            }
            catch (WebException ex) //if server is off it will throw exeception and here we need notify user
            {
            }

            return "Fail";

        }


        public string GetMarketData(string startDate, string Type, string symbol)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string s = client.DownloadString("http://marketdata.websol.barchart.com/getHistory.json?key=" + apiKey + "&symbol=" + symbol + "&type=" + Type + "&startDate=" + startDate);
                    return s.Replace("//", "");
                }
            }
            catch (WebException ex) //if server is off it will throw exeception and here we need notify user
            {
            }

            return "Fail";

        }

        public JsonResult GetMarketChartData(string startDate, string Type, string symbol)
        {
            try
            {
                startDate = BluSignalComman.DateTime9MonthBack;

                if (Type == "weekly")
                {
                    startDate = BluSignalComman.DateTime9MonthWeeksBack;
                }

                if (Type == "dailySecond" || Type == "dailyBulQuantData" || Type == "dailyBuleFractal" || Type == "dailyBlueNeural")
                {
                    Type = "daily";
                }

                using (WebClient client = new WebClient())
                {
                    string s = client.DownloadString("http://marketdata.websol.barchart.com/getHistory.json?key=" + apiKey + "&symbol=" + symbol + "&type=" + Type + "&startDate=" + startDate);
                    var returnstring = s.Replace("//", "");
                    var chartJsonData = Helpers.GetJsonResponse<List<resultsData>>(returnstring, "results");
                    var stringtoReturn = chartJsonData.ToList().Select(item => new object[]
                    {
                       item.tradingDay.ToString(),
                       //Convert.ToDecimal(Convert.ToDateTime(item.tradingDay).ToString("yyyyMMddHHmmssfff")),
                       Math.Round(Convert.ToDecimal((item.open)),2),
                       Math.Round(Convert.ToDecimal((item.high)),2),
                       Math.Round(Convert.ToDecimal((item.low)),2),
                       Math.Round(Convert.ToDecimal((item.close)),2)
                    });
                    return Json(stringtoReturn, JsonRequestBehavior.AllowGet);
                }
            }
            catch (WebException ex) //if server is off it will throw exeception and here we need notify user
            {
            }
            return Json("");
        }
        private JsonResult GetNameFromSymbol(string symbol)
        {
            // symbol = "QQQ";
            string ss = "http://marketdata.websol.barchart.com/getQuote.json?key=" + apiKey + "&symbols=" + symbol;

            string ss2 = webClientHelp.GetWebClientResponse(ss);


            return Json(ss2);


        }



        public async Task<JsonResult> GetAllMarketData(string startDate, string Type, string selectedCode)
        {
            startDate = BluSignalComman.DateTime9MonthBack;
            MarketDataViewModel vm = new MarketDataViewModel();

            //var types = new[] { "daily", "weekly", "monthly", "quarterly", "yearly" };
            var result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(selectedCode) && selectedCode.IndexOf("-") != -1)
                {
                    var codeArray = selectedCode.Split('-');
                    vm.Name = codeArray[0].Trim();
                    vm.Code = codeArray[1].Trim();
                }
                else { vm.Code = selectedCode; }

                var dddd = GetNameFromSymbol(vm.Code);
                if (dddd != null)
                {
                    vm.SymbolNameData = Convert.ToString(dddd.Data);




                }


                vm.LastTradingDay = "02 Feb 2017";

                using (WebClient client = new WebClient())
                {
                    var apiUrl = string.Empty;

                    /**DAILY DATA**/
                    apiUrl = string.Format("http://marketdata.websol.barchart.com/getHistory.json?key=" + apiKey + "&symbol={0}&type=daily&startDate={1}", vm.Code, startDate);
                    result = await client.DownloadStringTaskAsync(new System.Uri(apiUrl));
                    if (!string.IsNullOrEmpty(result))
                    {
                        result = result.Replace("//", "");
                        vm.MarketDataDaily = result;



                        var data = JsonConvert.DeserializeObject<Data>(result);
                        if (data != null && data.results != null && data.results.Any())
                        {

                            int lastDays = 90;
                            var maxRexord = data.results.OrderByDescending(a => a.tradingDay).Take(lastDays).Sum(x => Convert.ToSingle(x.volume));
                            if (maxRexord > 0)
                            {
                                vm.AverageVolumn = Convert.ToString(Math.Round((maxRexord / lastDays), 0));
                            }
                        }


                        vm.ClosingPrice = string.Empty;
                    }
                    /**DAILY DATA**/


                    /**Weekly DATA**/
                    apiUrl = string.Format("http://marketdata.websol.barchart.com/getHistory.json?key=" + apiKey + "&symbol={0}&type=weekly&startDate={1}", vm.Code, startDate);
                    result = await client.DownloadStringTaskAsync(new System.Uri(apiUrl));
                    if (!string.IsNullOrEmpty(result))
                    {
                        result = result.Replace("//", "");
                        vm.MarketDataWeekly = result;
                    }
                    /**Weekly DATA**/



                    /**Monthly DATA**/
                    apiUrl = string.Format("http://marketdata.websol.barchart.com/getHistory.json?key=" + apiKey + "&symbol={0}&type=monthly&startDate={1}", vm.Code, startDate);
                    result = await client.DownloadStringTaskAsync(new System.Uri(apiUrl));
                    if (!string.IsNullOrEmpty(result))
                    {
                        result = result.Replace("//", "");
                        vm.MarketDataMonthly = result;

                        var data = JsonConvert.DeserializeObject<Data>(result);
                        if (data != null && data.results != null && data.results.Any())
                        {
                            var maxRexord = data.results.OrderByDescending(a => a.timestamp).First();
                            if (maxRexord != null)
                                vm.ClosingPrice = maxRexord.close;






                        }
                    }
                    /**Monthly DATA**/



                    /**Quarterly DATA**/
                    apiUrl = string.Format("http://marketdata.websol.barchart.com/getHistory.json?key=" + apiKey + "&symbol={0}&type=quarterly&startDate={1}", vm.Code, startDate);
                    result = await client.DownloadStringTaskAsync(new System.Uri(apiUrl));
                    if (!string.IsNullOrEmpty(result))
                    {
                        result = result.Replace("//", "");
                        vm.MarketDataQuartely = result;
                    }
                    /**Quarterly DATA**/



                    /**Yearly DATA**/
                    apiUrl = string.Format("http://marketdata.websol.barchart.com/getHistory.json?key=" + apiKey + "&symbol={0}&type=yearly&startDate={1}", vm.Code, startDate);
                    result = await client.DownloadStringTaskAsync(new System.Uri(apiUrl));
                    if (!string.IsNullOrEmpty(result))
                    {
                        result = result.Replace("//", "");
                        vm.MarketDataYearly = result;
                    }
                    /**Yearly DATA**/

                    var chartJsonData = Helpers.GetJsonResponse<List<resultsData>>(result, "result");
                    var ChartData = new List<ChartDataModel>();
                    ChartData.AddRange(chartJsonData.ToList().Select(item => new ChartDataModel()
                    {
                        symbol = item.symbol,
                        close = Convert.ToDecimal(item.close),
                        high = Convert.ToDecimal(item.high),
                        low = Convert.ToDecimal(item.low),
                        open = Convert.ToDecimal(item.open),
                        TradingDayTimeStamp = Convert.ToDateTime(item.tradingDay).ToString("yyyyMMddHHmmssfff"),
                    }));
                    vm.ChartData = ChartData;
                }
            }
            catch (WebException ex) //if server is off it will throw exeception and here we need notify user
            {
                throw ex;
            }

            return Json(vm, JsonRequestBehavior.AllowGet);

        }

        public async Task<JsonResult> GetMarketDataById(long id)
        {
            JsonResult json = null;
            try
            {
                var vm = await MarketBal.GetMarketDataById(id);
                json = new JsonResult
                {
                    Data = vm,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (WebException ex) //if server is off it will throw exeception and here we need notify user
            {
                throw ex;
            }

            return json;

        }

        public async Task<JsonResult> GetMarketSetupData()
        {
            JsonResult json = null;
            try
            {
                var list = await MarketBal.GetMarketData();
                json = new JsonResult
                {
                    Data = new
                    {
                        blueFractal = list.Where(a => a.ProductTypeID.Equals("101")),
                        blueQuant = list.Where(a => a.ProductTypeID.Equals("103")),
                        livePortfolio = list,
                        Last10CompletedTrades = list.Where(a => a.ExitDate.HasValue).OrderBy(n => n.ExitDate).Take(10)
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

            }
            catch (WebException ex) //if server is off it will throw exeception and here we need notify user
            {
                throw ex;
            }

            return json;

        }

        public async Task<JsonResult> GetMarketSetDataByType(string typeId)
        {
            JsonResult json = null;
            try
            {
                var list = await MarketBal.GetMarketData(typeId);
                json = new JsonResult
                {
                    Data = list,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (WebException ex) //if server is off it will throw exeception and here we need notify user
            {
                throw ex;
            }

            return json;

        }

        [HttpPost]
        public ActionResult Contact(ContactViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var db = new BluSignalsEntities();

                    var conact = new ContactLog()
                    {

                        Name = model.Name,
                        Email = model.Email,
                        Message = model.Message,
                        Subject = model.Subject,
                        CreatedDate = DateTime.UtcNow

                    };


                    var emailTemp = db.EmailTemplates.Where(e => e.EmailType == 1).FirstOrDefault();

                    var EB = new StringBuilder(emailTemp.EmailBody);

                    EB.Replace("@Name", conact.Name);
                    EB.Replace("@Email", conact.Email);
                    EB.Replace("@Subject", conact.Subject);
                    EB.Replace("@Query", conact.Message);
                    MailHelper.SendMailMessage(CommonConfig.AdminEmailID, "", "", emailTemp.EmailSubject + "-" + conact.Subject, EB.ToString(), true);

                    db.ContactLogs.Add(conact);
                    db.SaveChanges();





                    var model2 = new ContactViewModel();
                    ModelState.Clear();
                    model2.IsSuccess = 2;
                    //model2.Name = string.Empty;
                    //model2.Email = string.Empty;
                    //model2.Message = string.Empty;
                    //model2.Subject = string.Empty;





                    return PartialView("_ContactLog", model2);
                }
                else
                {
                    model.IsSuccess = 1;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return PartialView("_ContactLog", model);
        }


        public async Task<JsonResult> SaveMarketData(MarketDataDto vm)
        {
            if (vm.Id > 0)
            {
                vm.ModifiedBy = 1;
                vm.ModifiedDate = DateTime.Now;
            }
            else
            {
                vm.CreatedBy = 1;
                vm.CreatedDate = DateTime.Now;
            }

            var result = await MarketBal.SaveMarketData(vm);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> DeleteMarketData(long id)
        {
            var current = await MarketBal.GetMarketDataById(id);
            if (current != null)
            {
                current.IsActive = false;
                current.ModifiedBy = 1;
                current.ModifiedDate = DateTime.Now;
                var result = await MarketBal.SaveMarketData(current);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json('0', JsonRequestBehavior.AllowGet);
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using BlueSignal.Common;
using BlueSignal.Models;
using BlueSignalCore.Bal;
using BlueSignalCore.Dto;
using BlueSignalCore.Models;
using BluSignalHelpMethod;
using Comman.DBAccess;
using Newtonsoft.Json;

namespace BlueSignal.Controllers
{



    public class HomeController : BaseController
    {
        public string apiKey = BluSignalComman.APIkey;
        WebClientHelp webClientHelp = new WebClientHelp();
        [LogonAuthorize]
        public async Task<ActionResult> Index()
        {
            return await Task.FromResult(View());
        }


        [AllowAnonymous]
        public async Task<ActionResult> Auth()
        {
            var userName = Request.QueryString["E"];
            var Password = Request.QueryString["P"];
            using (var bal = new MarketBal())
            {
                var user = bal.GetWpUser(userName, Password);
                if (user != null)
                {
                    SystemLogin(user);

                    await CheckUserBundle(user);
                    var loggedInUser = Session["SystemUser"];
                    return RedirectToAction("Index");
                }
                else
                {
                    SystemLogout();
                    //Redirect To Unknown payment page
                }
            }
            return await Task.FromResult(View());
        }

        private static async Task CheckUserBundle(WP_User user)
        {
            var client = new HttpClient();
            var requestContent = new FormUrlEncodedContent(new[] {
            new KeyValuePair<string, string>("apikey", "9i6t91dkbr"),
            new KeyValuePair<string, string>("apisecret","4tqpbbph1r"),
            new KeyValuePair<string, string>("email","sofia@gmail.com")//"brown@gmail.com")
        });
            string methodName = "getMember";

            HttpResponseMessage response = await client.PostAsync("https://www.blusignalsystems.com/wp-content/plugins/membermouse/api/request.php?q=/" + methodName, requestContent);

            // Get the response content.
            HttpContent responseContent = response.Content;

            // Get the stream of the content.
            using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            {
                var jsonString = (await reader.ReadToEndAsync()) + Environment.NewLine;

                jsonString = @"{'response_code':'200','response_message':'','response_data':{'member_id':44,'first_name':'Dee','last_name':'Menzies','is_complimentary':'false','registered':'2017 - 02 - 17 19:24:36','cancellation_date':'','last_logged_in':'2017 - 02 - 18 21:55:45','last_updated':'2017 - 02 - 18 21:55:45','days_as_member':3,'status':'1','status_name':'Active','membership_level':'2','membership_level_name':'Paid Membership','username':'menzies.dee @gmail.com','email':'menzies.dee @gmail.com','password':null,'phone':'1112223333','billing_address':'123 Maple','billing_city':'Houston','billing_state':'TX','billing_zip':'77002','billing_country':'United States','shipping_address':'123 Maple','shipping_city':'Houston','shipping_state':'TX','shipping_zip':'77002',
'shipping_country':'United States','bundles':[{'id':7,'name':'Single System - BluFractal (monthly)'},{'id':3,'name':'Single System - BluNeural (monthly)'}],'custom_fields':[{'id':2,'name':'Terms of Serv','value':''},{'id':3,'name':'Terms of Service','value':'mm_cb_on'}]}}";

                var objResponse = jsonString;
                Rootobject facebookFriends = new JavaScriptSerializer().Deserialize<Rootobject>(objResponse);

                if (facebookFriends != null && facebookFriends.response_code == "200" && facebookFriends.response_data != null && facebookFriends.response_data.bundles != null && facebookFriends.response_data.bundles.Count > 0)
                {
                    user.bundles = facebookFriends.response_data.bundles;
                }
            }
        }

        [LogonAuthorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [LogonAuthorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [LogonAuthorize]
        public ActionResult GoogleFinance()
        {
            ViewBag.Message = "Google Finance";
            return View();
        }

        [LogonAuthorize]
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


        

        public ActionResult authFailed()
        {
            return View();
        }



        private void SystemLogin(WP_User user)
        {
            if (Session["SystemUser"] != null)
            {
                Session.Remove("SystemUser");
            }
            Session.Add("SystemUser", user);
        }

        private void SystemLogout()
        {
            Session.Remove("SystemUser");
        }




        #region Market Data Admin Setup Actions

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

        #endregion
    }
}
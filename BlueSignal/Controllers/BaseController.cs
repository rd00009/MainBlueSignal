using BlueSignalCore.Bal;
using BlueSignalCore.Repository;
using System.Web.Mvc;

namespace BlueSignal.Controllers
{
    public class BaseController : Controller
    {
        private MarketBal _marketBal;

        public MarketBal MarketBal
        {
            get
            {
                if (_marketBal == null)
                    _marketBal = new MarketBal();
                return _marketBal;
            }
        }
    }
}
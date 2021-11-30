using Microsoft.AspNetCore.Mvc;
using NF.Common.Utility;
using NF.IBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NF.Web.Areas.Finance.Controllers
{
    [Area("Finance")]
    [Route("Finance/[controller]/[action]")]
    public class JdOracleController : Controller
    {
        private IContActualFinanceService _IContActualFinanceService;

        public JdOracleController(IContActualFinanceService IContActualFinanceService
       )
        {
            _IContActualFinanceService = IContActualFinanceService;
        }

        public IActionResult Index()
        {
            return View();
        }
        #region 
      
        public int Gtr()
        {
            var er = -1;
            try
            {
                Log4netHelper.Error("定时器进入oracl方法");
                
                return er;
            }
            catch (Exception r)
            {

                Log4netHelper.Error(er+r.Message);
                return er;
            }
           
        }
        #endregion
    }
}

using Microsoft.AspNetCore.Mvc;

namespace NF.WeiXinApp.Controllers
{
    public class EnterpriseController : Controller
    {
        /// <summary>
        /// 检查列表
        /// </summary>
        /// <param name="Wxzh">微信账号</param>
        /// <returns></returns>
        public IActionResult Index(string Wxzh)
        {
            ViewData["wxzh"] = Wxzh;
            return View();
        }

        /// <summary>
        /// 新增资料
        /// </summary>
        /// <param name="Wxzh">账号</param>
        /// <param name="FinanceType"></param>
        /// <returns></returns>
        public IActionResult EnterpriseAdd(string Wxzh, int Id)
        {
            ViewData["WxCurrUserId"] = Wxzh;// HttpContext.Session.GetString("WxUserId");
            ViewData["customerId"] = Id;
            return View();

        }

        /// <summary>
        /// 新增客户服务
        /// http://localhost:9066/Company/CustFuWuAdd?Wxzh=daiyekun&compId=996
        /// </summary>
        /// <param name="Wxzh">账号</param>
        /// <param name="compId">客户ID</param>
        /// <returns></returns>
        public IActionResult EnterpriseAttWuAdd(string Wxzh, int compId)
        {
            ViewData["WxCurrUserId"] = Wxzh;// HttpContext.Session.GetString("WxUserId");
            ViewData["CompanyId"] = compId;
            //清除垃圾数据
            string sqlstr = "delete ContAttacFile where AttId=-188";
            //_ICompAttachmentService.ExecuteSqlCommand(sqlstr);

            return View();

        }
    }
}

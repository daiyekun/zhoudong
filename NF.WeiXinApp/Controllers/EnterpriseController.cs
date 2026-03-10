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
    }
}

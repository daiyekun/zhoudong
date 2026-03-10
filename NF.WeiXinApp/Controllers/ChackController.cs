using Microsoft.AspNetCore.Mvc;

namespace NF.WeiXinApp.Controllers
{
    public class ChackController : Controller
    {
        /// <summary>
        /// 检查列表
        /// </summary>
        /// <param name="Wxzh"></param>
        /// <returns></returns>
        public IActionResult Index(string Wxzh)
        {
            ViewData["wxzh"] = Wxzh;
            return View();
        }

        /// <summary>
        /// 新增检测
        /// </summary>
        /// <param name="Wxzh">账号</param>
        /// <returns></returns>
        public IActionResult CheckAdd(string Wxzh, int Id)
        {
            ViewData["WxCurrUserId"] = Wxzh;
            ViewData["customerId"] = Id;
            return View();

        }
    }
}

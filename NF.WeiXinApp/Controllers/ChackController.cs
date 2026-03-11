using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NF.IBLL;
using NF.WeiXin.Lib.Utility;

namespace NF.WeiXinApp.Controllers
{
    public class ChackController : Controller
    {

        private IHttpContextAccessor _accessor;
        private ICheckFileService _checkFileService;
        public ChackController(IHttpContextAccessor httpContextAccessor,
            ICheckFileService checkFileService)
        {
            _accessor = httpContextAccessor;
            _checkFileService = checkFileService;
        }

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
            ViewData["CompanyId"] = Id;
            return View();

        }

        /// <summary>
        /// 测试   http://localhost:9066/Company/Detail?Id=996&FinanceType=0
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="FinanceType"></param>
        /// <returns></returns>
        public IActionResult Detail(int Id, int FinanceType)
        {

            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("WxUserId")))
            {

                var httpcontext = _accessor.HttpContext;
                var code = httpcontext.Request.Query["Code"];

                var accessToken = WeixinUtiliy.GetAccessTokenStr();
                var wxUser = WxQYHOAuth2Utility.SetSessionUser(accessToken, code);

                HttpContext.Session.SetString("WxUserId", wxUser.UserId);
                //var userwx = HttpContext.Session.GetString("WxUserId");
            }
            ViewData["contId"] = Id;
            var d = HttpContext.Session.GetString("WxUserId"); //"daiyekun"; ////
            //测试代码

            //var d = "daiyekun";
            ViewData["WxCurrUserId"] = d;// HttpContext.Session.GetString("WxUserId");
            ViewData["FinanceType"] = FinanceType;
            return View();
        }
    }
}

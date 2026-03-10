using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NF.IBLL;
using NF.WeiXin.Lib.Utility;

namespace NF.WeiXinApp.Controllers
{
    public class EnterpriseController : Controller
    {
        private IHttpContextAccessor _accessor;
        private IEnterpriseFileService _enterpriseFileService;
        public EnterpriseController(IHttpContextAccessor httpContextAccessor,
            IEnterpriseFileService  enterpriseFileService)
        {
            _accessor = httpContextAccessor;
            _enterpriseFileService= enterpriseFileService;
        }
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

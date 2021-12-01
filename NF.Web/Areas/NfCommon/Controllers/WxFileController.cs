using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NF.Common.Models;
using NF.Common.Utility;
using NF.ViewModel.Extend.Enums;
using NF.Web.Utility;
using NF.Web.Utility.Common;
using NF.Web.Utility.Filters;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NF.Web.Areas.NfCommon.Controllers
{

    /// <summary>
    /// 微信上传附件
    /// </summary>
    [Area("NfCommon")]
    [Route("NfCommon/[controller]/[action]")]
    public class WxFileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        


      

        /// <summary>
        ///小文件
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult WxUpload1(string imgbase64)
        {
            UploadFileInfo uploadFileInfo = new UploadFileInfo();
            var pics= imgbase64.Split(',');

            
           // var pics= StringHelper.Strint2ArrayString1(imgbase64);
            var fname = $"{Guid.NewGuid().ToString()}";
            var extend = ".png";
            switch (pics[0])
            {

                case "data:image/png;base64":
                    fname = fname + ".png";
                    extend = ".png";
                    break;
                case "data:image/gif;base64":
                    fname = fname + ".gif";
                    extend = ".gif";
                    break;
                case "data:image/jpg;base64":
                    fname = fname + ".jpg";
                    extend = ".jpg";
                    break;
                case "data:image/jpeg;base64":
                    fname = fname + ".jpeg";
                    extend = ".jpeg";
                    break;

            }
            uploadFileInfo.GuidFileName = fname;
            uploadFileInfo.Extension = extend;
            uploadFileInfo.FolderName= EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 0);
            uploadFileInfo.SourceFileName = $"{uploadFileInfo.FolderName}/{uploadFileInfo.GuidFileName}";
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "Uploads", EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 0)
                        , fname);

            var filebyte=FileBinaryConvertHelper.FromBase64String(pics[1]);

             FileBinaryConvertHelper.Bytes2File(filebyte, path);
           

            return new CustomResultJson(new RequstResult()
            {
                Msg = "上传成功",
                Code = 0,
                Data= uploadFileInfo


            });
        }



    }
}

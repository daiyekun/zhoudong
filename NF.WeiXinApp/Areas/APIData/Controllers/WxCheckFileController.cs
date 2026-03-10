using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NF.BLL;
using NF.Common.Models;
using NF.Common.Utility;
using NF.IBLL;
using NF.Model.Models;
using NF.ViewModel.Extend.Enums;
using NF.WeiXinApp.Utility;
using NF.WeiXinApp.Utility.Common;
using NF.WeiXinApp.Utility.Filters;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NF.WeiXinApp.Areas.APIData.Controllers
{

    /// <summary>
    /// 企业资料
    /// </summary>
    [Area("NfCommon")]
    [Route("NfCommon/[controller]/[action]")]
    public class WxCheckFileController : Controller
    {
        private ICheckFileService  _checkFileService;
        public WxCheckFileController(ICheckFileService  checkFileService)
        {
            _checkFileService = checkFileService;

        }
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
            var pics = imgbase64.Split(',');


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
            uploadFileInfo.FolderName = EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 19);
            uploadFileInfo.SourceFileName = $"/Uploads/{uploadFileInfo.FolderName}/{uploadFileInfo.GuidFileName}";
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "Uploads", EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 19)
                        , fname);

            var filebyte = FileBinaryConvertHelper.FromBase64String(pics[1]);

            FileBinaryConvertHelper.Bytes2File(filebyte, path);


            return new WxResultJson(new RequstResult()
            {
                Msg = "上传成功",
                Code = 0,
                Data = uploadFileInfo


            });
        }


        /// <summary>
        ///多图片上传
        /// </summary>
        /// <param name="imgbase64">图片信息base64压缩</param>
        /// /// <param name="commpId">客户ID</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult WxUpload1s(string imgbase64, int commpId)
        {


            var pics = imgbase64.Split(',');


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
            var attacFile = new CheckFile();
            attacFile.IsDelete = 0;
            attacFile.FileType = 0;//文件类型，图片
            attacFile.CreateDateTime = DateTime.Now;
            attacFile.CreateUserId = 1;
            attacFile.ModifyUserId = 1;
            attacFile.ModifyDateTime = DateTime.Now;
            attacFile.CompanyId = commpId;
            attacFile.AttId = -188;//附件信息
            attacFile.Extend = extend;
            attacFile.GuidFileName = fname;
            attacFile.FolderName = EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 19);
            attacFile.FilePath = $"/Uploads/{attacFile.FolderName}/{attacFile.GuidFileName}";
            var path = Path.Combine(
                       Directory.GetCurrentDirectory(), "Uploads", EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 19)
                       , fname);

            var filebyte = FileBinaryConvertHelper.FromBase64String(pics[1]);

            FileBinaryConvertHelper.Bytes2File(filebyte, path);


            var info = _checkFileService.Add(attacFile);



            return new WxResultJson(new RequstResult()
            {
                Msg = "上传成功",
                Code = 0,
                Data = info


            });
        }


        /// <summary>
        /// 上传大文件
        /// </summary>
        /// <param name="downLoadAndUploadRequestInfo">上传文件时请求对象</param>
        /// <returns>返回上传后信息</returns>
        [HttpPost]
        [DisableFormValueModelBinding]

        public async Task<IActionResult> UploadVideoMax(string imgbase64, int commpId, string video, string compressedVideo)
        {



            var path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 19));
            FormValueProvider formModel;
            //UploadFileInfo uploadFileInfo = new UploadFileInfo();
            string extend = ".mp4";
            var fname = $"{Guid.NewGuid().ToString()}";
            var attacFile = new CheckFile();
            attacFile.IsDelete = 0;
            attacFile.FileType = 1;//文件类型，视频
            attacFile.CreateDateTime = DateTime.Now;
            attacFile.CreateUserId = 1;
            attacFile.ModifyUserId = 1;
            attacFile.ModifyDateTime = DateTime.Now;
            attacFile.CompanyId = commpId;
            attacFile.AttId = -188;//附件信息
            attacFile.Extend = extend;
            attacFile.GuidFileName = $"{fname}{extend}";
            // attacFile.FileName = fileName;
            attacFile.FolderName = EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 19);
            attacFile.FilePath = $"/Uploads/{attacFile.FolderName}/{attacFile.GuidFileName}";
            //attacFile.ThumPath = repic;

            formModel = await Request.StreamFiles_Video(path, attacFile);

            var viewModel = new MyViewModel();

            var bindingSuccessful = await TryUpdateModelAsync(viewModel, prefix: "",
                valueProvider: formModel);


            if (!bindingSuccessful)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
            }
            //生成缩略图
            var repic = FFmpegUtiltiy.CreateVideoPic(attacFile.GuidFileName);
            attacFile.ThumPath = repic;
            attacFile.FilePath = $"/Uploads/{attacFile.FolderName}/{attacFile.GuidFileName}";


            //return Ok(viewModel);
            return new WxResultJson(new RequstResult()
            {
                Msg = "上传成功",
                Code = 0,
                Data = attacFile


            });
        }


        /// <summary>
        /// 添加客户
        /// </summary>
        /// <param name="info">添加客户</param>
        /// <returns></returns>
        //[CustomAction2CommitFilter]
        [HttpPost]
        public IActionResult AddVedioInfo([FromBody] CheckFile attacFile)
        {
            try
            {
                //保存数据

                attacFile.IsDelete = 0;
                attacFile.FileType = 1;//文件类型，视频
                attacFile.CreateDateTime = DateTime.Now;
                attacFile.CreateUserId = 1;
                attacFile.ModifyUserId = 1;
                attacFile.ModifyDateTime = DateTime.Now;
                attacFile.CompanyId = attacFile.CompanyId;
                attacFile.AttId = -188;//附件信息
                attacFile.Extend = attacFile.Extend;
                attacFile.GuidFileName = attacFile.GuidFileName;
                // attacFile.FileName = fileName;
                attacFile.FolderName = EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 19);
                attacFile.FilePath = attacFile.FilePath;
                var info = _checkFileService.Add(attacFile);

                return new WxResultJson(new RequstResult()
                {
                    Msg = "保存附件信息",
                    Code = 0,
                    Data = info


                });
            }
            catch (Exception ex)
            {
                Log4netHelper.Error(ex.Message);
                return new WxResultJson(new RequstResult()
                {
                    Msg = "报错信息失败",
                    Code = 1,

                });
            }
        }


        /// <summary>
        ///视频上传
        /// </summary>
        /// <param name="imgbase64">图片信息base64压缩</param>
        /// /// <param name="commpId">客户ID</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult WxUploadvideo(string imgbase64, int commpId, string video, string compressedVideo)
        {

            try
            {
                //Log4netHelper.Info("上传视频开始---");
                var fname = $"{Guid.NewGuid().ToString()}";
                string extend = ".mp4";

                // 获取上传的视频文件
                var videoFile = Request.Form.Files["video"];
                extend = Path.GetExtension(videoFile.FileName);

                // 获取压缩后的视频文件
                var compressedVideoFile = Request.Form.Files["compressedVideo"];

                // 获取视频文件名
                var fileName = videoFile.FileName;

                // 保存视频文件
                var tempdev4path = Path.Combine(
                          Directory.GetCurrentDirectory(), "Uploads", EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 19)
                          , $"{fname}{extend}");
                //Log4netHelper.Info($"视频路径---{tempdev4path}");
                using (var fileStream = new FileStream(tempdev4path, FileMode.Create))
                {
                    videoFile.CopyTo(fileStream);
                }
                //生成缩略图
                var repic = FFmpegUtiltiy.CreateVideoPic($"{fname}{extend}");

                //压缩图片保存压缩后的视频文件
                if (compressedVideoFile != null && compressedVideoFile.Length > 0)
                {
                    var dev4path = Path.Combine(
                           Directory.GetCurrentDirectory(), "Uploads", EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 19)
                           , ($"{fname}.png"));
                    using (var fileStream = new FileStream(dev4path, FileMode.Create))
                    {
                        compressedVideoFile.CopyTo(fileStream);
                    }
                }



                var attacFile = new CheckFile();
                attacFile.IsDelete = 0;
                attacFile.FileType = 1;//文件类型，视频
                attacFile.CreateDateTime = DateTime.Now;
                attacFile.CreateUserId = 1;
                attacFile.ModifyUserId = 1;
                attacFile.ModifyDateTime = DateTime.Now;
                attacFile.CompanyId = commpId;
                attacFile.AttId = -188;//附件信息
                attacFile.Extend = extend;
                attacFile.GuidFileName = $"{fname}{extend}";
                attacFile.FileName = fileName;
                attacFile.FolderName = EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 19);
                attacFile.FilePath = $"/Uploads/{attacFile.FolderName}/{attacFile.GuidFileName}";
                attacFile.ThumPath = repic;
                //var path = Path.Combine(
                //           Directory.GetCurrentDirectory(), "Uploads", EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 1)
                //           , ($"{fname}{extend}");
                var info = _checkFileService.Add(attacFile);
                return new WxResultJson(new RequstResult()
                {
                    Msg = "上传成功",
                    Code = 0,
                    Data = info


                });
            }
            catch (Exception ex)
            {

                Log4netHelper.Error(ex.Message);
                return new WxResultJson(new RequstResult()
                {
                    Msg = "上传失败",
                    Code = 501,



                });
            }
        }


        /// <summary>
        /// 清除数据
        /// </summary>
        /// <returns></returns>
        public IActionResult ClearData()
        {

            string sqlstr = $"delete ContAttacFile where AttId={-188}";

            _checkFileService.ExecuteSqlCommand(sqlstr);



            return new WxResultJson(new RequstResult()
            {
                Msg = "上传成功",
                Code = 0,


            });
        }

        /// <summary>
        /// 根据ID删除数据
        /// </summary>
        /// <returns></returns>
        public IActionResult DeleteData(int Id)
        {

            string sqlstr = $"delete ContAttacFile where Id={Id}";

            _checkFileService.ExecuteSqlCommand(sqlstr);



            return new WxResultJson(new RequstResult()
            {
                Msg = "上传成功",
                Code = 0,


            });
        }



    }
}

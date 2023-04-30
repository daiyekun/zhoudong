using NF.Common.Utility;
using NF.ViewModel.Extend.Enums;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NF.WeiXinApp.Utility
{
    public class FFmpegUtiltiy
    {
        private static double GetVideoDuration(string videoFilePath)
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = @"C:\\ffmpeg\\ffmpeg\\bin\\ffprobe";
            //var escapedPath = videoFilePath.Replace(@"\", @"\\");
            process.StartInfo.Arguments = $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 {videoFilePath}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }

            if (!double.TryParse(output, out double duration))
            {
                throw new Exception($"Failed to parse video duration: {output}");
            }

            return duration * 1000;
        }

        /// <summary>
        /// 视频生成缩略图
        /// 使用ffmepg
        /// </summary>
        public async static Task<string> CreateVideoPic(string videoFileName)
        {
            try
            {
                // 设置视频文件路径
                var videoFilePath = Path.Combine(
                      Directory.GetCurrentDirectory(), "Uploads", EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 1)
                      , $"{videoFileName}");
                //var videoFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "videos", videoFileName);

                // 获取视频截图的时间点，这里使用视频时长的一半
                var position = new TimeSpan(0, 0, 0, 0, (int)(GetVideoDuration(videoFilePath) / 2));


                // 设置缩略图文件名
                var thumbnailFileName = $"{Path.GetFileNameWithoutExtension(videoFileName)}-{position.TotalSeconds}.jpg";

                // 设置缩略图文件路径
                // var thumbnailFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "thumbnails", thumbnailFileName);
                var thumbnailFilePath = Path.Combine(
                     Directory.GetCurrentDirectory(), "Uploads", EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 1)
                     , "thumbnails", $"{thumbnailFileName}");
                // 如果缩略图文件已存在，则直接返回缩略图路径
                if (System.IO.File.Exists(thumbnailFilePath))
                {
                    //return Json(new { thumbnailPath = $"/thumbnails/{thumbnailFileName}" });
                    return $"/Uploads/{EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 1)}/thumbnails/{thumbnailFileName}";
                }

                // 使用 FFmpeg 查询视频信息并生成缩略图
                var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = @"C:\\ffmpeg\\ffmpeg\\bin\\ffmpeg";
                process.StartInfo.Arguments = $"-ss {position:hh\\:mm\\:ss\\.fff} -i {videoFilePath} -vframes 1 -filter:v scale=-2:120 -q:v 2 {thumbnailFilePath}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();
                //await process.WaitForExitAsync();
                process.Close();

                //process.WaitForExit();
                //await Task.Run(() => process.WaitForExit());
                // 返回缩略图路径
                //return Json(new { thumbnailPath = $"/thumbnails/{thumbnailFileName}" });
                return $"/Uploads/{EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 1)}/thumbnails/{thumbnailFileName}"; ;
            }
            catch (Exception ex)
            {
                Log4netHelper.Error(ex.Message);
                //_logger.LogError(ex, "Failed to generate thumbnail for video {0}", videoFileName);
                //return Json(new { error = "Failed to generate thumbnail" });
                return "";
            }

        }


    }
}

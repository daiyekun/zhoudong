using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NF.Common.Utility
{

    /// <summary>
    /// 工具类：文件与二进制流间的转换
    /// </summary>
    public class FileBinaryConvertHelper
    {
        /// <summary>
        /// 将文件转换为byte数组
        /// </summary>
        /// <param name="path">文件地址</param>
        /// <returns>转换后的byte数组</returns>
        public static byte[] File2Bytes(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                return new byte[0];
            }
            FileInfo fi = new FileInfo(path);
            byte[] buff = new byte[fi.Length];

            FileStream fs = fi.OpenRead();
            fs.Read(buff, 0, Convert.ToInt32(fs.Length));
            fs.Close();

            return buff;
        }

        /// <summary>
        /// 将byte数组转换为文件并保存到指定地址
        /// </summary>
        /// <param name="buff">byte数组</param>
        /// <param name="savepath">保存地址</param>
        public static void Bytes2File(byte[] buff, string savepath)
        {
            if (System.IO.File.Exists(savepath))
            {
                System.IO.File.Delete(savepath);
            }

            FileStream fs = new FileStream(savepath, FileMode.CreateNew);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(buff, 0, buff.Length);
            bw.Close();
            fs.Close();
        }
        /// <summary>
        /// //将二进制转成string类型，可以存到数据库里面了
        /// </summary>
        /// <param name="byteArray">二进制数组</param>
        /// <returns></returns>
        public static string ByteBase64String(byte[] byteArray)
        {
            return Convert.ToBase64String(byteArray); 
        }

        /// <summary>
        /// 字符串转换二进制
        /// </summary>
        /// <param name="bytestrng"></param>
        /// <returns></returns>
        public static byte[] FromBase64String(string bytestrng)
        {
            return Convert.FromBase64String(bytestrng);
        }

    }
}

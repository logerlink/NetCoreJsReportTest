using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;

namespace JsReportTest
{
    public class Util
    {
        /// <summary>
        /// 图片（路径）转base64
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ImageToBase64(string path)
        {
            using (Image image = Image.FromFile(path))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
        }

        /// <summary>
        /// 文字转条形码
        /// </summary>
        /// <param name="content"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="filePath"></param>
        /// <param name="codeFormat"></param>
        public static void CreateBarcode(string content, int width, int height, string filePath, string codeFormat = "Code128")
        {
            BarcodeWriter bw = new BarcodeWriter();
            EncodingOptions encodingOptions = new EncodingOptions
            {
                Height = height,
                Width = width,
                PureBarcode = true
            };
            bw.Options = encodingOptions;
            //使用ITF 格式，不能被现在常用的支付宝、微信扫出来
            //如果想生成可识别的可以使用 CODE_128 格式
            if (codeFormat == "CodeBar")
            {
                bw.Format = BarcodeFormat.CODABAR;
            }
            else if (codeFormat == "Code128")
            {
                bw.Format = BarcodeFormat.CODE_128;
            }
            else if (codeFormat == "Code39")
            {
                bw.Format = BarcodeFormat.CODE_39;
            }
            else
            {
                bw.Format = BarcodeFormat.CODE_128;
            }
            Bitmap bm = bw.Write(content);
            bm.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
            bm.Dispose();
            //MultiFormatWriter mutiWriter = new MultiFormatWriter();
            //BitMatrix bm = mutiWriter.encode(content, BarcodeFormat.CODABAR, width, height);
            //bm.Height
            //Bitmap img = new BarcodeWriter().Write(bm);
            //img.Save(filePath + content + ".jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);
        }
    }
}

using JdCat.Cat.Common;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.DrawingCore.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JdCat.Cat.FileService.Models
{
    public class ProductImage
    {
        static ProductImage()
        {
            var dir = Directory.GetCurrentDirectory();
            var productPath = Path.Combine(dir, "wwwroot", _defaultPath);
            if (!Directory.Exists(productPath))
            {
                Directory.CreateDirectory(productPath);
            }
            var logoPath = Path.Combine(dir, "wwwroot", _logoPath);
            if (!Directory.Exists(logoPath))
            {
                Directory.CreateDirectory(logoPath);
            }
            var licensePath = Path.Combine(dir, "wwwroot", _licencePath);
            if (!Directory.Exists(licensePath))
            {
                Directory.CreateDirectory(licensePath);
            }
        }
        public static string _defaultPath = "/File/Product";
        public static string _logoPath = "File/Logo";
        public static string _licencePath = "File/License";
        public static string _qrcodePath = "File/Qrcode";
        public int BusinessId { get; set; }
        public string Name { get; set; }
        public string Image400 { get; set; }
        public string Image200 { get; set; }
        public string Image100 { get; set; }
        public void Save(IHostingEnvironment environment)
        {
            // 建立商家目录
            var path = $"{environment.WebRootPath}/{_defaultPath}/{BusinessId}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // 建立400*300
            var path400 = $"{path}/400x300";
            if (!Directory.Exists(path400))
            {
                Directory.CreateDirectory(path400);
            }
            Save(Image400, Path.Combine(path400, Name));
            // 建立200*150
            var path200 = Path.Combine(path, "200x150");
            if (!Directory.Exists(path200))
            {
                Directory.CreateDirectory(path200);
            }
            Save(Image200, Path.Combine(path200, Name));
            // 建立100*75
            var path100 = Path.Combine(path, "100x75");
            if (!Directory.Exists(path100))
            {
                Directory.CreateDirectory(path100);
            }
            Save(Image100, Path.Combine(path100, Name));
        }
        private void Save(string source, string filepath)
        {
            using (var file = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                byte[] imageBytes = Convert.FromBase64String(source.Replace("data:image/jpeg;base64,", ""));
                file.Write(imageBytes, 0, imageBytes.Length);
            }
        }
        public string SaveLogo(IHostingEnvironment environment)
        {
            return Save(_logoPath, environment);
        }
        public string SaveLicense(IHostingEnvironment environment)
        {
            return Save(_licencePath, environment, true);
        }
        public string SaveQrcode(IHostingEnvironment environment)
        {
            return SaveSource(_qrcodePath, environment);
        }

        #region 私有
        ///
        private string Save(string parentPath, IHostingEnvironment environment, bool waterSign = false)
        {
            var dir = Path.Combine(environment.WebRootPath, parentPath, BusinessId + "");
            var extension = ".jpg";
            var content = string.Empty;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (Image400.Contains("data:image/png;base64"))
            {
                extension = ".png";
                content = Image400.Replace("data:image/png;base64,", "");
            }
            else
            {
                content = Image400.Replace("data:image/jpeg;base64,", "");
            }
            var filename = Name + extension;
            using (var file = new FileStream(Path.Combine(dir, filename), FileMode.OpenOrCreate, FileAccess.Write))
            {
                byte[] imageBytes = Convert.FromBase64String(content);
                file.Write(imageBytes, 0, imageBytes.Length);
            }
            if (waterSign)
            {
                WaterSign(Path.Combine(dir, filename), "Jiandanmao");
            }
            return filename;
        }
        private string SaveSource(string parentPath, IHostingEnvironment environment)
        {
            var dir = Path.Combine(environment.WebRootPath, parentPath);
            var extension = ".jpg";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var filename = Name + extension;
            using (var file = new FileStream(Path.Combine(dir, filename), FileMode.OpenOrCreate, FileAccess.Write))
            {
                byte[] imageBytes = Convert.FromBase64String(Image400);
                file.Write(imageBytes, 0, imageBytes.Length);
            }
            return filename;
        }

        #endregion

        /// <summary>
        /// 在图片上添加水印文字
        /// </summary>
        /// <param name="stream">图片文件</param>
        /// <param name="waterWords">需要添加到图片上的文字</param>
        /// <returns></returns>
        public void WaterSign(string file, string waterWords)
        {
            //创建一个图片对象用来装载要被添加水印的图片
            var img = Image.FromFile(file);

            //获取图片的宽和高
            int phWidth = img.Width;
            int phHeight = img.Height;

            //
            //建立一个bitmap，和我们需要加水印的图片一样大小
            var bmPhoto = new Bitmap(phWidth, phHeight, img.PixelFormat);
            //UtilHelper.Log($"坐标：{img.HorizontalResolution},{img.VerticalResolution}");

            //这里直接将我们需要添加水印的图片的分辨率赋给了bitmap
            //bmPhoto.SetResolution(img.re, img.VerticalResolution);

            //Graphics：封装一个 GDI+ 绘图图面。
            Graphics grPhoto = Graphics.FromImage(bmPhoto);

            //设置图形的品质
            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;

            //将我们要添加水印的图片按照原始大小描绘（复制）到图形中
            grPhoto.DrawImage(
                img,                    //  要添加水印的图片
                new Rectangle(0, 0, phWidth, phHeight), // 根据要添加的水印图片的宽和高
                0,                           // X方向从0点开始描绘
                0,                           // Y方向
                phWidth,                     // X方向描绘长度
                phHeight,                    // Y方向描绘长度
                GraphicsUnit.Pixel);         // 描绘的单位，这里用的是像素

            //根据图片的大小我们来确定添加上去的文字的大小
            //在这里我们定义一个数组来确定
            int[] sizes = new int[] { 50, 48, 46, 44, 42, 40, 38, 36, 34, 32, 30, 28, 26, 24, 22, 20, 18, 16, 14, 12, 10, 8, 6, 4, 2 };

            //字体
            Font crFont = null;
            //矩形的宽度和高度，SizeF有三个属性，分别为Height高，width宽，IsEmpty是否为空
            SizeF crSize = new SizeF();

            //利用一个循环语句来选择我们要添加文字的型号
            //直到它的长度比图片的宽度小
            for (int i = 0; i < sizes.Length; i++)
            {
                crFont = new Font(new FontFamily("微软雅黑"), sizes[i], FontStyle.Bold);

                //测量用指定的 Font 对象绘制并用指定的 StringFormat 对象格式化的指定字符串。
                crSize = grPhoto.MeasureString(waterWords, crFont);

                // ushort 关键字表示一种整数数据类型
                if ((ushort)crSize.Width < (ushort)phWidth)
                    break;
            }

            //截边5%的距离，定义文字显示(由于不同的图片显示的高和宽不同，所以按百分比截取)
            int yPixlesFromBottom = (int)(phHeight * .05);

            //定义在图片上文字的位置
            float wmHeight = crSize.Height;
            float wmWidth = crSize.Width;

            float xPosOfWm = phWidth / 2;
            float yPosOfWm = phHeight / 2;

            //封装文本布局信息（如对齐、文字方向和 Tab 停靠位），显示操作（如省略号插入和国家标准 (National) 数字替换）和 OpenType 功能。
            var StrFormat = new StringFormat();

            //定义需要印的文字居中对齐
            StrFormat.Alignment = StringAlignment.Center;

            //SolidBrush:定义单色画笔。画笔用于填充图形形状，如矩形、椭圆、扇形、多边形和封闭路径。
            //这个画笔为描绘阴影的画笔，呈灰色
            int m_alpha = Convert.ToInt32(255 * 0.2);
            SolidBrush semiTransBrush2 = new SolidBrush(Color.FromArgb(m_alpha, 234, 112, 66));

            //描绘文字信息，这个图层向右和向下偏移一个像素，表示阴影效果
            //DrawString 在指定矩形并且用指定的 Brush 和 Font 对象绘制指定的文本字符串。
            grPhoto.DrawString(waterWords,                        //string of text
                          crFont,                                 //font
                          semiTransBrush2,                        //Brush
                          new PointF(xPosOfWm + 1, yPosOfWm + 1), //Position
                          StrFormat);

            //从四个 ARGB 分量（alpha、红色、绿色和蓝色）值创建 Color 结构，这里设置透明度为153
            //这个画笔为描绘正式文字的笔刷，呈白色
            SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(153, 255, 255, 255));

            //第二次绘制这个图形，建立在第一次描绘的基础上
            grPhoto.DrawString(waterWords,                //string of text
                          crFont,                         //font
                          semiTransBrush,                 //Brush
                          new PointF(xPosOfWm, yPosOfWm), //Position
                          StrFormat);

            //释放资源，将定义的Graphics实例grPhoto释放，grPhoto功德圆满

            img.Dispose();//释放底图，解决图片保存时 “GDI+ 中发生一般性错误。”
            bmPhoto.Save(file);
            grPhoto.Dispose();
        }


    }

}

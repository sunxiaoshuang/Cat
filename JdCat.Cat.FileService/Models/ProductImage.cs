using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
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
            return Save(_licencePath, environment);
        }

        #region 私有
        ///
        private string Save(string parentPath, IHostingEnvironment environment)
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
            return filename;
        }

        #endregion
    }
}

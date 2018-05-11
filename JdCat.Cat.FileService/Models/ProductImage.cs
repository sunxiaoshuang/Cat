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
        public static string _defaultPath = "/File/Product";
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
    }
}

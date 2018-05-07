using JdCat.Cat.Model.Data;
using System;

namespace JdCat.Cat.Web.Models
{
    public class ProductModel : Product
    {
        public string Img400 { get; set; }
        public string Img200 { get; set; }
        public string Img100 { get; set; }
    }
}
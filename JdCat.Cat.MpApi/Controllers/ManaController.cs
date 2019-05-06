using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JdCat.Cat.MpApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSameDomain")]
    public class ManaController : BaseController<IMpRepository, Business>
    {
        public ManaController(IMpRepository service) : base(service)
        {
        }

        [HttpGet("business")]
        public async Task<IActionResult> GetBusinesses([FromQuery]string openid)
        {
            var businesses = await Service.GetBusinessForOpenIdAsync(openid);
            if (businesses == null) return NoContent();
            return Json(businesses.Select(a => new { a.Name, a.ID }));
        }

        [HttpGet("products/{id}")]
        public async Task<IActionResult> GetProducts(int id, [FromServices]IProductRepository rep)
        {
            var products = await rep.GetProductTreeAsync(id, true);
            return Json(products);
        }

        [HttpPut("products/{id}")]
        public IActionResult PutProduct(int id, [FromQuery]ProductStatus status, [FromServices]IProductRepository rep)
        {
            if (status == ProductStatus.Sale) rep.Up(id);
            else rep.Down(id);
            rep.Commit();
            return Ok("操作成功");
        }

    }
}
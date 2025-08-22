using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductMaintenance.Business.Interfaces;
using ProductMaintenance.Models;

namespace ProductMaintenance.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin,Manager")] // adjust or remove if this API should be public
    public class ProductApiController : ControllerBase
    {
        private readonly IProductProcess _process;

        public ProductApiController(IProductProcess process)
        {
            _process = process;
        }

        // GET: api/v1/Get/All/Products?pageSize=1000
        [Route("api/v{version:apiVersion}/Get/All/Products")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<ProductListItem>>> GetAllProductList([FromQuery] int pageSize = 1000)
        {
            if (pageSize <= 0) pageSize = 1000;
            var result = await _process.SearchAsync(query: null, page: 1, pageSize: pageSize, sortField: null, sortDir: null);
            return Ok(result.Items);
        }
    }
}

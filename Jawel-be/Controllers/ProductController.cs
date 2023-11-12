using Jawel_be.Dtos.Product;
using Jawel_be.Exceptions;
using Jawel_be.Models;
using Jawel_be.Services.ProductService;
using Jawel_be.Utils;
using Jawel_be.Validators.Product;
using Microsoft.AspNetCore.Mvc;

namespace Jawel_be.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productService.GetProducts();
            return Ok(products.Select(c => c.AsDto()).ToList());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productService.GetProductById(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product.AsDto());

        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            try
            {
                var validator = new CreateProductDtoValidator();
                var validationResult = await validator.ValidateAsync(createProductDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                Product newProduct = await _productService.CreateProduct(createProductDto);
                return CreatedAtAction(nameof(GetProduct), new { id = newProduct.Id }, newProduct.AsDto());
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            try
            {
                var validator = new UpdateProductDtoValidator();
                var validationResult = await validator.ValidateAsync(updateProductDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var product = await _productService.UpdateProduct(id, updateProductDto);
                return Ok(product.AsDto());
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productService.DeleteProduct(id);
                return Ok();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound();
            }
        }
    }

}

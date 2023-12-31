﻿using Jawel_be.Dtos.Category;
using Jawel_be.Exceptions;
using Jawel_be.Models;
using Jawel_be.Services.CategoryService;
using Jawel_be.Utils;
using Jawel_be.Validators.Category;
using Microsoft.AspNetCore.Mvc;

namespace Jawel_be.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetCategories();
            return Ok(categories.Select(c => c.AsDto()).ToList());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _categoryService.GetCategoryById(id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category.AsDto());

        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            var validator = new CreateCategoryDtoValidator();
            var validationResult = await validator.ValidateAsync(createCategoryDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            Category newCategory = await _categoryService.CreateCategory(createCategoryDto);
            return CreatedAtAction(nameof(GetCategory), new { id = newCategory.Id }, newCategory.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            try
            {
                var validator = new UpdateCategoryDtoValidator();
                var validationResult = await validator.ValidateAsync(updateCategoryDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var category = await _categoryService.UpdateCategory(id, updateCategoryDto);
                return Ok(category.AsDto());
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _categoryService.DeleteCategory(id);
                return Ok();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound();
            }
        }
    }

}

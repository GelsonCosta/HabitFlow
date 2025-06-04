using System.Security.Claims;
using HabitFlow.Application.Features.Categories.Commands.CreateCategory;
using HabitFlow.Application.Features.Categories.Commands.CreateCategory.Dtos;
using HabitFlow.Application.Features.Categories.Commands.DeleteCategory;
using HabitFlow.Application.Features.Categories.Commands.UpdateCategory;
using HabitFlow.Application.Features.Categories.Commands.UpdateCategory.Dtos;
using HabitFlow.Application.Features.Categories.Queries.GetCategories;
using HabitFlow.Application.Features.Categories.Queries.GetCategoryById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HabitFlow.Api.Controllers
{
    [ApiController]
    [Route("categories")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var query = new GetCategoriesQuery(userId);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var query = new GetCategoryByIdQuery(userId, id);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var command = new CreateCategoryCommand(userId, createCategoryDto);
            var result = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetCategory), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var command = new UpdateCategoryCommand(userId, id, updateCategoryDto);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var command = new DeleteCategoryCommand(userId, id);
            await _mediator.Send(command);

            return NoContent();
        }
    }
}

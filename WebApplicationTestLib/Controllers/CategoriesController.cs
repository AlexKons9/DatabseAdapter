using Microsoft.AspNetCore.Mvc;
using WebApplicationTestLib.Entities;

namespace WebApplicationTestLib.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryRepository _repository;

        public CategoriesController(CategoryRepository repository)
        {
            _repository = repository;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _repository.GetAsync(cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest,
                               detail: ex.Message,
                               title: "Failed to fetch categories");
            }
        }

        // GET api/categories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _repository.GetByIdAsync(id, cancellationToken);
                if (result == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest,
                               detail: ex.Message,
                               title: "Failed to fetch category",
                               instance: $"/api/categories/{id}");
            }
        }

        // POST api/categories
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Category category, CancellationToken cancellationToken)
        {
            try
            {
                await _repository.CreateAsync(category, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest,
                               detail: ex.Message,
                               title: "Failed to create category");
            }
        }

        // PUT api/categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] Category category, CancellationToken cancellationToken)
        {
            try
            {
                category.Id = id;  // Ensure the ID in the URL matches the entity's ID
                await _repository.UpdateAsync(category, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest,
                               detail: ex.Message,
                               title: "Failed to update category");
            }
        }

        // DELETE api/categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
        {
            try
            {
                await _repository.DeleteAsync(id, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest,
                               detail: ex.Message,
                               title: "Failed to delete category");
            }
        }
    }
}

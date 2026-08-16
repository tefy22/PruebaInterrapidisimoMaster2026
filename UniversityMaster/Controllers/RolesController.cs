using Application.Roles.CreateRol;
using Application.Roles.DeleteRol;
using Application.Roles.SearchRol;
using Domain.Abstractions;
using Domain.Roles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace UniversityMaster.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly ISender _sender;

        public RolesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> SearchRoles(CancellationToken cancellationToken = default)
        {
            var query = new SearchAllRolQuery();

            var result = await _sender.Send(query, cancellationToken);
            if (result.IsFailure)
                return BadRequest(result.Error);

            var response = Result.Success(result.Value);

            return Ok(response);
        }

        [HttpGet("{id:guid}", Name = "GetRoleById")]
        public async Task<IActionResult> SearchById(Guid id, CancellationToken cancellationToken = default)
        {
            var command = new SearchByIdRolQuery(id);
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(Result.Success(result.Value));
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRolCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            //return CreatedAtAction(nameof(SearchRoles), new { id = result.Value }, result.Value);
            //return CreatedAtRoute("GetRoleById", new { id = result.Value }, result.Value);

            return CreatedAtRoute("GetRoleById", new { id = result.Value }, Result.Success(result.Value));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteRole(Guid id, CancellationToken cancellationToken = default)
        {
            var command = new DeleteRolCommand(id);
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }

    }
}

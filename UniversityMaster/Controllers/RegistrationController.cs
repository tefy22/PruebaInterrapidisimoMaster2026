using Application.Registrations.CreateRegistration;
using Application.Registrations.SearchRegistration;
using Application.Registrations.UpdateRegistration;
using Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UniversityMaster.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/registrations")]
    public class RegistrationController : Controller
    {
        public readonly ISender _sender;

        public RegistrationController(ISender sender)
        {
            _sender = sender;
        }
        [HttpGet]
        public async Task<IActionResult> SearchRegistrations(CancellationToken cancellationToken = default)
        {
            var query = new SearchAllRegistrationQuery();
            var result = await _sender.Send(query, cancellationToken);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return Ok(Result.Success(result.Value));
        }
        [HttpGet("shared/{studentId:guid}")]
        public async Task<IActionResult> GetSharedSubjects(Guid studentId, CancellationToken cancellationToken = default)
        {
            var query = new GetSharedSubjectsQuery(studentId);
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(Result.Success(result.Value));
        }

        [HttpPost]
        public async Task<IActionResult> CreateRegistration([FromBody] CreateRegistrationCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteRegistration(Guid id, CancellationToken cancellationToken = default)
        {
            var command = new DeleteRegistrationCommand(id);

            var result = await _sender.Send(command, cancellationToken);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }
    }
}

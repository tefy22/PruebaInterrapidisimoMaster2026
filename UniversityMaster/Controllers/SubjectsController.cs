using Application.Subjects.CreateSubject;
using Application.Subjects.DeleteSubject;
using Application.Subjects.SearchSubject;
using Application.Subjects.UpdateSubject;
using Domain.Abstractions;
using Domain.Roles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace UniversityMaster.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/subjects")]
    public class SubjectsController : ControllerBase
    {
        private readonly ISender _sender;

        public SubjectsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("active")]
        public async Task<IActionResult> SearchActiveStatus(CancellationToken cancellationToken = default)
        {
            var query = new SearchActiveSubjectQuery();
            var result = await _sender.Send(query, cancellationToken);
            if (result.IsFailure)
                return BadRequest(result.Error);

            var response = Result.Success(result.Value);

            return Ok(response);
        }

        [HttpGet("all")]
        public async Task<IActionResult> SearchSubject(CancellationToken cancellationToken = default)
        {
            var query = new SearchAllSubjectQuery();
            var result = await _sender.Send(query, cancellationToken);
            if (result.IsFailure)
                return BadRequest(result.Error);

            var response = Result.Success(result.Value);

            return Ok(response);
        }

        [HttpGet("{id:guid}", Name = "GetSubjectsById")]
        public async Task<IActionResult> SearchById(Guid id, CancellationToken cancellationToken = default)
        {
            var query = new SearchByIdSubjectQuery(id);
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(Result.Success(result.Value));
        }


        [HttpPost]
        public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return CreatedAtRoute("GetSubjectsById", new { id = result.Value }, Result.Success(result.Value));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteSubject(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteSubjectCommand(id);
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }

        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> UpdateSubject(Guid id, [FromBody] UpdateSubjectCommand command, CancellationToken cancellationToken = default)
        {
            var request = new UpdateSubjectCommand(id, command.name, command.credits, command.idTeacher, command.estado);

            var result = await _sender.Send(request, cancellationToken);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(Result.Success(result.Value));
        }
    }
}

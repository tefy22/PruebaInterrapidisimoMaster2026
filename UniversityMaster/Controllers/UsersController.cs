using Application.Users.CreateUser;
using Application.Users.DeleteUser;
using Application.Users.LoginUser;
using Application.Users.SearchUser;
using Application.Users.UpdateUser;
using Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UniversityMaster.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        public readonly ISender _sender;

        public UsersController(ISender sender)
        {
            _sender = sender;
        }
        [Authorize]
        [HttpGet("students")]
        public async Task<IActionResult> SearchStudents(CancellationToken cancellationToken = default)
        {
            var query = new SearchStudentsQuery();
            var result = await _sender.Send(query, cancellationToken);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(Result.Success(result.Value));
        }
        [Authorize]
        [HttpGet("students/{id:guid}", Name = "GetStudentById")]
        public async Task<IActionResult> SearchStudentById(Guid id, CancellationToken cancellationToken = default)
        {
            var query = new SearchStudentByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(Result.Success(result.Value));
        }
        [Authorize]
        [HttpGet("teachers")]
        public async Task<IActionResult> SearchTeachers(CancellationToken cancellationToken = default)
        {
            var query = new SearchTeachersQuery();
            var result = await _sender.Send(query, cancellationToken);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(Result.Success(result.Value));
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SearchAllUsers(CancellationToken cancellationToken = default)
        {
            var query = new SearchAllUserQuery();
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            var response = Result.Success(result.Value);
            return Ok(response);
        }
        [Authorize]
        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command, CancellationToken cancellationToken = default)
        {
            var request = new UpdateUserCommand(
                id: id,
                dni: command.dni,
                name: command.name,
                lastName: command.lastName,
                email: command.email,
                password: command.password,
                phoneNumber: command.phoneNumber,
                roleId: command.roleId,
                status: command.status
            );

            var result = await _sender.Send(request, cancellationToken);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(Result.Success(result.Value));
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
        {
            var command = new LoginCommand(request.email, request.password);
            var result = await _sender.Send(command, cancellationToken);
            if (result.IsFailure)
                return Unauthorized(result.Error);

            return Ok(Result.Success(result.Value));
        }
        
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateUserCommand(
                dni: request.dni,
                name: request.name,
                lastName: request.lastName,
                email: request.email,
                password: request.password,
                phoneNumber: request.phoneNumber,
                roleId: request.roleId
            );
            var result = await _sender.Send(command, cancellationToken);
            if (result.IsFailure)
                return Unauthorized(result.Error);

            return Ok(Result.Success(result.Value));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteUserCommand(id);
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }
    }
}

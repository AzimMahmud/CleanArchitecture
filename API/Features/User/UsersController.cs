using Application.Features.Users.Queries.GetAllUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.User;

[ApiController]
[Route("api/[Controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        this._mediator = mediator;
    }
    
    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        var response = await _mediator.Send(new GetAllUserQuery());
        return Ok(response);
    }
}
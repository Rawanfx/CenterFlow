using CenterFlow.Application.Common.Models;
using CenterFlow.Application.Features.Auth.Common;
using MediatR;

namespace CenterFlow.Application.Features.Auth.Login;

public record LoginCommand(string Email, string Password) : IRequest<Response<LoginResult>>;

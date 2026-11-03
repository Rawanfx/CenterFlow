using CenterFlow.Application.Common.Models;
using MediatR;

namespace CenterFlow.Application.Features.Auth.ConfirmEmail
{
    public record ConfirmEmailCommand(string Email, string Token) : IRequest<Response<string>>;

}

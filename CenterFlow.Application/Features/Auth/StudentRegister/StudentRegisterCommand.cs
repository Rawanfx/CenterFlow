
using CenterFlow.Application.Common.Models;
using CenterFlow.Domain.Enum;
using MediatR;
namespace CenterFlow.Application.Features.Auth.StudentRegister;

public record StudentRegisterCommand(string FullName, string Email, string Password, GradeLevel Level) : IRequest<Response<string>>;

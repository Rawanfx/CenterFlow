using CenterFlow.Application.Common.Models;
using MediatR;

namespace CenterFlow.Application.Features.Auth.TeacherRegister;

public record TeacherRegisterCommand
    (string FullName, string Phone, string Email, Guid SubjectId,string Password) : IRequest<Response<string>>;


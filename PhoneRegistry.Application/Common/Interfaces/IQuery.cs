using MediatR;

namespace PhoneRegistry.Application.Common.Interfaces;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}

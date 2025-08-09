using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PhoneRegistry.Application.Features.Persons.Commands.CreatePerson;
using PhoneRegistry.Application.Features.Persons.Commands.DeletePerson;
using PhoneRegistry.Application.Features.Persons.Queries.GetPersonById;
using PhoneRegistry.Application.Features.Persons.Queries.GetAllPersons;
using PhoneRegistry.Application.Features.ContactInfos.Commands.AddContactInfo;
using PhoneRegistry.Application.Features.Reports.Commands.RequestReport;

namespace PhoneRegistry.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // AutoMapper
        services.AddAutoMapper(assembly);

        // FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        // Command Handlers
        services.AddScoped<CreatePersonCommandHandler>();
        services.AddScoped<DeletePersonCommandHandler>();
        services.AddScoped<AddContactInfoCommandHandler>();
        services.AddScoped<RequestReportCommandHandler>();

        // Query Handlers
        services.AddScoped<GetPersonByIdQueryHandler>();
        services.AddScoped<GetAllPersonsQueryHandler>();

        return services;
    }
}

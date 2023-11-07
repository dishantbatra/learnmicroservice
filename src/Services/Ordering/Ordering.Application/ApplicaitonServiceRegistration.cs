using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Behaviours;
using System.Reflection;

namespace Ordering.Application
{
    public static class ApplicaitonServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly()); //Profile class inhertied
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly()); // AbstractValidator class inherited
            services.AddMediatR(cfg =>
     cfg.RegisterServicesFromAssembly(typeof(ApplicaitonServiceRegistration).Assembly)); //IRequestHandler class inherited

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandleExceptionBehaviour<,>)); // Pipeline behaviour implemented
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            return services;
        }
    }
}

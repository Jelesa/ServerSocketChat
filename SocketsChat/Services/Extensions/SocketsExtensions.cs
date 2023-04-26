using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SocketsChat.Handlers.Abstract;
using SocketsChat.Handlers.Concrete;
using SocketsChat.Services.Abstract;
using SocketsChat.Services.Concrete;
using SocketsChat.Services.Middleware;

namespace SocketsChat.Services.Extensions
{
    public static class SocketsExtensions
    {
        public static IServiceCollection AddWebSocketsManager(this IServiceCollection services)
        {
            services.AddSingleton<IConnectionManager, ConnectionManager>();
            services.AddSingleton<ISocketHandler, SocketHandler>();
            services.AddSingleton<ISenderService, SenderService>();
            services.AddSingleton<IMessageProcessor, MessageProcessor>();

            return services;
        }

        public static IApplicationBuilder MapSockets(
            this IApplicationBuilder app,
            PathString path,
            SocketHandler socket)
        {
            return app.Map(path, (x) => x.UseMiddleware<SocketsMiddleware>());
        }
    }
}

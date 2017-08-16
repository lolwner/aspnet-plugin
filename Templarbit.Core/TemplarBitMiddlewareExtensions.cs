using Microsoft.AspNetCore.Builder;

namespace Templarbit.Core
{
    public static class TemplarbitMiddlewareExtensions
    {
        public static IApplicationBuilder UseTemplarbit(
            this IApplicationBuilder builder, TemplarbitMiddlewareModel model, ITemplarbitLogger logger)
        {
            return builder.UseMiddleware<TemplarbitMiddleware>(model, logger);
        }
    }
}

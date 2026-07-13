using Graveyard.API.Data;
using Graveyard.API.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Graveyard.API.Infrastructure;

// Her basarili yazma islemini (POST/PUT/DELETE) islem gunlugune yazar.
public class AuditFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executed = await next(); // once asil islemi calistir

        var http = context.HttpContext;
        var method = http.Request.Method;
        if (method != "POST" && method != "PUT" && method != "DELETE") return;
        if (http.User?.Identity?.IsAuthenticated != true) return; // anonim (login/public) loglanmaz

        var status = (executed.Result as IStatusCodeActionResult)?.StatusCode ?? http.Response.StatusCode;
        if (status < 200 || status >= 300) return; // sadece basarili islemler

        var controller = context.RouteData.Values["controller"]?.ToString();
        if (controller == "AuditLogs") return;

        var key = context.RouteData.Values["id"]?.ToString()
                  ?? context.RouteData.Values["ssn"]?.ToString()
                  ?? context.RouteData.Values["plotNumber"]?.ToString();
        var action = method == "POST" ? "Create" : method == "PUT" ? "Update" : "Delete";

        try
        {
            var db = http.RequestServices.GetRequiredService<GraveyardDbContext>();
            db.AuditLogs.Add(new AuditLog
            {
                Username = http.User.Identity?.Name ?? "?",
                Action = action,
                Entity = controller,
                EntityKey = key,
                Timestamp = DateTime.Now,
            });
            await db.SaveChangesAsync();
        }
        catch { /* audit yazilamazsa ana islemi etkileme */ }
    }
}

using System.Text;

public class BasicAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _username;
    private readonly string _password;

    public BasicAuthMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _username = config["APISettings:login"] ?? "";
        _password = config["APISettings:password"] ?? "";
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Пропускаем аутентификацию для не-главной страницы
        if (!context.Request.Path.Equals("/") && !context.Request.Path.Equals("/tenderhome"))
        {
            await _next(context);
            return;
        }

        string authHeader = context.Request.Headers["Authorization"];
        if (authHeader != null && authHeader.StartsWith("Basic "))
        {
            var encodedCredentials = authHeader.Split(" ")[1];
            var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
            var credentials = decodedCredentials.Split(':', 2);

            if (credentials.Length == 2 && credentials[0] == _username && credentials[1] == _password)
            {
                await _next(context);
                return;
            }
        }

        context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"TenderHome\"";
        context.Response.StatusCode = 401;
    }
}
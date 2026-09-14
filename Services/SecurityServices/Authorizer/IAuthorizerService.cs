namespace MakeItWork.Services.SecurityServices.Authorizer
{
    public interface IAuthorizerService
    {
        public Task SignInAsync(HttpContext context, string username, CancellationToken ct = default);
        public Task SignInAsync(HttpContext context, int userId, string username, string securityStamp);
        public Task SignOutAsync(HttpContext context);
    }
}

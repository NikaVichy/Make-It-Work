using MakeItWork.Models;

namespace MakeItWork.Services.SecurityServices.Authorizer
{
    public interface IAuthorizerService
    {
        public Task SignInAsync(HttpContext context, int id, string username);
        public Task SignInAsync(HttpContext context, int id, string username, UserAuthModel userAuthModel);
        public Task SignOutAsync(HttpContext context);
    }
}

namespace MakeItWork.Services.SecurityServices.Authenticator
{
    public interface IAuthenticatorService
    {
        Task<bool> TryValidateAuthModelAsync(string username, char[] password, CancellationToken ct = default);
    }
}

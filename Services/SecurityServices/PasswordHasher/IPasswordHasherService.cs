namespace MakeItWork.Services.SecurityServices.PasswordHasher
{
    public interface IPasswordHasherService
    {
        Task<string> HashValidatedPasswordAsync(char[] password);
        Task<bool> VerifyPasswordAsync(char[] password, string encodedHash);
    }
}

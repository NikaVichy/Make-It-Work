namespace MakeItWork.Services.SecurityServices.Totp.TotpGenerator
{
    public interface ITotpGenerator
    {
        byte[] GetSecret();
        string GetOtpAuth(string issuer, string escapedUsername, byte[] secret);
        string GetQR(string otpAuth);
        bool VerifyTotp(byte[] secret, char[] code);
    }
}

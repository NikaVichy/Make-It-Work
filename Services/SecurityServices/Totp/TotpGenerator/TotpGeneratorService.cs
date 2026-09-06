using OtpNet;
using QRCoder;
using System.Security.Cryptography;

namespace MakeItWork.Services.SecurityServices.Totp.TotpGenerator
{
    public class TotpGeneratorService : ITotpGenerator
    {
        public byte[] GetSecret()
        {
            return KeyGeneration.GenerateRandomKey(20);
        }
        public string GetOtpAuth(string issuer, string escapedUsername, byte[] secret)
        {
            var secretBase32 = Base32Encoding.ToString(secret);
            CryptographicOperations.ZeroMemory(secret);

            var otpAuth = $"otpauth://totp/{issuer}:{escapedUsername}?secret={secretBase32}&issuer={issuer}&digits=6";
            secretBase32 = null;

            return otpAuth;
        }
        public string GetQR(string otpAuth)
        {
            var qRCodeGenerator = new QRCodeGenerator();
            var data = qRCodeGenerator.CreateQrCode(otpAuth, QRCodeGenerator.ECCLevel.Q);
            otpAuth = null;

            var code = new PngByteQRCode(data);
            var pngBytes = code.GetGraphic(20);

            return Convert.ToBase64String(pngBytes);
        }
        public bool VerifyTotp(byte[] secret, char[] code)
        {
            var totp = new OtpNet.Totp(secret);

            var stringCode = code.ToString();
            Array.Clear(code);

            bool isValid = totp.VerifyTotp(stringCode, out long _, new VerificationWindow(1, 1));

            stringCode = null;
            CryptographicOperations.ZeroMemory(secret);

            return isValid;
        }
    }
}

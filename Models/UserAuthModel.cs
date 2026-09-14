using System.Security.Cryptography;

namespace MakeItWork.Models
{
    public class UserAuthModel : BaseModel
    {
        public string hashedPassword { get; set; }
        public byte authAttempts { get; set; }
        public DateTime lockedSince { get; set; }
        public bool TotpEnabled { get; set; }
        public byte[]? EncryptedTotpSecret { get; set; }
        public string SecurityStamp { get; set; } = Convert.ToBase64String(RandomNumberGenerator.GetBytes(24));
        public UserModel userModel { get; init; }
        public int userId { get; init; }
    }
}

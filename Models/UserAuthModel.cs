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

        public void UpdateModel(UserAuthModel userAuthModel)
        {
            hashedPassword = userAuthModel.hashedPassword;
            TotpEnabled = userAuthModel.TotpEnabled;
            EncryptedTotpSecret = userAuthModel.EncryptedTotpSecret;

            var bStamp = RandomNumberGenerator.GetBytes(24);
            SecurityStamp = Convert.ToBase64String(bStamp);
        }
        public void AddAuthAttempt()
        {
            authAttempts++;
            if(authAttempts == 5)
            {
                lockedSince = DateTime.UtcNow;
            }
        }

        public bool IsLocked()
        {
            if (authAttempts == 5)
            {
                if (DateTime.Compare(lockedSince.AddHours(1), DateTime.UtcNow) > 0)
                    return true;
            }
            return false;
        }
        public bool TryClearAuthAttempts()
        {
            if(authAttempts == 0) return false;

            authAttempts = 0;
            return true;
        }
    }
}

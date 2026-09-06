namespace MakeItWork.Models
{
    public class UserModel : BaseModel
    {
        public string username { get; set; }
        public string photoPath { get; set; } = string.Empty;
        public UserAuthModel userAuthModel { get; set; }
        public UserModel() { }
    }
}

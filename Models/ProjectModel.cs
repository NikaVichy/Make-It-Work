namespace MakeItWork.Models
{
    public class ProjectModel : BaseModel
    {
        public string projectName { get; set; }
        public string projectDescription { get; set; } = "";
        public int ownerId { get; set; }
        public UserModel owner { get; set; }
        public List<UserModel> users { get; set; } = [];
        public List<TaskModel> tasks { get; set; } = [];
    }
}

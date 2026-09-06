namespace MakeItWork.DTOModels
{
    public class TaskDTO : BaseDTO
    {
        public string authorName { get; set; }
        public int authorId { get; set; }
        public string taskName { get; set; }
        public string taskDescription { get; set; } = string.Empty;
        public DateTimeOffset createdAt { get; set; }
        public DateTimeOffset? dueTo { get; set; }
        public int projectId { get; set; }
        public string projectName { get; set; }
    }
}

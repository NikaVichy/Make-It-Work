namespace MakeItWork.Models
{
    public class TaskModel : BaseModel
    {
        public string taskName
        {
            get;
            set;
        }
        public string taskDescription
        {
            get;
            set { field = value ?? ""; }
        } = "";
        public DateTimeOffset createdAt {
            get;
            private init; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? dueTo
        {
            get
            {
                if (!field.HasValue) return null;
                return field.Value;
            }
            set
            {
                if (!value.HasValue)
                {
                    field = null;
                    return;
                }

                if (value.Value.Offset == TimeSpan.Zero)
                {
                    field = value.Value;
                    return;
                }

                field = value.Value.ToUniversalTime();
            }
        }
        public UserModel taskAuthor { get; init; }
        public int taskAuthorId { get; init; }
        public List<UserModel> assignedUsers
        {
            get; set
            {
                field = value ?? [];
            }
        } = [];
        public int taskProjectId { get; init; }
        public ProjectModel taskProject { get; init; } = null;
        public bool isCompleted { get; set; } = false;
        public bool isHidden { get; set; } = false;
    }
}

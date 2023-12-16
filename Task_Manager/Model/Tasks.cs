namespace Task_Manager.Model
{
    public class Tasks
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime DueData { get; set; }
        public int Priority { get; set; }
        public string Status { get; set; } = "";    
        public Guid UserId { get; set; }
    }
}

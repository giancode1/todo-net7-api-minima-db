namespace TodosApi.Models
{
    public class Todo
    {
        public Guid Id { get; set; }
        public string Task { get; set; } = "";
        public string? Description { get; set; }
        public bool IsComplete  { get; set; }
        public DateTime CreatedAt {get; set;}
    }
}
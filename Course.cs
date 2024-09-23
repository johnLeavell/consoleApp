namespace sqltest
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Credits { get; set; }
    }
}

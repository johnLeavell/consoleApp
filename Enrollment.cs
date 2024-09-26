namespace sqltest
{
    public class Enrollment
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrolledDate { get; set; }
        public string StudentName { get; set; }
        public string CourseName { get; set; }
    }
}
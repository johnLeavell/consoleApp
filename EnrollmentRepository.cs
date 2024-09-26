using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace sqltest
{
    public class EnrollmentRepository
    {
        private readonly string connectionString;

        public EnrollmentRepository(IConfiguration configuration)
        {
            connectionString = "Host=localhost;Username=postgres;Password=password;Database=postgres";
        }

        public async Task EnrollStudentAsync(Enrollment enrollment)
        {
            try
            {
                await using var dataSource = NpgsqlDataSource.Create(connectionString);
                await using var cmd = dataSource.CreateCommand(@"
                    INSERT INTO enrollments (student_id, course_id, enrolled_date) 
                    VALUES (@StudentId, @CourseId, @EnrolledDate)");
                cmd.Parameters.AddWithValue("StudentId", enrollment.StudentId);
                cmd.Parameters.AddWithValue("CourseId", enrollment.CourseId);
                cmd.Parameters.AddWithValue("EnrolledDate", enrollment.EnrolledDate);
                await cmd.ExecuteNonQueryAsync();

                Console.WriteLine("Student enrolled successfully.");
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        public async Task<List<Enrollment>> GetAllEnrollmentsAsync()
        {
            var enrollments = new List<Enrollment>();

            try
            {
                await using var dataSource = NpgsqlDataSource.Create(connectionString);
                await using var cmd = dataSource.CreateCommand(@"
                    SELECT e.student_id, (s.first_name || ' ' || s.last_name) AS student_name, e.course_id, c.name AS course_name, e.enrolled_date
                    FROM enrollments e
                    JOIN students s ON e.student_id = s.id
                    JOIN courses c ON e.course_id = c.id
                    ");
                
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    enrollments.Add(new Enrollment
                    {
                        StudentId = reader.GetInt32(0),
                        StudentName = reader.GetString(1),
                        CourseId = reader.GetInt32(2),
                        CourseName = reader.GetString(3),
                        EnrolledDate = reader.GetDateTime(4)
                    });
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return enrollments;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace sqltest
{
    public class StudentRepository
    {
        private string connectionString = "Host=localhost;Username=postgres;Password=password;Database=postgres";

        public async Task AddStudentAsync(Student student)
        {
            try
            {
                await using var dataSource = NpgsqlDataSource.Create(connectionString);
                await using var cmd = dataSource.CreateCommand(@"
                    INSERT INTO students (first_name, last_name, email, registration_date) 
                    VALUES (@FirstName, @LastName, @Email, @RegistrationDate)");
                cmd.Parameters.AddWithValue("FirstName", student.FirstName);
                cmd.Parameters.AddWithValue("LastName", student.LastName);
                cmd.Parameters.AddWithValue("Email", student.Email);
                cmd.Parameters.AddWithValue("RegistrationDate", student.RegistrationDate);
                await cmd.ExecuteNonQueryAsync();

                Console.WriteLine("Student added successfully.");
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

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            var students = new List<Student>();

            try
            {
                await using var dataSource = NpgsqlDataSource.Create(connectionString);
                await using var cmd = dataSource.CreateCommand("SELECT * FROM students");
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    students.Add(new Student
                    {
                        Id = reader.GetInt32(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        Email = reader.GetString(3),
                        RegistrationDate = reader.GetDateTime(4)
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

            return students;
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            try
            {
                await using var dataSource = NpgsqlDataSource.Create(connectionString);
                await using var cmd = dataSource.CreateCommand("SELECT * FROM students WHERE id = @Id");
                cmd.Parameters.AddWithValue("Id", id);
                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Student
                    {
                        Id = reader.GetInt32(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        Email = reader.GetString(3),
                        RegistrationDate = reader.GetDateTime(4)
                    };
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

            return null;
        }

        public async Task<List<Student>> SearchStudentsByNameAsync(string name)
        {
            var students = new List<Student>();

            try
            {
                await using var dataSource = NpgsqlDataSource.Create(connectionString);
                await using var cmd = dataSource.CreateCommand("SELECT * FROM students WHERE first_name ILIKE @Name OR last_name ILIKE @Name");
                cmd.Parameters.AddWithValue("Name", $"%{name}%");
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    students.Add(new Student
                    {
                        Id = reader.GetInt32(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        Email = reader.GetString(3),
                        RegistrationDate = reader.GetDateTime(4)
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

            return students;
        }

        public async Task UpdateStudentAsync(Student student)
        {
            try
            {
                await using var dataSource = NpgsqlDataSource.Create(connectionString);
                await using var cmd = dataSource.CreateCommand(@"
                    UPDATE students SET first_name = @FirstName, last_name = @LastName, email = @Email, registration_date = @RegistrationDate
                    WHERE id = @Id");
                cmd.Parameters.AddWithValue("Id", student.Id);
                cmd.Parameters.AddWithValue("FirstName", student.FirstName);
                cmd.Parameters.AddWithValue("LastName", student.LastName);
                cmd.Parameters.AddWithValue("Email", student.Email);
                cmd.Parameters.AddWithValue("RegistrationDate", student.RegistrationDate);
                await cmd.ExecuteNonQueryAsync();

                Console.WriteLine("Student updated successfully.");
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

        public async Task DeleteStudentAsync(int id)
        {
            try
            {
                await using var dataSource = NpgsqlDataSource.Create(connectionString);
                await using var cmd = dataSource.CreateCommand("DELETE FROM students WHERE id = @Id");
                cmd.Parameters.AddWithValue("Id", id);
                await cmd.ExecuteNonQueryAsync();

                Console.WriteLine("Student deleted successfully.");
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
    }
}

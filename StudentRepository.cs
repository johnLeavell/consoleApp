using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        // Additional CRUD methods (Update, Delete, Get, etc.) can be added here
    }
}

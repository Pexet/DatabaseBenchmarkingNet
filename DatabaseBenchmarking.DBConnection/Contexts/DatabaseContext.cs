using DatabaseBenchmarking.DBConnection.Entities;
using DatabaseBenchmarking.DBConnection.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseBenchmarking.DBConnection.Contexts
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Event> Events { get; set; }

        public DbSet<Schedule> Schedules { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<StudentInserted> StudentsInserted { get; set; }

        public DbSet<Theme> Themes { get; set; }

        public DbSet<EventStyle> EventsStyles { get; set; }

        public DbSet<Link> Links { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Random random = new Random();
            List<StudentInserted> seedStudents = new List<StudentInserted>();
            for (int i = 0; i < 10000; ++i)
            {
                string personEmail = BenchmarkingHelper.GetEmail();
                string personPassword = BenchmarkingHelper.GenerateRandomString(10);
                string personNumber = BenchmarkingHelper.GenerateRandomString(10);
                bool? isNotifiable = random.NextDouble() > 0.5;
                seedStudents.Add(new StudentInserted()
                {
                    Id = i + 1,
                    EmailAddress = personEmail!,
                    Password = personPassword!,
                    IsNotifiable = isNotifiable!.Value,
                    PhoneNumber = personNumber!,
                });
            }
            modelBuilder.Entity<StudentInserted>()
                .HasData(seedStudents);

        }
    }
}

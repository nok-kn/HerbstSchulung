using FluentAssertions;
using HerbstSchulung.EntityFramework.DataModel;
using Microsoft.EntityFrameworkCore;
using Xunit;


namespace HerbstSchulung.EntityFramework.Tests
{
    // nach jedem Test wird die schnell DB zurückgesetzt (mit dem DbFixture)
    
    [Collection("db")]
    public class IsolierteDbTests :  IAsyncLifetime
    {
        private readonly DbFixture _dbFixture;
        
        public IsolierteDbTests(DbFixture dbFicture)
        {
            _dbFixture = dbFicture;
        }
        
        [Fact]
        public async Task AppDbContext_Can_Add_And_Retrieve_Student()
        {
            // Arrange
            using var sut = Arrange.CreateDbContext(false);
            var student = new Student
            {
                Id = "STU-001",
                Name = "Max Mustermann",
                Nationality = "DE",
                School = "Test Schule"
            };

            // Act
            await sut.MergeAsync(student);
            await sut.SaveChangesAsync();

            // Assert
            var actual = await sut.Students.FirstOrDefaultAsync(s => s.Id == student.Id);
            actual.Should().NotBeNull();
            actual!.Name.Should().Be(student.Name);
            actual.School.Should().Be(student.School);
            actual.Nationality.Should().Be(student.Nationality);
            actual.Art.Should().Be(PersonArt.Student);
        }

        [Fact]
        public async Task AppDbContext_Can_Add_And_Retrieve_Teacher()
        {
            // Arrange
            using var sut = Arrange.CreateDbContext(false);
            var teacher = new Teacher
            {
                Id = "TCH-001",
                Name = "Anna Schmidt",
                Nationality = "AT",
                Subject = "Mathematik"
            };

            // Act
            await sut.MergeAsync(teacher);
            await sut.SaveChangesAsync();

            // Assert
            var actual = await sut.Teachers.FirstOrDefaultAsync(t => t.Id == teacher.Id);
            actual.Should().NotBeNull();
            actual!.Name.Should().Be(teacher.Name);
            actual.Subject.Should().Be(teacher.Subject);
            actual.Nationality.Should().Be(teacher.Nationality);
            actual.Art.Should().Be(PersonArt.Teacher);
        }

        // Wird vor jedem Test ausgeführt
        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
        
        // Wird nach jedem Test ausgeführt
        public Task DisposeAsync()
        {
            return _dbFixture.ResetAsync();
        }
    }
}

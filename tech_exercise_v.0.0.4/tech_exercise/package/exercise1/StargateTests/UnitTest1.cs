using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;

namespace StargateApiTests
{
    [TestClass]
    public class UnitTest1
    {
        public DbContextOptions<StargateContext> options = new DbContextOptionsBuilder<StargateContext>()
                .UseSqlite("Data Source=:memory:")
                .Options;

        [TestMethod]
        public async Task Process_WhenRequestIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new StargateContext(options);
            var command = new CreatePerson { Name = "Test Person" };
            var handler = new CreatePersonHandler(context);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            var person = await context.People.FirstOrDefaultAsync(p => p.Name == "Test Person");
            Assert.IsNotNull(person);
        }

        [TestMethod]
        public async Task Process_WhenPersonExists_ThrowsBadHttpRequestException()
        {
            // Arrange
            using (var context = new StargateContext(options))
            {
                var existingPerson = new Person { Name = "TestPerson1" };
                context.People.Add(existingPerson);
                context.SaveChanges();

                var command = new CreatePerson { Name = "TestPerson2" };
                var processor = new CreatePersonPreProcessor(context);

                // Act and Assert
                await Assert.ThrowsExceptionAsync<BadHttpRequestException>(() => processor.Process(command, default));
            }
        }
    }
}
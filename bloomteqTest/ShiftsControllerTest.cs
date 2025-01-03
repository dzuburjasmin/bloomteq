using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using bloomteq.Models;
using InformationProtocolSubSystem.Api.Controllers.odata;
using bloomteq;
using Microsoft.AspNetCore.OData.Results;

namespace bloomteqTest
{
    public class ShiftsControllerTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly ShiftsController _controller;
        private readonly Guid _testUserId = Guid.NewGuid();

        public ShiftsControllerTests()
        {
            _mockContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            var claims = new List<Claim>
        {
            new Claim("userId", _testUserId.ToString())
        };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = user };
            _mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);

            _controller = new ShiftsController(_mockContext.Object, _mockHttpContextAccessor.Object);
        }

        [Fact]
        public void Get_ReturnsShiftsForUser()
        {
            // Arrange
            var shifts = new List<Shift>
        {
            new Shift { Id = Guid.NewGuid(), UserId = _testUserId.ToString() },
            new Shift { Id = Guid.NewGuid(), UserId = _testUserId.ToString() }
        }.AsQueryable();

            var mockSet = new Mock<DbSet<Shift>>();
            mockSet.As<IQueryable<Shift>>().Setup(m => m.Provider).Returns(shifts.Provider);
            mockSet.As<IQueryable<Shift>>().Setup(m => m.Expression).Returns(shifts.Expression);
            mockSet.As<IQueryable<Shift>>().Setup(m => m.ElementType).Returns(shifts.ElementType);
            mockSet.As<IQueryable<Shift>>().Setup(m => m.GetEnumerator()).Returns(shifts.GetEnumerator());

            _mockContext.Setup(c => c.Shifts).Returns(mockSet.Object);

            // Act
            var result = _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedShifts = Assert.IsAssignableFrom<IEnumerable<Shift>>(okResult.Value);
            Assert.Equal(2, returnedShifts.Count());
        }

        [Fact]
        public void Post_AddsNewShift()
        {
            var shift = new Shift {Description = "Test", Date = DateTime.Now, Hours = 4};
            _mockContext.Setup(c => c.Shifts.Add(It.IsAny<Shift>())).Verifiable();
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            var result = _controller.Post(shift);

            var createdResult = Assert.IsType<CreatedODataResult<Shift>>(result);
            Assert.Equal(shift, createdResult.Entity);
            _mockContext.Verify(c => c.Shifts.Add(It.IsAny<Shift>()), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Patch_UpdatesExistingShift()
        {
            // Arrange
            var existingShift = new Shift { Id = Guid.NewGuid(), UserId = _testUserId.ToString(), Description = "Old Desc" };
            var delta = new Delta<Shift>();
            delta.TrySetPropertyValue("Description", "New Desc");

            var mockSet = new Mock<DbSet<Shift>>();
            mockSet.Setup(m => m.Find(existingShift.Id)).Returns(existingShift);
            _mockContext.Setup(c => c.Shifts).Returns(mockSet.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _controller.Patch(existingShift.Id, delta);

            // Assert
            var updatedResult = Assert.IsType<UpdatedODataResult<Shift>>(result);
            Assert.Equal("New Desc", updatedResult.Entity.Description);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Delete_RemovesShift()
        {
            // Arrange
            var existingShift = new Shift { Id = Guid.NewGuid(), UserId = _testUserId.ToString() };

            var mockSet = new Mock<DbSet<Shift>>();
            mockSet.Setup(m => m.Find(existingShift.Id)).Returns(existingShift);
            _mockContext.Setup(c => c.Shifts).Returns(mockSet.Object);
            _mockContext.Setup(c => c.Remove(existingShift)).Verifiable();
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _controller.Delete(existingShift.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockContext.Verify(c => c.Remove(existingShift), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }
    }

}
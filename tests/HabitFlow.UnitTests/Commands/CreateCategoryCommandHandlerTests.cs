using System;
using AutoMapper;
using HabitFlow.Application.Features.Categories.Commands.CreateCategory;
using HabitFlow.Application.Features.Categories.Commands.CreateCategory.Dtos;
using HabitFlow.Domain.Entities;
using HabitFlow.Domain.Repositories;
using Moq;

namespace HabitFlow.UnitTests.Commands;

public class CreateCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateCategoryCommandHandler(
            _categoryRepositoryMock.Object,
            _userRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnCreatedCategoryDto_When_InputIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryDto = new CreateCategoryDto
        {
            Name = "Health",
            Description = "Health related habits",
            Color = "#4CAF50"
        };

        var command = new CreateCategoryCommand(userId, categoryDto);

        _userRepositoryMock.Setup(x => x.ExistsAsync(userId))
            .ReturnsAsync(true);

        _mapperMock.Setup(x => x.Map<CreatedCategoryDto>(It.IsAny<Category>()))
            .Returns(new CreatedCategoryDto
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                Color = categoryDto.Color
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryDto.Name, result.Name);
        Assert.Equal(categoryDto.Description, result.Description);
        Assert.Equal(categoryDto.Color, result.Color);

        _categoryRepositoryMock.Verify(x => x.AddAsync(It.Is<Category>(c =>
            c.UserId == userId &&
            c.Name == categoryDto.Name &&
            c.Description == categoryDto.Description &&
            c.Color == categoryDto.Color)), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_UserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryDto = new CreateCategoryDto { Name = "Test" };

        var command = new CreateCategoryCommand(userId, categoryDto);

        _userRepositoryMock.Setup(x => x.ExistsAsync(userId))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}


using System;
using AutoMapper;
using HabitFlow.Application.Features.Categories.Queries.GetCategories.Dtos;
using HabitFlow.Application.Features.Categories.Queries.GetCategoryById;
using HabitFlow.Domain.Entities;
using HabitFlow.Domain.Repositories;
using Moq;

namespace HabitFlow.UnitTests.Commands;

public class GetCategoryByIdQueryHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetCategoryByIdQueryHandler _handler;

    public GetCategoryByIdQueryHandlerTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetCategoryByIdQueryHandler(
            _categoryRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnCategory_When_ItExistsAndBelongsToUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category(userId, "Test", "Desc", "#FFF");

        var query = new GetCategoryByIdQuery(userId, categoryId);

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        _mapperMock.Setup(x => x.Map<CategoryDto>(category))
            .Returns(new CategoryDto { Name = category.Name });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(category.Name, result.Name);
        _categoryRepositoryMock.Verify(x => x.GetByIdAsync(categoryId), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_CategoryNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var query = new GetCategoryByIdQuery(userId, categoryId);

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync((Category)null);

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_CategoryDoesNotBelongToUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category(otherUserId, "Test", null, null);

        var query = new GetCategoryByIdQuery(userId, categoryId);

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }
}

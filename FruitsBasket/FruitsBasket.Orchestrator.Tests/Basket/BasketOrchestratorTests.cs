using FluentAssertions;
using FruitsBasket.Model.Basket;
using FruitsBasket.Orchestrator.Basket;
using FruitsBasket.Orchestrator.Exceptions;
using Moq;

namespace FruitsBasket.Orchestrator.Tests.Basket;

public class BasketOrchestratorTests
{
    private readonly IBasketOrchestrator _orchestrator;
    private readonly Mock<IBasketRepository> _repositoryMock = new();

    public BasketOrchestratorTests()
    {
        _orchestrator = new BasketOrchestrator(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEntityWithGivenId_IfExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expected = new BasketDto()
        {
            Id = id,
            Name = "Basket",
            FruitsWeight = 1.1m,
            LastFruitAdded = new DateTime(2025, 01, 01),
        };

        _repositoryMock
            .Setup(rm => rm.GetByIdAsync(id))
            .ReturnsAsync(expected);

        // Act
        var actual = await _orchestrator.GetByIdAsync(id);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsException_IfNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var act = async () => await _orchestrator.GetByIdAsync(id);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Basket not found");
    }

    [Fact]
    public async Task GetAllAsync_Works()
    {
        // Arrange
        const int pageNumber = 1;
        const int pageSize = 10;
        var expected = new List<BasketDto> { new(), new() };

        _repositoryMock
            .Setup(rm => rm.GetAllAsync(pageNumber, pageSize))
            .ReturnsAsync(expected);

        // Act
        var actual = await _orchestrator.GetAllAsync(1, 10);

        // Assert
        _repositoryMock.Verify(rm => rm.GetAllAsync(pageNumber, pageSize), Times.Once);

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateAsync_Works()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expected = new BasketDto()
        {
            Id = id,
            Name = "Basket",
            FruitsWeight = 1.1m,
            LastFruitAdded = new DateTime(2025, 01, 01),
        };

        _repositoryMock
            .Setup(rm => rm.CreateAsync(expected))
            .ReturnsAsync(expected);

        // Act
        var actual = await _orchestrator.CreateAsync(expected);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateAsync_Works()
    {
        // Arrange
        var id = Guid.NewGuid();
        var basket = new BasketDto()
        {
            Id = id,
            Name = "Fruit",
            FruitsWeight = 1.1m,
            LastFruitAdded = new DateTime(2025, 01, 01),
        };

        _repositoryMock
            .Setup(rm => rm.GetByIdAsync(id))
            .ReturnsAsync(basket);

        // Act
        await _orchestrator.UpdateAsync(basket);

        // Assert
        _repositoryMock.Verify(rm => rm.UpdateAsync(basket), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsException_IfNotFound()
    {
        // Arrange
        _repositoryMock
            .Setup(rm => rm.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => null);

        // Act
        var act = async () => await _orchestrator.UpdateAsync(new BasketDto());

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Basket not found");
    }

    [Fact]
    public async Task DeleteAsync_Works()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock
            .Setup(rm => rm.GetByIdAsync(id))
            .ReturnsAsync(new BasketDto());

        // Act
        await _orchestrator.DeleteAsync(id);

        // Assert
        _repositoryMock.Verify(rm => rm.DeleteAsync(It.IsAny<BasketDto>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsException_IfNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var act = async () => await _orchestrator.DeleteAsync(id);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Basket not found");
    }
}
using FluentAssertions;
using FruitsBasket.Model.Fruit;
using FruitsBasket.Orchestrator.Exceptions;
using FruitsBasket.Orchestrator.Fruit;
using Moq;

namespace FruitsBasket.Orchestrator.Tests.Fruit;

public class FruitOrchestratorTests
{
    private readonly IFruitOrchestrator _orchestrator;
    private readonly Mock<IFruitRepository> _repositoryMock = new();

    public FruitOrchestratorTests()
    {
        _orchestrator = new FruitOrchestrator(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsFruitWithGivenId_IfExists()
    {
        // Arrange
        const int id = 1;
        var expected = new FruitDto
        {
            Id = id,
            Name = "Fruit",
            Weight = 1.1m,
            HarvestDate = new DateTime(2025, 01, 01),
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
        const int id = 1;

        // Act
        var act = async () => await _orchestrator.GetByIdAsync(id);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Fruit not found");
    }

    [Fact]
    public async Task GetAllAsync_Works()
    {
        // Arrange
        const int pageNumber = 1;
        const int pageSize = 10;
        var expected = new List<FruitDto> { new(), new() };

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
        const int id = 1;
        var expected = new FruitDto
        {
            Id = id,
            Name = "Fruit",
            Weight = 1.1m,
            HarvestDate = new DateTime(2025, 01, 01),
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
        const int id = 1;
        var fruit = new FruitDto
        {
            Id = id,
            Name = "Fruit",
            Weight = 1.1m,
            HarvestDate = new DateTime(2025, 01, 01),
        };

        _repositoryMock
            .Setup(rm => rm.GetByIdAsync(id))
            .ReturnsAsync(fruit);

        // Act
        await _orchestrator.UpdateAsync(fruit);

        // Assert
        _repositoryMock.Verify(rm => rm.UpdateAsync(fruit), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsException_IfNotFound()
    {
        // Arrange
        _repositoryMock
            .Setup(rm => rm.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(() => null);

        // Act
        var act = async () => await _orchestrator.UpdateAsync(new FruitDto());

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Fruit not found");
    }

    [Fact]
    public async Task DeleteAsync_Works()
    {
        // Arrange
        const int id = 1;
        _repositoryMock
            .Setup(rm => rm.GetByIdAsync(id))
            .ReturnsAsync(new FruitDto());

        // Act
        await _orchestrator.DeleteAsync(id);

        // Assert
        _repositoryMock.Verify(rm => rm.DeleteAsync(It.IsAny<FruitDto>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsException_IfNotFound()
    {
        // Arrange
        const int id = 1;

        // Act
        var act = async () => await _orchestrator.DeleteAsync(id);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Fruit not found");
    }
}
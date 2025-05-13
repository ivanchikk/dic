using FluentAssertions;
using FruitsBasket.Model.Basket;
using FruitsBasket.Model.Fruit;
using FruitsBasket.Model.FruitBasket;
using FruitsBasket.Orchestrator.BlobStorage;
using FruitsBasket.Orchestrator.Exceptions;
using FruitsBasket.Orchestrator.FruitBasket;
using Moq;

namespace FruitsBasket.Orchestrator.Tests.FruitBasket;

public class FruitBasketOrchestratorTests
{
    private readonly IFruitBasketOrchestrator _orchestrator;
    private readonly Mock<IFruitOrchestrator> _fruitOrchestratorMock = new();
    private readonly Mock<IBasketOrchestrator> _basketOrchestratorMock = new();
    private readonly Mock<IBlobStorage> _blobStorageMock = new();

    public FruitBasketOrchestratorTests()
    {
        _orchestrator = new FruitBasketOrchestrator(
            _basketOrchestratorMock.Object,
            _fruitOrchestratorMock.Object,
            _blobStorageMock.Object
        );
    }

    [Fact]
    public async Task GetAllBasketsAsync_Works()
    {
        // Arrange
        var expected = new List<Guid> { Guid.Empty, Guid.Empty };

        _blobStorageMock
            .Setup(bsm => bsm.GetAllBasketsAsync())
            .ReturnsAsync(expected);

        // Act
        var actual = await _orchestrator.GetAllBasketsAsync();

        // Assert
        actual.Should().BeEquivalentTo(expected);

        _blobStorageMock.Verify(bsm => bsm.GetAllBasketsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllFruitsAsync_Works()
    {
        // Arrange
        var expected = new List<int> { 0, 0 };

        _blobStorageMock
            .Setup(bsm => bsm.GetAllFruitsAsync())
            .ReturnsAsync(expected);

        // Act
        var actual = await _orchestrator.GetAllFruitsAsync();

        // Assert
        actual.Should().BeEquivalentTo(expected);

        _blobStorageMock.Verify(bsm => bsm.GetAllFruitsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllFruitsByBasketIdAsync_ReturnsFruits_IfBasketExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expected = new List<int> { 0, 0 };

        _basketOrchestratorMock
            .Setup(bom => bom.GetByIdAsync(id))
            .ReturnsAsync(new BasketDto());

        _blobStorageMock
            .Setup(bsm => bsm.GetAllFruitsByBasketIdAsync(id))
            .ReturnsAsync(expected);

        // Act
        var actual = await _orchestrator.GetAllFruitsByBasketIdAsync(id);

        // Assert
        actual.Should().BeEquivalentTo(expected);

        _basketOrchestratorMock.Verify(bom => bom.GetByIdAsync(id), Times.Once);
        _blobStorageMock.Verify(bsm => bsm.GetAllFruitsByBasketIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetAllFruitsByBasketIdAsync_ThrowsException_IfBasketNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        _basketOrchestratorMock
            .Setup(bom => bom.GetByIdAsync(id))
            .ThrowsAsync(new NotFoundException("Basket not found"));

        // Act
        var act = async () => await _orchestrator.GetAllFruitsByBasketIdAsync(id);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Basket not found");

        _basketOrchestratorMock.Verify(bom => bom.GetByIdAsync(id), Times.Once);
        _blobStorageMock.Verify(bsm => bsm.GetAllFruitsByBasketIdAsync(id), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_Works()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        const int fruitId = 1;
        var fileName = $"{basketId:N}_{fruitId}";
        var expected = new FruitBasketDto
        {
            BasketId = basketId,
            FruitId = fruitId,
        };

        _fruitOrchestratorMock
            .Setup(fom => fom.GetByIdAsync(fruitId))
            .ReturnsAsync(new FruitDto());
        _basketOrchestratorMock
            .Setup(bom => bom.GetByIdAsync(basketId))
            .ReturnsAsync(new BasketDto());
        _blobStorageMock
            .Setup(bsm => bsm.ContainsFileAsync(fileName))
            .ReturnsAsync(false);
        _blobStorageMock
            .Setup(bsm => bsm.GetAllFruitsAsync())
            .ReturnsAsync([]);

        // Act
        var actual = await _orchestrator.CreateAsync(basketId, fruitId);

        // Assert
        actual.Should().BeEquivalentTo(expected);

        _fruitOrchestratorMock.Verify(fom => fom.GetByIdAsync(fruitId), Times.Once);
        _basketOrchestratorMock.Verify(bom => bom.GetByIdAsync(basketId), Times.Once);
        _blobStorageMock.Verify(bsm => bsm.ContainsFileAsync(fileName), Times.Once);
        _blobStorageMock.Verify(bsm => bsm.GetAllFruitsAsync(), Times.Once);
        _blobStorageMock.Verify(bsm => bsm.CreateFileAsync(fileName), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ThrowsException_IfFruitNotFound()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        const int fruitId = 1;
        var fileName = $"{basketId:N}_{fruitId}";

        _fruitOrchestratorMock
            .Setup(fom => fom.GetByIdAsync(fruitId))
            .ThrowsAsync(new NotFoundException("Fruit not found"));

        // Act
        var act = async () => await _orchestrator.CreateAsync(basketId, fruitId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Fruit not found");

        _fruitOrchestratorMock.Verify(fom => fom.GetByIdAsync(fruitId), Times.Once);
        _blobStorageMock.Verify(bsm => bsm.ContainsFileAsync(fileName), Times.Never);
        _blobStorageMock.Verify(bsm => bsm.GetAllFruitsAsync(), Times.Never);
        _blobStorageMock.Verify(bsm => bsm.CreateFileAsync(fileName), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ThrowsException_IfBasketNotFound()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        const int fruitId = 1;
        var fileName = $"{basketId:N}_{fruitId}";

        _basketOrchestratorMock
            .Setup(bom => bom.GetByIdAsync(basketId))
            .ThrowsAsync(new NotFoundException("Basket not found"));

        // Act
        var act = async () => await _orchestrator.CreateAsync(basketId, fruitId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Basket not found");

        _basketOrchestratorMock.Verify(bom => bom.GetByIdAsync(basketId), Times.Once);
        _blobStorageMock.Verify(bsm => bsm.ContainsFileAsync(fileName), Times.Never);
        _blobStorageMock.Verify(bsm => bsm.GetAllFruitsAsync(), Times.Never);
        _blobStorageMock.Verify(bsm => bsm.CreateFileAsync(fileName), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ThrowsException_IfFruitInBasket()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        const int fruitId = 1;
        var fileName = $"{basketId:N}_{fruitId}";

        _fruitOrchestratorMock
            .Setup(fom => fom.GetByIdAsync(fruitId))
            .ReturnsAsync(new FruitDto());
        _basketOrchestratorMock
            .Setup(bom => bom.GetByIdAsync(basketId))
            .ReturnsAsync(new BasketDto());
        _blobStorageMock
            .Setup(bsm => bsm.ContainsFileAsync(fileName))
            .ReturnsAsync(true);
        _blobStorageMock
            .Setup(bsm => bsm.GetAllFruitsAsync())
            .ReturnsAsync([]);

        // Act
        var act = async () => await _orchestrator.CreateAsync(basketId, fruitId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("This fruit is already in a basket");

        _fruitOrchestratorMock.Verify(fom => fom.GetByIdAsync(fruitId), Times.Once);
        _basketOrchestratorMock.Verify(bom => bom.GetByIdAsync(basketId), Times.Once);
        _blobStorageMock.Verify(bsm => bsm.ContainsFileAsync(fileName), Times.Once);
        _blobStorageMock.Verify(bsm => bsm.CreateFileAsync(fileName), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ThrowsException_IfFruitInAnotherBasket()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        const int fruitId = 1;
        var fileName = $"{basketId:N}_{fruitId}";

        _fruitOrchestratorMock
            .Setup(fom => fom.GetByIdAsync(fruitId))
            .ReturnsAsync(new FruitDto());
        _basketOrchestratorMock
            .Setup(bom => bom.GetByIdAsync(basketId))
            .ReturnsAsync(new BasketDto());
        _blobStorageMock
            .Setup(bsm => bsm.ContainsFileAsync(fileName))
            .ReturnsAsync(false);
        _blobStorageMock
            .Setup(bsm => bsm.GetAllFruitsAsync())
            .ReturnsAsync([fruitId]);

        // Act
        var act = async () => await _orchestrator.CreateAsync(basketId, fruitId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("This fruit is already in another basket");

        _fruitOrchestratorMock.Verify(fom => fom.GetByIdAsync(fruitId), Times.Once);
        _basketOrchestratorMock.Verify(bom => bom.GetByIdAsync(basketId), Times.Once);
        _blobStorageMock.Verify(bsm => bsm.GetAllFruitsAsync(), Times.Once);
        _blobStorageMock.Verify(bsm => bsm.CreateFileAsync(fileName), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_Works()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        const int fruitId = 1;
        var fileName = $"{basketId:N}_{fruitId}";
        var expected = new FruitBasketDto
        {
            BasketId = basketId,
            FruitId = fruitId,
        };

        _blobStorageMock
            .Setup(bsm => bsm.ContainsFileAsync(fileName))
            .ReturnsAsync(true);
        _blobStorageMock
            .Setup(bsm => bsm.DeleteFileAsync(fileName))
            .ReturnsAsync(expected);

        // Act
        var actual = await _orchestrator.DeleteAsync(basketId, fruitId);

        // Assert
        actual.Should().BeEquivalentTo(expected);

        _blobStorageMock.Verify(bsm => bsm.ContainsFileAsync(fileName), Times.Once);
        _blobStorageMock.Verify(bsm => bsm.DeleteFileAsync(fileName), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsException_IfNotFound()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        const int fruitId = 1;
        var fileName = $"{basketId:N}_{fruitId}";

        _blobStorageMock
            .Setup(bsm => bsm.ContainsFileAsync(fileName))
            .ReturnsAsync(false);

        // Act
        var act = async () => await _orchestrator.DeleteAsync(basketId, fruitId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("File not found");

        _blobStorageMock.Verify(bsm => bsm.ContainsFileAsync(fileName), Times.Once);
        _blobStorageMock.Verify(bsm => bsm.DeleteFileAsync(fileName), Times.Never);
    }
}
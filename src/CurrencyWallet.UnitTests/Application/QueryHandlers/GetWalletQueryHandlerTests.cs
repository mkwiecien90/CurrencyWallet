#region Usings

using AutoMapper;
using CurrencyWallet.Application.DTOs;
using CurrencyWallet.Application.Interfaces;
using CurrencyWallet.Application.Queries.Handlers;
using CurrencyWallet.Application.Queries;
using CurrencyWallet.Domain.Entities;
using CurrencyWallet.Domain.Exceptions;
using Moq;
using FluentAssertions;

#endregion

namespace CurrencyWallet.UnitTests.Application.QueryHandlers
{
    public class GetWalletQueryHandlerTests
    {
        private readonly Mock<IWalletRepository> _walletRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetWalletQueryHandler _handler;

        public GetWalletQueryHandlerTests()
        {
            _walletRepositoryMock = new Mock<IWalletRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetWalletQueryHandler(_walletRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnWalletViewModel_WhenWalletExists()
        {
            // Arrange
            var walletId = Guid.NewGuid();
            var wallet = new Wallet { Id = walletId, Name = "Test Wallet" };
            var walletViewModel = new WalletViewModel { Id = walletId, Name = "Test Wallet" };

            _walletRepositoryMock
                .Setup(repo => repo.GetByIdAsync(walletId))
                .ReturnsAsync(wallet);

            _mapperMock
                .Setup(mapper => mapper.Map<WalletViewModel>(wallet))
                .Returns(walletViewModel);

            var query = new GetWalletQuery(walletId);
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(walletViewModel);
            _walletRepositoryMock.Verify(repo => repo.GetByIdAsync(walletId), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<WalletViewModel>(wallet), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenWalletDoesNotExist()
        {
            // Arrange
            var walletId = Guid.NewGuid();

            _walletRepositoryMock
                .Setup(repo => repo.GetByIdAsync(walletId))
                .ReturnsAsync((Wallet)null);

            var query = new GetWalletQuery(walletId);
            var cancellationToken = new CancellationToken();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, cancellationToken));
            Assert.Equal($"Wallet with ID {walletId} not found.", exception.Message);
            _walletRepositoryMock.Verify(repo => repo.GetByIdAsync(walletId), Times.Once);
        }
    }
}

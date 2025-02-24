#region Usings

using AutoMapper;
using CurrencyWallet.Application.DTOs;
using CurrencyWallet.Application.Interfaces;
using CurrencyWallet.Application.Queries.Handlers;
using CurrencyWallet.Application.Queries;
using CurrencyWallet.Domain.Entities;
using Moq;
using FluentAssertions;
using CurrencyWallet.Domain.ValueObjects;

#endregion

namespace CurrencyWallet.UnitTests.Application.QueryHandlers
{
    public class GetAllWalletsQueryHandlerTests
    {
        private readonly Mock<IWalletRepository> _walletRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAllWalletsQueryHandler _handler;

        public GetAllWalletsQueryHandlerTests()
        {
            _walletRepositoryMock = new Mock<IWalletRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetAllWalletsQueryHandler(_walletRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnWallets_WhenWalletsExist()
        {
            // Arrange
            var wallets = new List<Wallet>
            {
                new Wallet { Id = Guid.NewGuid(), Name = "Wallet 1" },
                new Wallet { Id = Guid.NewGuid(), Name = "Wallet 2" }
            };
            var walletViewModels = new List<WalletViewModel>
            {
                new WalletViewModel { Id = wallets[0].Id, Name = wallets[0].Name, Balances = new List<WalletBalance>() { new WalletBalance(new Money(100m, "JOD")) } },
                new WalletViewModel { Id = wallets[1].Id, Name = wallets[1].Name }
            };

            _walletRepositoryMock
                .Setup(repo => repo.GetAll())
                .ReturnsAsync(wallets);

            _mapperMock
                .Setup(mapper => mapper.Map<List<WalletViewModel>>(wallets))
                .Returns(walletViewModels);

            var query = new GetAllWalletsQuery();
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(walletViewModels);
            result.First().Balances.Should().HaveCount(1);
            _walletRepositoryMock.Verify(repo => repo.GetAll(), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<List<WalletViewModel>>(wallets), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoWalletsExist()
        {
            // Arrange
            var wallets = new List<Wallet>();
            var walletViewModels = new List<WalletViewModel>();

            _walletRepositoryMock
                .Setup(repo => repo.GetAll())
                .ReturnsAsync(wallets);

            _mapperMock
                .Setup(mapper => mapper.Map<List<WalletViewModel>>(wallets))
                .Returns(walletViewModels);

            var query = new GetAllWalletsQuery();
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _walletRepositoryMock.Verify(repo => repo.GetAll(), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<List<WalletViewModel>>(wallets), Times.Once);
        }
    }
}

#region Usings

using CurrencyWallet.Application.Commands.Handlers;
using CurrencyWallet.Application.Commands;
using CurrencyWallet.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using CurrencyWallet.Domain.Entities;
using FluentAssertions;

#endregion

namespace CurrencyWallet.UnitTests.Application.Handlers
{
    public class CreateWalletCommandHandlerTests
    {
        private readonly Mock<IWalletRepository> _walletRepositoryMock;
        private readonly Mock<ILogger<CreateWalletCommandHandler>> _loggerMock;
        private readonly CreateWalletCommandHandler _handler;

        public CreateWalletCommandHandlerTests()
        {
            _walletRepositoryMock = new Mock<IWalletRepository>();
            _loggerMock = new Mock<ILogger<CreateWalletCommandHandler>>();
            _handler = new CreateWalletCommandHandler(_walletRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateWallet_AndReturnGuid()
        {
            // Arrange
            var command = new CreateWalletCommand("Test Wallet");
            var cancellationToken = new CancellationToken();

            _walletRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Wallet>()))
                .Returns(Task.FromResult(Guid.NewGuid()));

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.Should().NotBeEmpty();
            _walletRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Wallet>(w =>
                w.Name == command.Name &&
                w.Balances != null &&
                w.Balances.Count == 0
            )), Times.Once);

            _loggerMock.Verify(
                log => log.Log(
                    It.Is<LogLevel>(level => level == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Wallet {command.Name} created.")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }
    }
}

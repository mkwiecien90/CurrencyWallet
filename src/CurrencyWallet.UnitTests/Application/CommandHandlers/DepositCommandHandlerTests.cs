#region Usings

using AutoMapper;
using CurrencyWallet.Application.Commands.Handlers;
using CurrencyWallet.Application.Commands;
using CurrencyWallet.Application.Interfaces;
using CurrencyWallet.Domain.Entities;
using CurrencyWallet.Domain.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

#endregion

namespace CurrencyWallet.UnitTests.Application.Handlers
{
    public class DepositCommandHandlerTests
    {
        private readonly Mock<IWalletRepository> _walletRepositoryMock;
        private readonly Mock<ILogger<DepositCommandHandler>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly DepositCommandHandler _handler;

        public DepositCommandHandlerTests()
        {
            _walletRepositoryMock = new Mock<IWalletRepository>();
            _loggerMock = new Mock<ILogger<DepositCommandHandler>>();
            _mapperMock = new Mock<IMapper>();
            _handler = new DepositCommandHandler(_walletRepositoryMock.Object, _loggerMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldDeposit_WhenWalletExists()
        {
            // Arrange
            var wallet = new Wallet { Id = Guid.NewGuid(), Name = "Test Wallet" };
            var command = new DepositCommand(wallet.Id, "JOD", 100m );
            var cancellationToken = new CancellationToken();

            _walletRepositoryMock
                .Setup(repo => repo.GetByIdAsync(command.WalletId))
                .ReturnsAsync(wallet);

            _walletRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Wallet>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.Should().BeTrue();
            wallet.Balances.Should().Contain(b => b.Money.Currency == "JOD" && b.Money.Amount == 100m);

            _walletRepositoryMock.Verify(repo => repo.GetByIdAsync(command.WalletId), Times.Once);
            _walletRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Wallet>(w => w.Id == wallet.Id)), Times.Once);

            // Weryfikacja logowania
            _loggerMock.Verify(
                log => log.Log(
                    It.Is<LogLevel>(level => level == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Deposit to wallet {wallet.Name} value: {command.Amount} {command.Currency}.")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenWalletDoesNotExist()
        {
            // Arrange
            var command = new DepositCommand(Guid.NewGuid(), "JOD", 100m );
            var cancellationToken = new CancellationToken();

            _walletRepositoryMock
                .Setup(repo => repo.GetByIdAsync(command.WalletId))
                .ReturnsAsync((Wallet)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, cancellationToken);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Wallet with ID {command.WalletId} not found.");

            _walletRepositoryMock.Verify(repo => repo.GetByIdAsync(command.WalletId), Times.Once);
            _walletRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Wallet>()), Times.Never);
            _loggerMock.VerifyNoOtherCalls();
        }
    }
}

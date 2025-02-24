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
using CurrencyWallet.Domain.ValueObjects;

#endregion

namespace CurrencyWallet.UnitTests.Application.Handlers
{
    public class ConvertCurrencyCommandHandlerTests
    {
        private readonly Mock<IWalletRepository> _walletRepositoryMock;
        private readonly Mock<ICurrencyRateRepository> _currencyRateRepositoryMock;
        private readonly Mock<ILogger<ConvertCurrencyCommandHandler>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ConvertCurrencyCommandHandler _handler;

        public ConvertCurrencyCommandHandlerTests()
        {
            _walletRepositoryMock = new Mock<IWalletRepository>();
            _currencyRateRepositoryMock = new Mock<ICurrencyRateRepository>();
            _loggerMock = new Mock<ILogger<ConvertCurrencyCommandHandler>>();
            _mapperMock = new Mock<IMapper>();
            _handler = new ConvertCurrencyCommandHandler(
                _walletRepositoryMock.Object,
                _currencyRateRepositoryMock.Object,
                _loggerMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldConvertCurrency_WhenWalletExistsAndRatesAreAvailable()
        {
            // Arrange
            var wallet = new Wallet
            {
                Id = Guid.NewGuid(),
                Name = "Test Wallet",
                Balances = new List<WalletBalance>
                {
                    new WalletBalance(new Money(200m, "JOD"))
                }
            };

            var currencyRates = new List<CurrencyRate>
            {
                new CurrencyRate { Code = "JOD", Mid = 1.0m },
                new CurrencyRate { Code = "GMD", Mid = 1.2m }
            };

            var command = new ConvertCurrencyCommand(wallet.Id, "JOD", "GMD", 100m);
            var cancellationToken = new CancellationToken();

            _walletRepositoryMock
                .Setup(repo => repo.GetByIdAsync(command.WalletId))
                .ReturnsAsync(wallet);

            _currencyRateRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(currencyRates);

            _walletRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Wallet>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.Should().BeTrue();
            wallet.Balances.Should().Contain(b => b.Money.Currency == "JOD" && b.Money.Amount == 100m);  // 200m - 100m = 100m
            wallet.Balances.Should().Contain(b => b.Money.Currency == "GMD" && b.Money.Amount == 120m);  // 100m / 1.0 * 1.2 = 120m

            _walletRepositoryMock.Verify(repo => repo.GetByIdAsync(command.WalletId), Times.Once);
            _currencyRateRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
            _walletRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Wallet>(w => w.Id == wallet.Id)), Times.Once);

            // Weryfikacja logowania
            _loggerMock.Verify(
                log => log.Log(
                    It.Is<LogLevel>(level => level == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Converted on wallet {wallet.Name}: {command.Amount} {command.FromCurrency} to currency {command.ToCurrency}.")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenWalletDoesNotExist()
        {
            // Arrange
            var command = new ConvertCurrencyCommand(Guid.NewGuid(), "JOD", "GMD", 100m);
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
            _currencyRateRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Never);
            _loggerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenCurrencyRatesAreNotAvailable()
        {
            // Arrange
            var wallet = new Wallet
            {
                Id = Guid.NewGuid(),
                Name = "Test Wallet",
                Balances = new List<WalletBalance>
                {
                    new WalletBalance(new Money(200m, "JOD"))
                }
            };

            var command = new ConvertCurrencyCommand(wallet.Id, "JOD", "GMD", 100m);
            var cancellationToken = new CancellationToken();

            _walletRepositoryMock
                .Setup(repo => repo.GetByIdAsync(command.WalletId))
                .ReturnsAsync(wallet);

            _currencyRateRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync((List<CurrencyRate>)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, cancellationToken);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Can not find currency rates.");

            _walletRepositoryMock.Verify(repo => repo.GetByIdAsync(command.WalletId), Times.Once);
            _currencyRateRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
            _walletRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Wallet>()), Times.Never);
            _loggerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_ShouldThrowBadRequestException_WhenWalletHasInsufficientFunds()
        {
            // Arrange
            var wallet = new Wallet
            {
                Id = Guid.NewGuid(),
                Name = "Test Wallet",
                Balances = new List<WalletBalance>
                {
                    new WalletBalance(new Money(50m, "JOD"))
                }
            };

            var currencyRates = new List<CurrencyRate>
            {
                new CurrencyRate { Code = "JOD", Mid = 1.0m },
                new CurrencyRate { Code = "GMD", Mid = 1.2m }
            };
;
            var command = new ConvertCurrencyCommand(wallet.Id, "JOD", "GMD", 100m);
            var cancellationToken = new CancellationToken();

            _walletRepositoryMock
                .Setup(repo => repo.GetByIdAsync(command.WalletId))
                .ReturnsAsync(wallet);

            _currencyRateRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(currencyRates);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, cancellationToken);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage($"There are not enough funds in your wallet to change your currency.");

            _walletRepositoryMock.Verify(repo => repo.GetByIdAsync(command.WalletId), Times.Once);
            _currencyRateRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
            _walletRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Wallet>()), Times.Never);
            _loggerMock.VerifyNoOtherCalls();
        }
    }
}

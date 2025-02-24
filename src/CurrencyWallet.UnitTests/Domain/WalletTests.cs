#region Usings

using CurrencyWallet.Domain.Entities;
using CurrencyWallet.Domain.Exceptions;
using FluentAssertions;

#endregion

namespace CurrencyWallet.UnitTests.Domain
{
    public class WalletTests
    {
        [Fact]
        public void Deposit_ShouldIncreaseBalance_WhenCurrencyExists()
        {
            // Arrange
            var wallet = new Wallet { Name = "Test Wallet" };
            wallet.Deposit("JOD", 100);

            // Act
            wallet.Deposit("JOD", 50);

            // Assert
            wallet.Balances.Should().ContainSingle(b => b.Money.Currency == "JOD" && b.Money.Amount == 150);
        }

        [Fact]
        public void Deposit_ShouldAddNewCurrency_WhenCurrencyDoesNotExist()
        {
            // Arrange
            var wallet = new Wallet { Name = "Test Wallet" };

            // Act
            wallet.Deposit("GMD", 200);

            // Assert
            wallet.Balances.Should().ContainSingle(b => b.Money.Currency == "GMD" && b.Money.Amount == 200);
        }

        [Fact]
        public void Withdraw_ShouldReduceBalance_WhenSufficientFunds()
        {
            // Arrange
            var wallet = new Wallet { Name = "Test Wallet" };
            wallet.Deposit("GMD", 100);

            // Act
            bool result = wallet.Withdraw("GMD", 50);

            // Assert
            result.Should().BeTrue();
            wallet.Balances.Should().ContainSingle(b => b.Money.Currency == "GMD" && b.Money.Amount == 50);
        }

        [Fact]
        public void Withdraw_ShouldReturnFalse_WhenInsufficientFunds()
        {
            // Arrange
            var wallet = new Wallet { Name = "Test Wallet" };
            wallet.Deposit("JOD", 30);

            // Act
            bool result = wallet.Withdraw("JOD", 50);

            // Assert
            result.Should().BeFalse();
            wallet.Balances.Should().ContainSingle(b => b.Money.Currency == "JOD" && b.Money.Amount == 30);
        }

        [Fact]
        public void Withdraw_ShouldReturnFalse_WhenCurrencyNotExists()
        {
            // Arrange
            var wallet = new Wallet { Name = "Test Wallet" };

            // Act
            bool result = wallet.Withdraw("JOD", 20);

            // Assert
            result.Should().BeFalse();
            wallet.Balances.Should().BeEmpty();
        }

        [Fact]
        public void Convert_ShouldThrowException_WhenCurrencyRateNotAvailable()
        {
            // Arrange
            var wallet = new Wallet { Name = "Test Wallet" };
            wallet.Deposit("JOD", 100);
            var currencyRates = new List<CurrencyRate> { new CurrencyRate { Code = "GMD", Mid = 0.5m } };

            // Act
            Action act = () => wallet.Convert(50, "JOD", "GBP", currencyRates);

            // Assert
            act.Should().Throw<BadRequestException>()
                .WithMessage("Rates for one of the currencies are not available.");
        }

        [Fact]
        public void Convert_ShouldThrowException_WhenSourceCurrencyNotInWallet()
        {
            // Arrange
            var wallet = new Wallet { Name = "Test Wallet" };
            var currencyRates = new List<CurrencyRate>
            {
                new CurrencyRate { Code = "JOD", Mid = 1.0m },
                new CurrencyRate { Code = "GMD", Mid = 1.5m }
            };

            // Act
            Action act = () => wallet.Convert(50, "JOD", "GMD", currencyRates);

            // Assert
            act.Should().Throw<BadRequestException>()
                .WithMessage("The JOD currency is not supported by this wallet.");
        }

        [Fact]
        public void Convert_ShouldThrowException_WhenNotEnoughFunds()
        {
            // Arrange
            var wallet = new Wallet { Name = "Test Wallet" };
            wallet.Deposit("JOD", 30);
            var currencyRates = new List<CurrencyRate>
            {
                new CurrencyRate { Code = "JOD", Mid = 1.0m },
                new CurrencyRate { Code = "GMD", Mid = 1.5m }
            };

            // Act
            Action act = () => wallet.Convert(50, "JOD", "GMD", currencyRates);

            // Assert
            act.Should().Throw<BadRequestException>()
                .WithMessage("There are not enough funds in your wallet to change your currency.");
        }

        [Fact]
        public void Convert_ShouldUpdateBalances_WhenConversionIsSuccessful()
        {
            // Arrange
            var wallet = new Wallet { Name = "Test Wallet" };
            wallet.Deposit("JOD", 100);
            var currencyRates = new List<CurrencyRate>
            {
                new CurrencyRate { Code = "JOD", Mid = 1.0m },
                new CurrencyRate { Code = "GMD", Mid = 1.5m }
            };

            // Act
            wallet.Convert(40, "JOD", "GMD", currencyRates);

            // Assert
            wallet.Balances.Should().ContainSingle(b => b.Money.Currency == "JOD" && b.Money.Amount == 60);
            wallet.Balances.Should().ContainSingle(b => b.Money.Currency == "GMD" && b.Money.Amount == (40 / 1.0m * 1.5m));
        }
    }
}
#region Usings

using CurrencyWallet.Application.Commands.Handlers;
using CurrencyWallet.Application.Commands;
using CurrencyWallet.Application.Interfaces;
using CurrencyWallet.Domain.Entities;
using CurrencyWallet.Domain.Exceptions;
using FluentAssertions;
using Moq;

#endregion

namespace CurrencyWallet.UnitTests.Application.Handlers
{
    public class DeleteWalletCommandHandlerTests
    {
        private readonly Mock<IWalletRepository> _walletRepositoryMock;
        private readonly DeleteWalletCommandHandler _handler;

        public DeleteWalletCommandHandlerTests()
        {
            _walletRepositoryMock = new Mock<IWalletRepository>();
            _handler = new DeleteWalletCommandHandler(_walletRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnTrue_WhenWalletExistsAndIsDeleted()
        {
            // Arrange
            var walletId = Guid.NewGuid();
            var wallet = new Wallet { Id = walletId, Name = "Test Wallet" };
            var command = new DeleteWalletCommand(walletId);
            var cancellationToken = new CancellationToken();

            _walletRepositoryMock
                .Setup(repo => repo.GetByIdAsync(walletId))
                .ReturnsAsync(wallet);

            _walletRepositoryMock
                .Setup(repo => repo.DeleteAsync(wallet))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.Should().BeTrue();
            _walletRepositoryMock.Verify(repo => repo.GetByIdAsync(walletId), Times.Once);
            _walletRepositoryMock.Verify(repo => repo.DeleteAsync(wallet), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenWalletDoesNotExist()
        {
            // Arrange
            var walletId = Guid.NewGuid();
            var command = new DeleteWalletCommand(walletId);
            var cancellationToken = new CancellationToken();

            _walletRepositoryMock
                .Setup(repo => repo.GetByIdAsync(walletId))
                .ReturnsAsync((Wallet)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, cancellationToken);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Wallet with ID {walletId} not found.");

            _walletRepositoryMock.Verify(repo => repo.GetByIdAsync(walletId), Times.Once);
            _walletRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Wallet>()), Times.Never);
        }
    }
}

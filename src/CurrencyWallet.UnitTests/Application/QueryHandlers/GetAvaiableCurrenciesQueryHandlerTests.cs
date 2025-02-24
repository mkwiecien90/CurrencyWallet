#region Usings

using AutoMapper;
using CurrencyWallet.Application.Cache;
using CurrencyWallet.Application.Queries.Handlers;
using CurrencyWallet.Application.Queries;
using Moq;
using FluentAssertions;

#endregion

namespace CurrencyWallet.UnitTests.Application.QueryHandlers
{
    public class GetAvaiableCurrenciesQueryHandlerTests
    {
        private readonly Mock<ICurrencyCache> _currencyCacheMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAvaiableCurrenciesQueryHandler _handler;

        public GetAvaiableCurrenciesQueryHandlerTests()
        {
            _currencyCacheMock = new Mock<ICurrencyCache>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetAvaiableCurrenciesQueryHandler(_currencyCacheMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnCurrencies_WhenCurrenciesExist()
        {
            // Arrange
            var availableCurrencies = new List<string> { "JOD", "GMD" };

            _currencyCacheMock
                .Setup(cache => cache.GetAvailableCurrencies())
                .Returns(availableCurrencies);

            var query = new GetAvaiableCurrenciesQuery();
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(availableCurrencies);
            _currencyCacheMock.Verify(cache => cache.GetAvailableCurrencies(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoCurrenciesExist()
        {
            // Arrange
            var availableCurrencies = new List<string>();

            _currencyCacheMock
                .Setup(cache => cache.GetAvailableCurrencies())
                .Returns(availableCurrencies);

            var query = new GetAvaiableCurrenciesQuery();
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _currencyCacheMock.Verify(cache => cache.GetAvailableCurrencies(), Times.Once);
        }
    }
}

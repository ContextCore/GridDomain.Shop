using System;
using System.Threading.Tasks;
using Ploeh.AutoFixture;
using Shop.Domain.Aggregates.SkuAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit.XUnit.SkuAggregate.ProjectionBuilder
{
   
    public class Sku_created_projection_tests : SkuProjectionBuilderTests
    {
        private SkuCreated _message;

        public Sku_created_projection_tests() //Given_sku_created_message_projected()
        {
            _message = new Fixture().Create<SkuCreated>();
            ProjectionBuilder.Handle(_message).Wait();
        }

       [Fact]
        public async Task When_project_again_error_occures()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => ProjectionBuilder.Handle(_message));
        }

       [Fact]
        public void When_project_all_fields_are_filled()
        {
            using (var context = ContextFactory())
            {
                var row = context.Skus.Find(_message.SourceId);
                Assert.Equal(_message.SourceId, row.Id);
                Assert.Equal(_message.Article, row.Article);
                Assert.Equal(_message.Name, row.Name);
                Assert.Equal(_message.Number, row.Number);
                Assert.Equal(_message.CreatedTime, row.Created);
                Assert.Equal(_message.CreatedTime, row.LastModified);
                Assert.Equal(_message.Price.Amount, row.Price);
                Assert.Equal(_message.Price.CurrencyCode.ToString(), row.Currency);
            }
        }

       [Fact]
        public void When_project_new_row_is_added()
        {
            using (var context = ContextFactory())
            {
                Assert.NotNull(context.Skus.Find(_message.SourceId));
            }
        }
    }
}
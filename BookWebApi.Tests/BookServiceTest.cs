using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace BookWebApi.Tests
{

    public class BookServiceTest
    {

        private readonly IServiceProvider _serviceProvider;

        public BookServiceTest()
        {
            this._serviceProvider = CreateServiceProvider(services =>
            {
                services.AddAntiforgery();
            });
        }

        public static IServiceProvider CreateServiceProvider(Action<IServiceCollection> configureService)
        {
            var coll = new ServiceCollection();
            configureService(coll);
            return coll.BuildServiceProvider();
        }

        [Fact]
        public void AddTest()
        {
            //using (var scope = this._serviceProvider.CreateScope())
            //{
            //    var service = scope.ServiceProvider.GetService<IBookService>();
            //    service.AddAsync(new Book() { });
            //}
        }

    }

}
using GlobusTech.Models;
using GlobusTech.Services;
using Xunit;

namespace GlobusTech
{
    public class ServiceServiceTests
    {
        private List<Service> GetServices()
        {
            return new List<Service>
            {
                new Service { Name = "Web Development", StartDate = new DateOnly(2024, 3, 1) },
                new Service { Name = "Mobile App", StartDate = new DateOnly(2023, 12, 10) },
                new Service { Name = "IT Consulting", StartDate = new DateOnly(2024, 1, 15) }
            };
        }

        [Fact]
        public void GetServices_NoFilters_ReturnsAll()
        {
            var service = new ServiceService();

            var result = service.GetServices(GetServices(), null, 0);

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void GetServices_SearchByName_ReturnsFiltered()
        {
            var service = new ServiceService();

            var result = service.GetServices(GetServices(), "web", 0);

            Assert.Single(result);
            Assert.Equal("Web Development", result.First().Name);
        }

        [Fact]
        public void GetServices_SortByNew_ReturnsAscending()
        {
            var service = new ServiceService();

            var result = service.GetServices(GetServices(), null, 1);

            Assert.True(result[0].StartDate <= result[1].StartDate);
        }

        [Fact]
        public void GetServices_SortByOld_ReturnsDescending()
        {
            var service = new ServiceService();

            var result = service.GetServices(GetServices(), null, 2);

            Assert.True(result[0].StartDate >= result[1].StartDate);
        }

        [Fact]
        public void GetServices_SearchAndSort_WorksCorrectly()
        {
            var service = new ServiceService();

            var result = service.GetServices(GetServices(), "app", 1);

            Assert.Single(result);
            Assert.Equal("Mobile App", result.First().Name);
        }
    }
}

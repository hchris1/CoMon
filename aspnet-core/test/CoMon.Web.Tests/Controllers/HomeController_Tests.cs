using System.Threading.Tasks;
using CoMon.Models.TokenAuth;
using CoMon.Web.Controllers;
using Shouldly;
using Xunit;

namespace CoMon.Web.Tests.Controllers
{
    public class HomeController_Tests: CoMonWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}
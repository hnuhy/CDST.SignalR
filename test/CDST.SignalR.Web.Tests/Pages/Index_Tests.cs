using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace CDST.SignalR.Pages;

[Collection(SignalRTestConsts.CollectionDefinitionName)]
public class Index_Tests : SignalRWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}

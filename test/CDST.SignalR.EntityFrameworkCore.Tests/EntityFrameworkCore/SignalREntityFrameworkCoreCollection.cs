using Xunit;

namespace CDST.SignalR.EntityFrameworkCore;

[CollectionDefinition(SignalRTestConsts.CollectionDefinitionName)]
public class SignalREntityFrameworkCoreCollection : ICollectionFixture<SignalREntityFrameworkCoreFixture>
{

}

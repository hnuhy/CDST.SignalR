using CDST.SignalR.Samples;
using Xunit;

namespace CDST.SignalR.EntityFrameworkCore.Applications;

[Collection(SignalRTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<SignalREntityFrameworkCoreTestModule>
{

}

using CDST.SignalR.Samples;
using Xunit;

namespace CDST.SignalR.EntityFrameworkCore.Domains;

[Collection(SignalRTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<SignalREntityFrameworkCoreTestModule>
{

}

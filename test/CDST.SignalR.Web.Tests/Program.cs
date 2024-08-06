using Microsoft.AspNetCore.Builder;
using CDST.SignalR;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
await builder.RunAbpModuleAsync<SignalRWebTestModule>();

public partial class Program
{
}

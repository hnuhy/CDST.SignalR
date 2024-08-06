using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;
using System.Xml.Linq;
using Microsoft.AspNetCore.Identity;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using CDST.SignalR.Accounts;
using Volo.Abp.Uow;

namespace CDST.SignalR.Controllers.Auth
{
    [Controller]
    [ControllerName("用户身份验证")]
    [Route("api")]
    [AllowAnonymous]
    public class AuthController:SignalRController
    {
        private IConfiguration Configuration;
        private IAccountAppService _accountAppService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public AuthController(IConfiguration configuration, IAccountAppService accountAppService, IUnitOfWorkManager unitOfWorkManager)
        {
            Configuration = configuration;
            _accountAppService = accountAppService;
            _unitOfWorkManager = unitOfWorkManager;
        }

        /// <summary>
        /// 身份验证
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("authenticate")]
        [IgnoreAntiforgeryToken]
        public async Task<LoginResultModel> Login([FromBody] AuthDto loginDto)
        {
            if (loginDto == null)
            {
                return new LoginResultModel
                {
                    IsError = true,
                    Message = "登录信息不能为空"
                };
            }
            else if (string.IsNullOrWhiteSpace(loginDto.Username))
            {
                return new LoginResultModel
                {
                    IsError = true,
                    Message = "用户名不能为空"
                };
            }

            var userId = Guid.Empty;
            var userName = loginDto.Username;
            var name = "";
            var password = "Wfd2014!!";

            using (var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false))
            {
                //查看系统中是否存在
                var userDto = await _accountAppService.EnsureLimsUserInSystemAsync(userName, loginDto.Username, password);
                userId = userDto.Id;
                await uow.CompleteAsync();
            }

            var url = Configuration["App:DefaultAuthority"];
            string server = url.EnsureEndsWith('/');
            string tokenUrl = server + "connect/token";

            string clientId = Configuration["App:DefaultClientId"] ?? "";
            string clientSecret = Configuration["App:DefaultClientSecret"] ?? "";
            string scope = Configuration["App:DefaultScope"] ?? "";

            //解决报SSL错误The SSL connection could not be established, 
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, certChain, policyErrors) => { return true; };
            handler.SslProtocols = System.Security.Authentication.SslProtocols.None;


            var client = new HttpClient(handler);

            //Logger.LogInformation("GetDiscoveryDocumentAsync.开始：" + server);
            //var configuration = await client.GetDiscoveryDocumentAsync(server);
            //if (configuration.IsError)
            //{
            //    throw new Exception(configuration.Error);
            //}
            //tokenUrl = configuration.TokenEndpoint;
            //Logger.LogInformation("GetDiscoveryDocumentAsync,结束：" + tokenUrl);

            TokenResponse tokenResponse;

            //var passwordTokenRequest = new PasswordTokenRequest
            //{
            //    Address = tokenUrl,//
            //    ClientId = clientId,
            //    ClientSecret = clientSecret,
            //    UserName = userName,
            //    Password = password,
            //    Scope = scope,
            //};
            //passwordTokenRequest.Headers.Add("__tenant", "");

            //Logger.LogInformation("RequestPasswordTokenAsync，开始：" + server);
            //var tokenResponse = await client.RequestPasswordTokenAsync(passwordTokenRequest);

            //Logger.LogInformation("RequestPasswordTokenAsync,tokenResponse 返回：" + server);
            //if (tokenResponse.IsError)
            {
                var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);
                var collection = new List<KeyValuePair<string, string>>
                        {
                            new("client_id", clientId),
                            new("client_secret", clientSecret),
                            new("grant_type", "password"),
                            new("username", userName),
                            new("password", password),
                            new("scope", scope)
                        };
                var content = new FormUrlEncodedContent(collection);
                request.Content = content;

                Logger.LogInformation("tokenResponse为发送数据方式：" + server);
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                tokenResponse = await ProtocolResponse.FromHttpResponseAsync<TokenResponse>(response);

                if (tokenResponse.IsError)
                {
                    throw new Exception(tokenResponse.Error);
                }
            }
            Logger.LogInformation("RequestPasswordTokenAsync,结束：" + server);

            var result = new LoginResultModel();
            result.IsError = tokenResponse.IsError;
            result.TokenType = tokenResponse.TokenType;
            result.AccessToken = tokenResponse.AccessToken;
            result.RefreshToken = tokenResponse.RefreshToken;
            result.ExpiresIn = tokenResponse.ExpiresIn;
            result.UserId = userId;
            result.UserName = userName;
            result.Name = name;

            return result;
        }
    }

    /// <summary>
    /// 用户认证
    /// </summary>
    public class AuthDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public virtual string Username { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        public virtual string? Password { get; set; } = string.Empty;

        /// <summary>
        /// 加密密码
        /// </summary>
        public virtual string? CryptPassword { get; set; } = string.Empty;

        /// <summary>
        /// 标识
        /// </summary>
        public virtual string? From { get; set; } = string.Empty;
    }

    /// <summary>
    /// 登录结果
    /// </summary>
    public class LoginResultModel
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 是否错误
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// Token类型
        /// </summary>
        public string? TokenType { get; set; }

        /// <summary>
        /// 访问用
        /// </summary>
        public string? AccessToken { get; set; }

        /// <summary>
        /// 刷新用
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// 菜单
        /// </summary>
        //public List<MenuDto>? Menus { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public List<string>? Roles { get; set; }
    }
}

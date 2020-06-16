using System;
using System.Dynamic;
using HealthRiskAssessment.Configuration;
using HRATransition.External.Token;
using IEHP.DataTalk.Utilities;
using IEHP.DataTalk.Utilities.HttpClientComponent;

namespace HRATransition.External
{
	public static class TokenManager
	{

		private static AuthorizationToken _Token;

		static TokenManager()
		{
			_Token = new AuthorizationToken();
		}

		public static string Token
		{
			get
			{
				if (_Token == null || _Token.ExpireTime <= DateTime.Now)
				{
					RequestAuthorizationToken($"token{AppConfig.CurrentEnvironment}");
				}
				return _Token.AuthorizedToken;
			}
		}


		private static void RequestAuthorizationToken(String tokenEnv)
		{
			double expiredInSeconds;
			var client = new AuthorizationTokenClient();
			_Token.AuthorizedToken = client.GetToken(tokenEnv, out expiredInSeconds);
			_Token.ExpireTime = DateTime.Now.AddSeconds(expiredInSeconds).AddMinutes(-4);
			client = null;
		}

		private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
		{
			// Unix timestamp is seconds past epoch
			var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
			//;.ToLocalTime();
			return dtDateTime;
		}
		private class AuthorizationToken
		{
			public DateTime ExpireTime { get; set; }
			public string AuthorizedToken { get; set; }
		}
	}

	public class AuthorizationTokenClient : IehpHttpClientBase
	{
		public string GetToken(string tokenEnv, out double expiredInSeconds)
		{
			string token = string.Empty;
			using (var client = new HttpClientDataTalk())
			{
				var request = HttpClientComponentSettingProvider.GetRequestSetting(tokenEnv);
				var headers = HttpClientComponentSettingProvider.GetHeaderSetting("token");
				var body = HttpClientComponentSettingProvider.GetBodySetting("token");

				RequestApply(client, request);
				HeadersApply(client, headers);
				BodyApply(client, body.Content);
				try
				{
					client.Execute();
					token = GetToken(client, out expiredInSeconds);
				}
				catch (Exception)
				{
					throw;
				}

			}
			return token;
		}

		private string GetToken(HttpClientDataTalk client, out double expiredInSeconds)
		{
			dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(client.Result);
			expiredInSeconds = obj.expires_in;
			return obj.access_token;
		}

	}
}

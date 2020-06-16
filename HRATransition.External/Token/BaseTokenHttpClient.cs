using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using IEHP.DataTalk.Utilities;
using IEHP.DataTalk.Utilities.HttpClientComponent;
using IEHP.DataTalk.Utilities.Models;

namespace HRATransition.External.Token
{
	public abstract class IehpHttpClientBase
	{
		public PaginationModelDataTalk Pagination { get; set; }
		public string Body { get; private set; }
		protected void HeadersApply(HttpClientDataTalk client, HttpClientComponentHeaderSettingElement setting)
		{
			if (string.IsNullOrEmpty(setting.Accept) == false)
				client.Accept = setting.Accept;

			if (string.IsNullOrEmpty(setting.ContentType) == false)
				client.ContentType = setting.ContentType;

			if (string.IsNullOrEmpty(setting.ContentLength) == false)
				client.ContentLength = Convert.ToInt32(setting.ContentLength);

			if (string.IsNullOrEmpty(setting.Timeout) == false)
				client.Timeout = Convert.ToInt32(setting.Timeout);

			if (string.IsNullOrEmpty(setting.Custom) == false)
				HeaderCustomApply(client, setting.Custom);

		}

		protected void HeaderCustomApply(HttpClientDataTalk client, string customHeader)
		{
			//ArgusRXIntegration.ProgressNotification.Notification.Notify($"SOA Request -> Header: {customHeader}");
			var headers = new List<string>();

			headers.AddRange(customHeader.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
			foreach (var header in headers)
			{

				var attrs = header.Split(new char[] { '=' });
				if (attrs.Length == 1)
					attrs = header.Split(new char[] { ':' });
				client.Headers.Add(attrs[0], attrs[1]);
			}

		}


		protected void RequestApply(HttpClientDataTalk client, HttpClientComponentRequestSettingElement setting)
		{
			var url = string.Concat(setting.UriBase, setting.Route);
			client.Url = url;
			client.Method = setting.Action;
		}

		protected void BodyApply(HttpClientDataTalk client, string Content)
		{
			Body = Content;
			if (string.IsNullOrEmpty(Content)) return;
			client.Body = GetTokenRequestBodyInput(Content);

		}

		protected string AccessToken(string token)
			=> string.IsNullOrWhiteSpace(token) ? TokenManager.Token : token;

		private string GetTokenRequestBodyInput(string content)
		{
			var body = new ExpandoObject() as IDictionary<string, object>;
			foreach (var prop in ConvertToDictionary(";", content))
				body.Add(prop.Key, prop.Value);
			return Newtonsoft.Json.JsonConvert.SerializeObject(body);
		}

		private Dictionary<string, string> ConvertToDictionary(string delimiter, string stringList)
		{
			var dic = stringList.Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries)
				.Select(attrs => attrs.Split('='))
				.Where(attrs => attrs.Length == 2)
				.ToDictionary(prop => prop[0], prop => prop[1]);
			return dic;
		}


	}
}

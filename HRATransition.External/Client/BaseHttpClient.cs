using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using IEHP.DataTalk.Utilities;
using IEHP.DataTalk.Utilities.HttpClientComponent;
using IEHP.DataTalk.Utilities.Models;
using Newtonsoft.Json;

namespace HRATransition.External.Client
{
	public abstract class BaseHttpClient
	{
		protected int MaxRetryCount = 3;
		/// <summary>
		/// https://www.google.com/search?q=milliseconds+to+minutes&oq=millis&aqs=chrome.3.69i57j0l5.4851j0j8&sourceid=chrome&ie=UTF-8
		/// </summary>
		private const int SleepMinutesInMilleseconds = 120000; //2 minute
		protected bool SleepAndRetry(ref int currentRetryFailAttemtp)
		{
			if (currentRetryFailAttemtp < MaxRetryCount)
			{
				System.Threading.Thread.Sleep(SleepMinutesInMilleseconds);
				currentRetryFailAttemtp++;
				return true;
			}

			return false;
		}

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
			client.Body = BuiltRequestBodyInput(Content);

		}

		protected string BuiltRequestBodyInput(string content, string fieldDelimiter = ";", char pairDelimiter = '=')
		{
			var body = new ExpandoObject() as IDictionary<string, object>;
			var props = ConvertToDictionary(fieldDelimiter, pairDelimiter, content);
			if (props.Count == 0) return content;
			foreach (var prop in props)
			{
				if (IsClassObject(prop.Value) == true)
				{
					body.Add(prop.Key, ConvertToClassObject(prop.Value));
				}
				else body.Add(prop.Key, prop.Value);
			}

			return Newtonsoft.Json.JsonConvert.SerializeObject(body);
		}

		private bool IsClassObject(string data)
		{
			return (data.IndexOf("{") > -1) ? true : false;
		}

		private object ConvertToClassObject(string data)
		{
			data = data.Replace("{", string.Empty);
			data = data.Replace("}", string.Empty);
			return Newtonsoft.Json.JsonConvert.DeserializeObject(BuiltRequestBodyInput(data, ",", ':'));
		}
		private Dictionary<string, string> ConvertToDictionary(string fieldDelimiter, char pairDelimiter, string stringList)
		{
			var dic = stringList.Split(new string[] { fieldDelimiter }, StringSplitOptions.RemoveEmptyEntries)
				.Select(attrs => attrs.Split(pairDelimiter))
				.Where(attrs => attrs.Length == 2)
				.ToDictionary(prop => prop[0], prop => prop[1]);
			return dic;
		}

		protected string Serialize<T>(T values)
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(values);
		}
		protected string Serialize<T>(string propName, T values)
		{
			var obj = new ExpandoObject() as IDictionary<string, object>;
			obj.Add(propName, values);
			return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
		}

		protected string Serialize<T>(string body, string propName, T values, string fieldDelimiter = ";", string pairDelimiter = "=")
		{
			var obj = new ExpandoObject() as IDictionary<string, object>;
			foreach (var prop in body.Split(new string[] { fieldDelimiter }, StringSplitOptions.RemoveEmptyEntries))
			{
				var filter = prop.Split(new string[] { pairDelimiter }, StringSplitOptions.None);
				if (IsClassObject(filter[1]) == true)
				{
					obj.Add(filter[0], ConvertToClassObject(filter[1]));
				}
				else obj.Add(filter[0], filter[1]);
			}
			obj.Add(propName, values);
			return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
		}
		protected string ApplyCustomFilter(string body, IDictionary<string, object> filters)
		{
			var additions = new List<string>();
			foreach (var f in filters)
			{
				additions.Add($"{f.Key}={f.Value}");
			}
			return $"{body};{String.Join(";", additions.ToArray())}";
		}
		protected TResult ResponseData<TResult>(string retData)
		{
			var result = Newtonsoft.Json.JsonConvert.DeserializeObject<TResult>
				(retData,
					new JsonSerializerSettings
					{
						NullValueHandling = NullValueHandling.Ignore,
						MissingMemberHandling = MissingMemberHandling.Ignore,
						DefaultValueHandling = DefaultValueHandling.Ignore
					}
				);
			return result;
		}

		protected string AccessToken(string token = null)
		 => string.IsNullOrWhiteSpace(token) ? TokenManager.Token : token;

	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using Ebm.Soa.Models;
using HealthRiskAssessment.Configuration;
using IEHP.DataTalk.Utilities;
using IEHP.DataTalk.Utilities.HttpClientComponent;

namespace HRATransition.External.Client
{
	internal class SoaContext : BaseHttpClient
	{
		internal long[] MemberIdList { get; set; }

		#region MyRegion

		public ICollection<MemberDemographicModel> GetDemographic()
		{
			var memberIdList = GetDemographicLookup();
			return GetDemographicDetail(memberIdList);
		}

		public ICollection<long> GetDemographicLookup()
		{
			return GetDemographicLookup(0);
		}

		private ICollection<long> GetDemographicLookup(int currentRetryFailAttemtp)
		{
			ICollection<long> ret;
			// Get from Soa
			using (var client = new HttpClientDataTalk())
			{
				var request = HttpClientComponentSettingProvider.GetRequestSetting($"msademographiclookup{AppConfig.CurrentEnvironment}");
				var headers = HttpClientComponentSettingProvider.GetHeaderSetting("msa");
				//var body = HttpClientComponentSettingProvider.GetBodySetting($"msademographiclookup");

				RequestApply(client, request);
				HeadersApply(client, headers);
				HeaderCustomApply(client, $"Authorization: Bearer {AccessToken()}");
				//BodyApply(client, body.Content);
				client.Body += Serialize<long[]>(MemberIdList);
				try
				{
					client.Execute();
					ret = ResponseData<ICollection<long>>(client.Result);
				}
				catch (Exception ex)
				{
					if (SleepAndRetry(ref currentRetryFailAttemtp))
					{
						return GetDemographicLookup(currentRetryFailAttemtp);
					}
					
					throw new Exception($"{request}", ex);

				}
			}
			return ret;
		}

		public ICollection<MemberDemographicModel> GetDemographicDetail(IEnumerable<long> memberIdList)
		{
			return GetDemographicDetail(memberIdList, 0);
		}
		private ICollection<MemberDemographicModel> GetDemographicDetail(IEnumerable<long> memberIdList, int currentRetryFailAttemtp)
		{

			ICollection<MemberDemographicModel> ret;
			// Get from Soa
			using (var client = new HttpClientDataTalk())
			{
				var request = HttpClientComponentSettingProvider.GetRequestSetting($"msademographicdetail{AppConfig.CurrentEnvironment}");
				var headers = HttpClientComponentSettingProvider.GetHeaderSetting("msa");
				//var body = HttpClientComponentSettingProvider.GetBodySetting("msademographicdetail");

				RequestApply(client, request);
				HeadersApply(client, headers);
				HeaderCustomApply(client, $"Authorization: Bearer {AccessToken()}");
				//BodyApply(client, body.Content);
				client.Body += Serialize<long[]>((memberIdList ?? new long[] { 0 }).ToArray());
				try
				{
					client.Execute();
					ret = ResponseData<ICollection<MemberDemographicModel>>(client.Result);
				}
				catch (Exception ex)
				{
					if (SleepAndRetry(ref currentRetryFailAttemtp))
					{
						return GetDemographicDetail(memberIdList, currentRetryFailAttemtp);
					}
					
					throw new Exception($"{request}", ex);
				}
			}
			return ret;
		}


		#endregion

		#region Eligiblities
		
		internal ICollection<SoaEligibility> GetEligibilities()
			=> GetEligibilities(0);

		private ICollection<SoaEligibility> GetEligibilities(int currentRetryFailAttemtp)
		{

			ICollection<SoaEligibility> ret;
			// Get from Soa
			using (var client = new HttpClientDataTalk())
			{
				const string bodySettingName = "msadeltaeligibility";

				var request = HttpClientComponentSettingProvider.GetRequestSetting($"msaeligibility{AppConfig.CurrentEnvironment}");
				var headers = HttpClientComponentSettingProvider.GetHeaderSetting("msa");
				var body = HttpClientComponentSettingProvider.GetBodySetting(bodySettingName);

				RequestApply(client, request);
				HeadersApply(client, headers);
				HeaderCustomApply(client, $"Authorization: Bearer {AccessToken()}");				
				BodyApply(client, Serialize<long[]>(body.Content, "MemberNumbers", MemberIdList));
				try
				{
					client.Execute();
					ret = ResponseData<ICollection<SoaEligibility>>(client.Result);
				}
				catch (Exception ex)
				{
					if (SleepAndRetry(ref currentRetryFailAttemtp))
					{
						return GetEligibilities( currentRetryFailAttemtp);
					}

					throw new Exception($"{request}", ex);
				}
			}
			return ret;
		}
		#endregion

		#region Provider

		internal ICollection<ProviderDetailModel> GetProviders(long[] providerKeyList)
			=> GetProviders(providerKeyList, 0);

		private ICollection<ProviderDetailModel> GetProviders(long[] providerKeyList, int currentRetryFailAttemtp)
		{

			ICollection<ProviderDetailModel> ret;
			// Get from Soa
			using (var client = new HttpClientDataTalk())
			{
				var bodySettingName = "msaproviderdetail";

				var request = HttpClientComponentSettingProvider.GetRequestSetting($"msaproviderdetail{AppConfig.CurrentEnvironment}");
				var headers = HttpClientComponentSettingProvider.GetHeaderSetting("msa");
				var body = HttpClientComponentSettingProvider.GetBodySetting(bodySettingName);

				RequestApply(client, request);
				HeadersApply(client, headers);
				HeaderCustomApply(client, $"Authorization: Bearer {AccessToken()}");
				BodyApply(client, Serialize<long[]>(body.Content, "ProviderKeys", providerKeyList));
				try
				{
					client.Execute();
					ret = ResponseData<ICollection<ProviderDetailModel>>(client.Result);
				}
				catch (Exception ex)
				{
					if (SleepAndRetry(ref currentRetryFailAttemtp))
					{
						return GetProviders(providerKeyList, currentRetryFailAttemtp);
					}
					throw new Exception($"{request}", ex);
				}
			}
			return ret;
		}


		#endregion

		#region Provider Contract Map

		internal ICollection<ProviderContractMap> GetProviderContractMap(long[] contractMapKey)
			=> GetProviderContractMap(contractMapKey, 0);

		private ICollection<ProviderContractMap> GetProviderContractMap(long[] contractMapKey, int currentRetryFailAttemtp)
		{

			ICollection<ProviderContractMap> ret;
			// Get from Soa
			using (var client = new HttpClientDataTalk())
			{
				var bodySettingName = "msaprovidercontractmapdetail";

				var request = HttpClientComponentSettingProvider.GetRequestSetting($"msaprovidercontractmap{AppConfig.CurrentEnvironment}");
				var headers = HttpClientComponentSettingProvider.GetHeaderSetting("msa");
				var body = HttpClientComponentSettingProvider.GetBodySetting(bodySettingName);

				RequestApply(client, request);
				HeadersApply(client, headers);
				HeaderCustomApply(client, $"Authorization: Bearer {AccessToken()}");
				BodyApply(client, body.Content);
				client.Body += Serialize<long[]>((contractMapKey ?? new long[] { 0 }).ToArray());
				try
				{
					client.Execute();
					ret = ResponseData<ICollection<ProviderContractMap>>(client.Result);
				}
				catch (Exception ex)
				{
					if (SleepAndRetry(ref currentRetryFailAttemtp))
					{
						return GetProviderContractMap(contractMapKey, currentRetryFailAttemtp);
					}
					throw new Exception($"{request}", ex);
				}
			}
			return ret;
		}


		#endregion
	}
}

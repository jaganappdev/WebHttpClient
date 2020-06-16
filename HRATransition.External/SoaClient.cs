using System;
using System.Collections.Generic;
using System.Linq;
using Ebm.Soa.Models;
using HRATransition.External.Client;
using HRATransition.External.Extension;

namespace HRATransition.External
{
	public static class SoaClient
	{
	    public static IEnumerable<MemberDemographicModel> GetDemographicData(long[] memberNumberList)
	    {
	        var client = new SoaContext
	        {
	            MemberIdList = memberNumberList
	        };
	        var demographic = client.GetDemographic();
	        return demographic;
	    }

        public static IEnumerable<SoaEligibility> GetEligbilityData(long[] memberNumberList)
		{
			var client = new SoaContext
			{
				MemberIdList = memberNumberList
			};
			var eligibilities = client.GetEligibilities();
			return eligibilities;
		}

		public static IEnumerable<HraSoaModel> GetEligilityAndPcpData(long[] memberNumberList)
		{
			var client = new SoaContext
			{
				MemberIdList = memberNumberList
			};

			var demographicList = client.GetDemographic();			
			var eligibilities = client.GetEligibilities();

			var member =
				demographicList?.GroupJoin(eligibilities, d => d.Identity.MemberNumber, e => e.MemberNumber, ToMemberEligibility);

			// -- Exclude invalid Assigned Provider Data
			var assignedproviderList = eligibilities.SelectMany(_ => _.EligibilityDetail.AssignedProvider)
				.Where(_ => _.TerminationDate == DateTime.MinValue || (_.EffectiveDate < _.TerminationDate))
				.OrderByDescending(_ => _.TerminationDate)
				//.ThenByDescending(_=>_.AssignedProviderKey)
				.ToList();

			// -- Get Providers Data
			var providers = client.GetProviders(assignedproviderList.Select(_ => _.ProviderKey).ToArray());

			// -- Merge Providers with Assigned Providers Data
			var assignedProviders = assignedproviderList.GroupJoin(providers, a => a.ProviderKey, p => p.ProviderKey, ToProvider);

			// -- Merge Member with Assigned Provider and Provider
			var memberWithProviders = member?.GroupJoin
				(assignedProviders, m => m.MemberNumber, e => e.MemberNumber, ToMemberProviders);

			// -- Merge Member Provider With Contract Info
			var providerContractMaps = client.GetProviderContractMap(assignedproviderList.Select(_=>_.ContractMapKey).ToArray());
			var ret = memberWithProviders?.GroupJoin(providerContractMaps, m => m.MemberProvider.ContractMapKey,c => c.ContractMapKey, ToContractMap);
			return ret;		

		}

		private static HraSoaModel ToContractMap(HraSoaModel hraSoa, IEnumerable<ProviderContractMap> contracMaps)
		{
			hraSoa.ContractMap = contracMaps.FirstOrDefault();
			return hraSoa;
		}

		private static HraSoaModel ToMemberEligibility(MemberDemographicModel demographic, IEnumerable<SoaEligibility> eligibilities)
		{
			return new HraSoaModel()
			{
				MemberNumber = demographic.Identity.MemberNumber.ToString(),
				Demographic = demographic,
				Eligibilities = eligibilities?.ToList()
			};
		}

		private static MemberProviderModel ToProvider(MemberAssignedProviderModel assignedProvider, IEnumerable<ProviderDetailModel> providerDetails)
		{
			var memberProvidver = new MemberProviderModel()
			{
				MemberNumber = assignedProvider.MemberNumber.ToString(),
				IpaName = assignedProvider.RiskGroupName,
				GroupNumber = assignedProvider.GroupNumber,
				ContractMapKey = assignedProvider.ContractMapKey,
				AssignedProviderId = assignedProvider.AssignedProviderKey
			};

			var providerDetailModels = providerDetails.ToList();
			if (providerDetailModels.Any() == false)
				return memberProvidver;

			memberProvidver.ProviderDetail = providerDetailModels.FirstOrDefault();
			return memberProvidver;
		}

		private static HraSoaModel ToMemberProviders(HraSoaModel hraSoa, IEnumerable<MemberProviderModel> assignedProviders)
		{
			hraSoa.MemberProvider = new MemberProviderModel();
			var assignedProviderList = assignedProviders.ToList();
			if (assignedProviderList.Any() == false)			
			return hraSoa;

			// -- EBM has duplicated records with same info and the most recent record is the one
			hraSoa.MemberProvider = assignedProviderList.OrderByDescending(_=>_.AssignedProviderId).FirstOrDefault();		
			return hraSoa;
		}

	}
}

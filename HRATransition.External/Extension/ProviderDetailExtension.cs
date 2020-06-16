using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ebm.Soa.Models;
using IEHP.DataTalk.Utilities;

namespace HRATransition.External.Extension
{
	internal static class ProviderDetailExtension
	{
		//internal static void PopulateIpa(this HraSoaModel member, MemberProviderModel provider)
		//{
		//	member.IpaName = provider.IpaName;
		//	member.Ipa = provider.GroupNumber;
		//	var detail = provider.ProviderDetail;
		//	member.PcpFirstName = detail.FirstName;
		//	member.PcpLastName = detail.LastName;
		//	member.Pcp = detail.NationalProviderId.GetDataSafeDataTalk<string>();
		//	member.PopulateIpaOffice(provider.ProviderDetail.PrimaryOffice);
		//}

		//private static void PopulateIpaOffice(this HraSoaModel member, OfficeDetailModel officeDetail)
		//{
		//	if (officeDetail == null)
		//		return;
		//	member.
		//	member.PcpPhone = officeDetail.ContactPhone.GetDataSafeDataTalk<string>();
		//	member.PcpFax = officeDetail.ContactFax.GetDataSafeDataTalk<string>();

		//	member.PcpAddress = officeDetail?.Address?.Address1;
		//	member.PcpCity = officeDetail?.Address?.City;

		//	member.PcpState = officeDetail?.Address?.State;
		//	member.PcpZip = officeDetail?.Address?.Zip.GetDataSafeDataTalk<string>();

		//}

	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ebm.Soa.Models;

namespace HRATransition.External
{
	public class MemberProviderModel
	{
		public string MemberNumber { get; set; }
		public string IpaName { get; set; }
		public string Ipa { get; set; }
		public string GroupNumber { get; set; }
		public string Pcp { get; set; }
		public string PcpFirstName { get; set; }
		public string PcpLastName { get; set; }
		public long ContractMapKey { get; set; }
		public long AssignedProviderId { get; set; }
		public ProviderDetailModel ProviderDetail { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ebm.Soa.Models;

namespace HRATransition.External
{
	public class HraSoaModel
	{
		public string MemberNumber { get; set; }
		public MemberDemographicModel Demographic { get; set; }
		public ICollection<SoaEligibility> Eligibilities { get; set; }
		public MemberProviderModel MemberProvider { get; set; }
		public ProviderContractMap ContractMap { get; set; }

	}
}

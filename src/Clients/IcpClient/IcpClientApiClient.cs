using EdjCase.ICP.Agent.Agents;
using EdjCase.ICP.Candid.Models;
using EdjCase.ICP.Candid;
using System.Threading.Tasks;
using System.Collections.Generic;
using Adna.HashingBlockchain.ICP.IcpClient;
using EdjCase.ICP.Agent.Responses;
using RootHash = System.String;

namespace Adna.HashingBlockchain.ICP.IcpClient
{
	public class IcpClientApiClient
	{
		public IAgent Agent { get; }

		public Principal CanisterId { get; }

		public CandidConverter? Converter { get; }

		public IcpClientApiClient(IAgent agent, Principal canisterId, CandidConverter? converter = default)
		{
			this.Agent = agent;
			this.CanisterId = canisterId;
			this.Converter = converter;
		}

		public async Task<OptionalValue<List<Models.Transaction>>> GetRootHash(RootHash arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "getRootHash", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<OptionalValue<List<Models.Transaction>>>(this.Converter);
		}

		public async Task StoreRootHash(RootHash arg0, Models.Transaction arg1)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter));
			await this.Agent.CallAndWaitAsync(this.CanisterId, "storeRootHash", arg);
		}
	}
}
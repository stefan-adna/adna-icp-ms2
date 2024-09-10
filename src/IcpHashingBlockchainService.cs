using Adna.HashingBlockchain.ICP.IcpClient;
using Adna.HashingBlockchain.ICP.IcpClient.Models;
using EdjCase.ICP.Agent.Agents;
using EdjCase.ICP.Agent.Identities;
using EdjCase.ICP.Candid.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adna.HashingBlockchain.ICP
{
    public class IcpHashingBlockchainService : IHashingBlockchainService
    {
        private readonly IBlockchainDataAccess _dataAccess;
        private readonly ILogger<IcpHashingBlockchainService> _logger;
        private readonly Principal _cannister;
        private readonly HttpAgent _agent;

        public IcpHashingBlockchainService(IConfiguration configuration, IBlockchainDataAccess dataAccess, ILogger<IcpHashingBlockchainService> logger)
        {
            _dataAccess = dataAccess;
            _logger = logger;

            IIdentity identity = null;
            _agent = new HttpAgent(identity, new Uri(configuration["BaseUrl"]));
            _cannister = Principal.FromText(configuration["CannisterId"]);
        }

        public async Task Init()
        {
            await Task.FromResult(0);
        }

        public async Task WritePendingHashes()
        {
            //get all hashes that have STATUS=PENDING in ClientHash table
            var pendingClientHashes = await _dataAccess.PendingClientHashes();
            _logger.LogInformation($"Pending hashes {pendingClientHashes.Count}");

            var client = new IcpClientApiClient(_agent, _cannister);

            foreach (var pendingClientHash in pendingClientHashes)
            {
                var rootHash = pendingClientHash.Hash.MerkleTreeRootHash;
                var transactionId = $"0x{Guid.NewGuid():n}";

                var transaction = new Transaction()
                {
                    ClientIdHash = rootHash,
                    TransactionId = transactionId,
                    CreateDateTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()
                };

                await client.StoreRootHash(rootHash, transaction);
                await _dataAccess.UpdateTransactionHash([pendingClientHash], transactionId);
            }

            await _dataAccess.Success(pendingClientHashes);
        }
    }
}

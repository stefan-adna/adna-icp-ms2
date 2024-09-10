using Time = EdjCase.ICP.Candid.Models.UnboundedInt;

namespace Adna.HashingBlockchain.ICP.IcpClient.Models
{
	public class Transaction
	{
		public string ClientIdHash { get; set; }

		public Time CreateDateTime { get; set; }

		public string TransactionId { get; set; }

		public Transaction(string clientIdHash, Time createDateTime, string transactionId)
		{
			this.ClientIdHash = clientIdHash;
			this.CreateDateTime = createDateTime;
			this.TransactionId = transactionId;
		}

		public Transaction()
		{
		}
	}
}
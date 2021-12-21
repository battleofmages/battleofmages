using BoM.Core;
using System;
using Unity.Netcode;

namespace BoM.Accounts {
	[Serializable]
	public class Account: IAccount, INetworkSerializable {
		public string Id { get { return id; } }
		public string Nick { get { return nick; } }
		public string Email { get { return email; } }

		private string id;
		private string nick;
		private string email;

		public Account(string newId, string newNick, string newEmail) {
			id = newId;
			nick = newNick;
			email = newEmail;
		}

		public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
			serializer.SerializeValue(ref id);
			serializer.SerializeValue(ref nick);
			serializer.SerializeValue(ref email);
		}
	}
}

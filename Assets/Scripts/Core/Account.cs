namespace BoM.Core {
	public class Account {
		public string Id { get; }
		public string Nick { get; }
		public string Email { get; }

		public Account(string id, string nick, string email) {
			Id = id;
			Nick = nick;
			Email = email;
		}
	}
}

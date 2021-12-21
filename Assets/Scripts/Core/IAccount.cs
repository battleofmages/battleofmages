using System;

namespace BoM.Core {
	public interface IAccount {
		string Id { get; }
		string Nick { get; }
		string Email { get; }
	}
}

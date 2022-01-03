using UnityEngine;

namespace BoM.Core {
	public interface ISkill {
		short Id { get; }
		string Name { get; }
		Sprite Icon { get; }
		float CoolDown { get; }
	}
}

using NUnit.Framework;
using UnityEngine;

namespace BoM {
	class PlayerTest {
		[Test]
		public void InitialValues() {
			var playerObject = new GameObject("PlayerTest");
			var player = playerObject.AddComponent<PlayerMain>();
			
			try {
				Assert.AreEqual(player.maxHealth, player.health);
				Assert.AreEqual(player.maxEnergy, player.energy);
				Assert.AreEqual(1f, player.attackDmgMultiplier);
				Assert.AreEqual(1f, player.defenseDmgMultiplier);
				Assert.AreEqual(1f, player.attackSpeedMultiplier);
				Assert.AreEqual(1f, player.moveSpeedModifier);
				Assert.IsNull(player.currentSkill);
				Assert.IsNull(player.account);
			} finally {
				Object.DestroyImmediate(playerObject);
			}
		}

		[Test]
		public void DeadAbilities() {
			var playerObject = new GameObject("PlayerTest");
			var player = playerObject.AddComponent<PlayerMain>();

			try {
				// Dead players can't do that
				Assert.IsFalse(player.canBlock);
				Assert.IsFalse(player.canCast);
				Assert.IsFalse(player.canBePulled);
				Assert.IsFalse(player.canHover);
				Assert.IsTrue(player.cantMove);
			} finally {
				Object.DestroyImmediate(playerObject);
			}
		}
	}
}
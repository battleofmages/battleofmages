using NUnit.Framework;
using UnityEngine;

namespace BoM {
	class ArtifactTest {
		[Test]
		public void RarityTest() {
			var common =	new Artifact(2 + (1 * 6) + (0 * 216));
			var uncommon =	new Artifact(2 + (0 * 6) + (0 * 216));
			var rare =		new Artifact(0 + (0 * 6) + (0 * 216));

			Assert.AreEqual(Rarity.Common, common.rarity);
			Assert.AreEqual(Rarity.Uncommon, uncommon.rarity);
			Assert.AreEqual(Rarity.Rare, rare.rarity);
		}

		[Test]
		public void JSON() {
			GameDB.InitCodecs();

			var obj = new Artifact(0);
			var json = Jboy.Json.WriteObject(obj);
			var objClone = Jboy.Json.ReadObject<Artifact>(json);

			Assert.IsTrue(obj.id == objClone.id);
		}
	}
}
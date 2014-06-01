using NUnit.Framework;

namespace BoM {
	class CharacterStatsTest {
		[Test]
		public void StatPointsUsed() {
			var charStats = new CharacterStats();
			
			Assert.LessOrEqual(charStats.totalStatPointsUsed, charStats.maxStatPoints);
		}

		[Test]
		public void DefaultBuild() {
			var charStats = new CharacterStats();

			Assert.AreEqual(50, charStats.attack);
			Assert.AreEqual(50, charStats.defense);
			Assert.AreEqual(50, charStats.block);
			Assert.AreEqual(50, charStats.cooldownReduction);
			Assert.AreEqual(50, charStats.attackSpeed);
			Assert.AreEqual(50, charStats.moveSpeed);
		}

		[Test]
		public void ApplyOffset() {
			var charStats = new CharacterStats();
			charStats.ApplyOffset(10);

			Assert.AreEqual(60, charStats.attack);
			Assert.AreEqual(60, charStats.defense);
			Assert.AreEqual(60, charStats.block);
			Assert.AreEqual(60, charStats.cooldownReduction);
			Assert.AreEqual(60, charStats.attackSpeed);
			Assert.AreEqual(60, charStats.moveSpeed);
		}

		[Test]
		public void JSON() {
			var charStats = new CharacterStats();
			var json = Jboy.Json.WriteObject(charStats);
			var charStatsClone = Jboy.Json.ReadObject<CharacterStats>(json);

			Assert.IsTrue(charStats.Compare(charStatsClone));
		}
	}
}
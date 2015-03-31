using NUnit.Framework;
using BoM;

namespace BoM.Tests {
	// TraitsTest
	class TraitsTest {
		[Test]
		public void Default_traits_should_be_0_points_each() {
			var traits = new Traits();
			
			Assert.AreEqual(0, traits.attack);
			Assert.AreEqual(0, traits.defense);
			Assert.AreEqual(0, traits.energy);
			Assert.AreEqual(0, traits.cooldownReduction);
			Assert.AreEqual(0, traits.attackSpeed);
			Assert.AreEqual(0, traits.moveSpeed);
		}

		[Test]
		public void Total_points_used_should_be_less_or_equal_than_max_points() {
			var traits = new Traits();
			
			Assert.LessOrEqual(traits.totalPointsUsed, traits.maxPoints);
		}
		
		[Test]
		public void Serializing_traits_to_JSON_and_deserializing_it_back_should_lead_to_the_same_traits() {
			var traits = new Traits();
			var json = Jboy.Json.WriteObject(traits);
			var charStatsClone = Jboy.Json.ReadObject<Traits>(json);
			
			Assert.IsTrue(traits.Compare(charStatsClone));
		}

		[Test]
		public void Applying_an_offset_should_increase_all_traits_by_a_certain_value([NUnit.Framework.Range(-100, 100, 50)] int offset) {
			var traits = new Traits();
			traits.ApplyOffset(offset);

			var expectedValue = Traits.defaultValue + offset;
			Assert.AreEqual(expectedValue, traits.attack);
			Assert.AreEqual(expectedValue, traits.defense);
			Assert.AreEqual(expectedValue, traits.energy);
			Assert.AreEqual(expectedValue, traits.cooldownReduction);
			Assert.AreEqual(expectedValue, traits.attackSpeed);
			Assert.AreEqual(expectedValue, traits.moveSpeed);
		}

		[Test]
		public void Equal_Attack_vs_Defense_points_should_not_change_the_damage([NUnit.Framework.Range(-50, 50, 10)] int value) {
			float damage = 1000f;
			var traits = new Traits(value);
			var resultingDamage = damage * traits.attackDmgMultiplier * traits.defenseDmgMultiplier;

			Assert.That(damage, Is.EqualTo(resultingDamage).Within(0.1));
		}
	}
}
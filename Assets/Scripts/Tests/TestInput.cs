using BoM.Players;
using NUnit.Framework;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

[TestFixture]
public class TestInput {
	private InputTestFixture input = new InputTestFixture();

	[OneTimeSetUp]
	public void Setup() {
		SceneManager.LoadScene("Main");
	}

	[TearDown]
	public void TearDown() {
		var player = Player.Main;
		player.Respawn(player.Team.RandomSpawnPosition, player.Team.SpawnRotation);
	}

	[UnityTest]
	public IEnumerator Connectivity() {
		yield return null;

		Assert.That(NetworkManager.Singleton, Is.Not.Null);
		Assert.That(NetworkManager.Singleton.IsClient, Is.True);
		Assert.That(NetworkManager.Singleton.IsServer, Is.True);
		Assert.That(NetworkManager.Singleton.IsHost, Is.True);

		Assert.That(Player.Main, Is.Not.Null);
		Assert.That(Player.Main.IsOwner, Is.True);
	}

	[UnityTest]
	public IEnumerator Movement() {
		var player = Player.Main;
		var position = player.transform.position;
		var keyboard = InputSystem.AddDevice<Keyboard>();

		yield return TestMovementKey(keyboard.wKey, player.transform);
		yield return TestMovementKey(keyboard.sKey, player.transform);
		yield return TestMovementKey(keyboard.aKey, player.transform);
		yield return TestMovementKey(keyboard.dKey, player.transform);
	}

	private IEnumerator TestMovementKey(ButtonControl key, Transform transform) {
		var position = transform.position;

		input.Press(key);
		yield return new WaitForSeconds(0.25f);
		Assert.That(Vector3.SqrMagnitude(transform.position - position), Is.GreaterThan(0.0001f));

		position = transform.position;

		input.Release(key);
		yield return new WaitForSeconds(0.25f);
		Assert.That(Vector3.SqrMagnitude(transform.position - position), Is.LessThan(0.0001f));
	}

	[UnityTest]
	public IEnumerator Skills() {
		var keyboard = InputSystem.AddDevice<Keyboard>();
		var player = Player.Main;
		var skillSystem = player.GetComponent<SkillSystem>();

		yield return TestSkillKey(keyboard.digit1Key, skillSystem);
		yield return TestSkillKey(keyboard.digit2Key, skillSystem);
		yield return TestSkillKey(keyboard.digit3Key, skillSystem);
		yield return TestSkillKey(keyboard.digit4Key, skillSystem);
	}

	private IEnumerator TestSkillKey(ButtonControl key, SkillSystem skillSystem) {
		Assert.That(skillSystem.isCasting, Is.False);

		input.PressAndRelease(key);
		yield return new WaitForEndOfFrame();
		Assert.That(skillSystem.isCasting, Is.True);

		yield return new WaitForSeconds(1f);
		Assert.That(skillSystem.isCasting, Is.False);
	}
}

using UnityEngine;

public class LevelProgressView : HUDElement {
	public ProgressBarStyle progressBarStyle;

	[Range(0.1f, 2.0f)]
	public float interpolationSpeed;

	private PlayerAccount account;
	private float progress;
	private float interpolationTime;

	// Update
	void Update() {
		account = PlayerAccount.mine;
		
		if(account == null)
			return;
		
		var level = account.level;
		float newProgress = (float)level - (int)level;

		if(System.Math.Abs(newProgress - progress) > 0.001f && interpolationTime >= 1f) {
			interpolationTime = 0f;
		}

		progress = Mathf.Lerp(progress, newProgress, interpolationTime);

		interpolationTime += Time.deltaTime * interpolationSpeed;
	}

	// Draw
	public override void Draw() {
		if(account == null)
			return;

		GUIHelper.ProgressBar(
			"",
			progress,
			account.experience + " EXP (" + (progress * 100).ToString("0") + " %)",
			progressBarStyle
		);
	}
}

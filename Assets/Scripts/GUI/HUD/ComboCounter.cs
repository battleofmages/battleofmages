using UnityEngine;
using System.Collections;

public class ComboCounter : HUDElement {
	public static double comboResetTime = 3.0f;
	
	public AnimationCurve alphaCurve;
	public GUIStyle comboCounterStyle;
	
	private int _hitCount;
	private int _damage;
	private double lastHitTime;
	
	// AddHit
	public void AddHit(int hitDamage) {
		_hitCount += 1;
		_damage += hitDamage;
		lastHitTime = uLink.Network.time;
	}
	
	// Draw
	public override void Draw() {
		if(_hitCount <= 0)
			return;
		
		float alpha = alphaCurve.Evaluate((float)((uLink.Network.time - lastHitTime) / comboResetTime));
		string text = "<size=40>" + _hitCount + "</size> hits\n<size=24>" + _damage + "</size> damage";
		
		// Draw shadowed
		GUI.color = new Color(1.0f, 1.0f, 1.0f, alpha);
		GUIHelper.Shadowed(
			0f,
			0f,
			(x, y) => {
				GUI.Label(new Rect(
					x, y,
					GUIArea.width, GUIArea.height
				), text, comboCounterStyle);
			}
		);
	}
	
	// Update
	void Update() {
		if(uLink.Network.time - lastHitTime >= comboResetTime) {
			_hitCount = 0;
			_damage = 0;
		}
	}
	
	// Count
	public int count {
		get { return _hitCount; }
	}
	
	// Damage
	public int damage {
		get { return _damage; }
	}
}

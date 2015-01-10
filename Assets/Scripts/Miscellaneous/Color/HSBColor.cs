using UnityEngine;

[System.Serializable]
public struct HSBColor {
	public float hue;
	public float saturation;
	public float brightness;
	public float alpha;
	
	public HSBColor(float h, float s, float b, float a) {
		this.hue = h;
		this.saturation = s;
		this.brightness = b;
		this.alpha = a;
	}
	
	public HSBColor(float h, float s, float b) {
		this.hue = h;
		this.saturation = s;
		this.brightness = b;
		this.alpha = 1f;
	}
	
	public HSBColor(Color col) {
		HSBColor temp = FromColor(col);
		hue = temp.hue;
		saturation = temp.saturation;
		brightness = temp.brightness;
		alpha = temp.alpha;
	}
	
	public static HSBColor FromColor(Color color) {
		HSBColor ret = new HSBColor(0f, 0f, 0f, color.a);
		
		float r = color.r;
		float g = color.g;
		float b = color.b;
		
		float max = Mathf.Max(r, Mathf.Max(g, b));
		
		if (max <= 0)
		{
			return ret;
		}
		
		float min = Mathf.Min(r, Mathf.Min(g, b));
		float dif = max - min;
		
		if (max > min)
		{
			if (g == max)
			{
				ret.hue = (b - r) / dif * 60f + 120f;
			}
			else if (b == max)
			{
				ret.hue = (r - g) / dif * 60f + 240f;
			}
			else if (b > g)
			{
				ret.hue = (g - b) / dif * 60f + 360f;
			}
			else
			{
				ret.hue = (g - b) / dif * 60f;
			}
			if (ret.hue < 0)
			{
				ret.hue = ret.hue + 360f;
			}
		}
		else
		{
			ret.hue = 0;
		}
		
		ret.hue *= 1f / 360f;
		ret.saturation = (dif / max) * 1f;
		ret.brightness = max;
		
		return ret;
	}
	
	public static Color ToColor(HSBColor hsbColor) {
		float r = hsbColor.brightness;
		float g = hsbColor.brightness;
		float b = hsbColor.brightness;
		if (hsbColor.saturation != 0)
		{
			float max = hsbColor.brightness;
			float dif = hsbColor.brightness * hsbColor.saturation;
			float min = hsbColor.brightness - dif;
			
			float h = hsbColor.hue * 360f;
			
			if (h < 60f)
			{
				r = max;
				g = h * dif / 60f + min;
				b = min;
			}
			else if (h < 120f)
			{
				r = -(h - 120f) * dif / 60f + min;
				g = max;
				b = min;
			}
			else if (h < 180f)
			{
				r = min;
				g = max;
				b = (h - 120f) * dif / 60f + min;
			}
			else if (h < 240f)
			{
				r = min;
				g = -(h - 240f) * dif / 60f + min;
				b = max;
			}
			else if (h < 300f)
			{
				r = (h - 240f) * dif / 60f + min;
				g = min;
				b = max;
			}
			else if (h <= 360f)
			{
				r = max;
				g = min;
				b = -(h - 360f) * dif / 60 + min;
			}
			else
			{
				r = 0;
				g = 0;
				b = 0;
			}
		}
		
		return new Color(Mathf.Clamp01(r),Mathf.Clamp01(g),Mathf.Clamp01(b),hsbColor.alpha);
	}
	
	public Color ToColor() {
		return ToColor(this);
	}
	
	public override string ToString() {
		return "H:" + hue + " S:" + saturation + " B:" + brightness;
	}
	
	public static HSBColor Lerp(HSBColor a, HSBColor b, float t) {
		float h,s;
		
		//check special case black (color.b==0): interpolate neither hue nor saturation!
		//check special case grey (color.s==0): don't interpolate hue!
		if(a.brightness==0){
			h=b.hue;
			s=b.saturation;
		}else if(b.brightness==0){
			h=a.hue;
			s=a.saturation;
		}else{
			if(a.saturation==0){
				h=b.hue;
			}else if(b.saturation==0){
				h=a.hue;
			}else{
				// works around bug with LerpAngle
				float angle = Mathf.LerpAngle(a.hue * 360f, b.hue * 360f, t);
				while (angle < 0f)
					angle += 360f;
				while (angle > 360f)
					angle -= 360f;
				h=angle/360f;
			}
			s=Mathf.Lerp(a.saturation,b.saturation,t);
		}
		return new HSBColor(h, s, Mathf.Lerp(a.brightness, b.brightness, t), Mathf.Lerp(a.alpha, b.alpha, t));
	}
	
	public static void Test() {
		HSBColor color;
		
		color = new HSBColor(Color.red);
		Debug.Log("red: " + color);
		
		color = new HSBColor(Color.green);
		Debug.Log("green: " + color);
		
		color = new HSBColor(Color.blue);
		Debug.Log("blue: " + color);
		
		color = new HSBColor(Color.grey);
		Debug.Log("grey: " + color);
		
		color = new HSBColor(Color.white);
		Debug.Log("white: " + color);
		
		color = new HSBColor(new Color(0.4f, 1f, 0.84f, 1f));
		Debug.Log("0.4, 1f, 0.84: " + color);
		
		Debug.Log("164,82,84   .... 0.643137f, 0.321568f, 0.329411f  :" + ToColor(new HSBColor(new Color(0.643137f, 0.321568f, 0.329411f))));
	}
}
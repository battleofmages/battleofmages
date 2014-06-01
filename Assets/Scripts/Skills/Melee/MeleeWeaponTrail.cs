#define USE_INTERPOLATION

//
// By Anomalous Underdog, 2011
//
// Based on code made by Forest Johnson (Yoggy) and xyber
//

using UnityEngine;
using System.Collections.Generic;

public class MeleeWeaponTrail : MonoBehaviour
{
	[SerializeField]
	bool _emit = true;
	public bool emit { set{_emit = value;} }
	
	[SerializeField]
	float _emitTime = 0.00f;
	
	[SerializeField]
	Material _material;
	
	[SerializeField]
	float _lifeTime = 1.00f;
	
	[SerializeField]
	Color[] _colors;
	
	[SerializeField]
	float[] _sizes;
	
	[SerializeField]
	float _minVertexDistance = 0.10f;
	[SerializeField]
	float _maxVertexDistance = 10.00f;
	
	[SerializeField]
	float _maxAngle = 3.00f;
	
	[SerializeField]
	bool _autoDestruct = false;
	
	#if USE_INTERPOLATION
	[SerializeField]
	int subdivisions = 4;
	#endif
	
	[SerializeField]
	Transform _base;
	[SerializeField]
	Transform _tip;
	
	List<Point> _points = new List<Point>();
	#if USE_INTERPOLATION
	List<Point> _smoothedPoints = new List<Point>();
	#endif
	GameObject _trail;
	Mesh _trailMesh;
	Vector3 _lastPosition;
	Vector3 _lastCameraPosition1;
	Vector3 _lastCameraPosition2;
	bool _lastFrameEmit = true;
	
	// Point
	public class Point {
		public float timeCreated = 0.00f;
		public Vector3 basePosition;
		public Vector3 tipPosition;
		public bool lineBreak = false;
	}

	// Start
	void Start() {
		// Server doesn't draw trails
		if(uLink.Network.isServer) {
			enabled = false;
			return;
		}

		_lastPosition = transform.position;
		_trail = new GameObject("Trail");
		_trail.transform.parent = null;
		_trail.transform.position = Vector3.zero;
		_trail.transform.rotation = Quaternion.identity;
		_trail.transform.localScale = Vector3.one;
		_trail.AddComponent(typeof(MeshFilter));
		_trail.AddComponent(typeof(MeshRenderer));
		_trail.renderer.material = _material;
		
		_trailMesh = new Mesh();
		_trailMesh.name = name + "TrailMesh";
		_trail.GetComponent<MeshFilter>().mesh = _trailMesh;
	}

	// OnDisable
	void OnDisable() {
		if(_trail != null)
			Destroy(_trail);
	}

	// Update
	void Update() {
		if (_emit && _emitTime != 0)
		{
			_emitTime -= Time.deltaTime;
			if (_emitTime == 0) _emitTime = -1;
			if (_emitTime < 0) _emit = false;
		}
		
		if (!_emit && _points.Count == 0 && _autoDestruct)
		{
			Destroy(_trail);
			Destroy(gameObject);
		}
		
		// early out if there is no camera
		if (!Camera.main) return;
		
		// if we have moved enough, create a new vertex and make sure we rebuild the mesh
		if (_emit)
		{
			float theDistance = (_lastPosition - transform.position).magnitude;
			if (theDistance > _minVertexDistance)
			{
				bool make = false;
				if (_points.Count < 3)
				{
					make = true;
				}
				else
				{
					//Vector3 l1 = _points[_points.Count - 2].basePosition - _points[_points.Count - 3].basePosition;
					//Vector3 l2 = _points[_points.Count - 1].basePosition - _points[_points.Count - 2].basePosition;
					Vector3 l1 = _points[_points.Count - 2].tipPosition - _points[_points.Count - 3].tipPosition;
					Vector3 l2 = _points[_points.Count - 1].tipPosition - _points[_points.Count - 2].tipPosition;
					if (Vector3.Angle(l1, l2) > _maxAngle || theDistance > _maxVertexDistance) make = true;
				}
				
				if (make)
				{
					Point p = new Point();
					p.basePosition = _base.position;
					p.tipPosition = _tip.position;
					p.timeCreated = Time.time;
					_points.Add(p);
					_lastPosition = transform.position;
					
					
					#if USE_INTERPOLATION
					if (_points.Count == 1)
					{
						_smoothedPoints.Add(p);
					}
					else if (_points.Count > 1)
					{
						// add 1+subdivisions for every possible pair in the _points
						for (int n = 0; n < 1+subdivisions; ++n)
							_smoothedPoints.Add(p);
					}
					
					// we use 4 control points for the smoothing
					if (_points.Count >= 4)
					{
						Vector3[] tipPoints = new Vector3[4];
						tipPoints[0] = _points[_points.Count - 4].tipPosition;
						tipPoints[1] = _points[_points.Count - 3].tipPosition;
						tipPoints[2] = _points[_points.Count - 2].tipPosition;
						tipPoints[3] = _points[_points.Count - 1].tipPosition;
						
						//IEnumerable<Vector3> smoothTip = Interpolate.NewBezier(Interpolate.Ease(Interpolate.EaseType.Linear), tipPoints, subdivisions);
						IEnumerable<Vector3> smoothTip = Interpolate.NewCatmullRom(tipPoints, subdivisions, false);
						
						Vector3[] basePoints = new Vector3[4];
						basePoints[0] = _points[_points.Count - 4].basePosition;
						basePoints[1] = _points[_points.Count - 3].basePosition;
						basePoints[2] = _points[_points.Count - 2].basePosition;
						basePoints[3] = _points[_points.Count - 1].basePosition;
						
						//IEnumerable<Vector3> smoothBase = Interpolate.NewBezier(Interpolate.Ease(Interpolate.EaseType.Linear), basePoints, subdivisions);
						IEnumerable<Vector3> smoothBase = Interpolate.NewCatmullRom(basePoints, subdivisions, false);
						
						List<Vector3> smoothTipList = new List<Vector3>(smoothTip);
						List<Vector3> smoothBaseList = new List<Vector3>(smoothBase);
						
						float firstTime = _points[_points.Count - 4].timeCreated;
						float secondTime = _points[_points.Count - 1].timeCreated;
						
						//Debug.Log(" smoothTipList.Count: " + smoothTipList.Count);
						
						for (int n = 0; n < smoothTipList.Count; ++n)
						{
							
							int idx = _smoothedPoints.Count - (smoothTipList.Count-n);
							// there are moments when the _smoothedPoints are lesser
							// than what is required, when elements from it are removed
							if (idx > -1 && idx < _smoothedPoints.Count)
							{
								Point sp = new Point();
								sp.basePosition = smoothBaseList[n];
								sp.tipPosition = smoothTipList[n];
								sp.timeCreated = Mathf.Lerp(firstTime, secondTime, (float)n/smoothTipList.Count);
								_smoothedPoints[idx] = sp;
							}
							//else
							//{
							//	Debug.LogError(idx + "/" + _smoothedPoints.Count);
							//}
						}
					}
					#endif
				}
				else
				{
					_points[_points.Count - 1].basePosition = _base.position;
					_points[_points.Count - 1].tipPosition = _tip.position;
					//_points[_points.Count - 1].timeCreated = Time.time;
					
					#if USE_INTERPOLATION
					_smoothedPoints[_smoothedPoints.Count - 1].basePosition = _base.position;
					_smoothedPoints[_smoothedPoints.Count - 1].tipPosition = _tip.position;
					#endif
				}
			}
			else
			{
				if (_points.Count > 0)
				{
					_points[_points.Count - 1].basePosition = _base.position;
					_points[_points.Count - 1].tipPosition = _tip.position;
					//_points[_points.Count - 1].timeCreated = Time.time;
				}
				
				#if USE_INTERPOLATION
				if (_smoothedPoints.Count > 0)
				{
					_smoothedPoints[_smoothedPoints.Count - 1].basePosition = _base.position;
					_smoothedPoints[_smoothedPoints.Count - 1].tipPosition = _tip.position;
				}
				#endif
			}
		}
		
		if (!_emit && _lastFrameEmit && _points.Count > 0)
			_points[_points.Count - 1].lineBreak = true;
		
		_lastFrameEmit = _emit;
		
		
		
		
		List<Point> remove = new List<Point>();
		foreach (Point p in _points)
		{
			// cull old points first
			if (Time.time - p.timeCreated > _lifeTime)
			{
				remove.Add(p);
			}
		}
		foreach (Point p in remove)
		{
			_points.Remove(p);
		}
		
		#if USE_INTERPOLATION
		remove = new List<Point>();
		foreach (Point p in _smoothedPoints)
		{
			// cull old points first
			if (Time.time - p.timeCreated > _lifeTime)
			{
				remove.Add(p);
			}
		}
		foreach (Point p in remove)
		{
			_smoothedPoints.Remove(p);
		}
		#endif
		
		
		#if USE_INTERPOLATION
		List<Point> pointsToUse = _smoothedPoints;
		#else
		List<Point> pointsToUse = _points;
		#endif
		
		if (pointsToUse.Count > 1)
		{
			Vector3[] newVertices = new Vector3[pointsToUse.Count * 2];
			Vector2[] newUV = new Vector2[pointsToUse.Count * 2];
			int[] newTriangles = new int[(pointsToUse.Count - 1) * 6];
			Color[] newColors = new Color[pointsToUse.Count * 2];
			
			for (int n = 0; n < pointsToUse.Count; ++n)
			{
				Point p = pointsToUse[n];
				float time = (Time.time - p.timeCreated) / _lifeTime;
				
				Color color = Color.Lerp(Color.white, Color.clear, time);
				if (_colors != null && _colors.Length > 0)
				{
					float colorTime = time * (_colors.Length - 1);
					float min = Mathf.Floor(colorTime);
					float max = Mathf.Clamp(Mathf.Ceil(colorTime), 1, _colors.Length - 1);
					float lerp = Mathf.InverseLerp(min, max, colorTime);
					if (min >= _colors.Length) min = _colors.Length - 1; if (min < 0) min = 0;
					if (max >= _colors.Length) max = _colors.Length - 1; if (max < 0) max = 0;
					color = Color.Lerp(_colors[(int)min], _colors[(int)max], lerp);
				}
				
				float size = 0f;
				if (_sizes != null && _sizes.Length > 0)
				{
					float sizeTime = time * (_sizes.Length - 1);
					float min = Mathf.Floor(sizeTime);
					float max = Mathf.Clamp(Mathf.Ceil(sizeTime), 1, _sizes.Length - 1);
					float lerp = Mathf.InverseLerp(min, max, sizeTime);
					if (min >= _sizes.Length) min = _sizes.Length - 1; if (min < 0) min = 0;
					if (max >= _sizes.Length) max = _sizes.Length - 1; if (max < 0) max = 0;
					size = Mathf.Lerp(_sizes[(int)min], _sizes[(int)max], lerp);
				}
				
				Vector3 lineDirection = p.tipPosition - p.basePosition;
				
				newVertices[n * 2] = p.basePosition - (lineDirection * (size * 0.5f));
				newVertices[(n * 2) + 1] = p.tipPosition + (lineDirection * (size * 0.5f));
				
				newColors[n * 2] = newColors[(n * 2) + 1] = color;
				
				float uvRatio = (float)n/pointsToUse.Count;
				newUV[n * 2] = new Vector2(uvRatio, 0);
				newUV[(n * 2) + 1] = new Vector2(uvRatio, 1);
				
				if (n > 0 /*&& !pointsToUse[n - 1].lineBreak*/)
				{
					newTriangles[(n - 1) * 6] = (n * 2) - 2;
					newTriangles[((n - 1) * 6) + 1] = (n * 2) - 1;
					newTriangles[((n - 1) * 6) + 2] = n * 2;
					
					newTriangles[((n - 1) * 6) + 3] = (n * 2) + 1;
					newTriangles[((n - 1) * 6) + 4] = n * 2;
					newTriangles[((n - 1) * 6) + 5] = (n * 2) - 1;
				}
			}
			
			_trailMesh.Clear();
			_trailMesh.vertices = newVertices;
			_trailMesh.colors = newColors;
			_trailMesh.uv = newUV;
			_trailMesh.triangles = newTriangles;
			
		}else{
			_trailMesh.Clear();
			
		}
	}
}
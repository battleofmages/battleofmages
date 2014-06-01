using UnityEngine;
using System.Collections;

public class SetVertexColor : MonoBehaviour {
	public Vector4 vertexColor;
	
	void Start() {
		Color vertexColorAsColor = new Color(vertexColor.x, vertexColor.y, vertexColor.z, vertexColor.w);
		Mesh mesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
		Vector3[] vertices = mesh.vertices;
		Color[] colors = new Color[vertices.Length];
		
		int i = 0;
		while(i < vertices.Length) {
			colors[i] = vertexColorAsColor;
			i++;
		}
		
		mesh.colors = colors;
	}
}
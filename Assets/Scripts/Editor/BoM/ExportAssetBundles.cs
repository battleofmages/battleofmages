using UnityEngine;
using UnityEditor;

public class ExportAssetBundles : MonoBehaviour {
	// Build a streamed unity3d file. This contain one scene that can be downloaded
	// on demand and loaded once its asset bundle has been loaded.
	[MenuItem("Battle of Mages/Map/Export scene as asset bundle")]
	public static void ExportScene() {
		var levels = new string[] {
			EditorApplication.currentScene
		};

		var path = EditorApplication.currentScene.Split('/');
		var sceneName = path[path.Length - 1].Replace(".unity", "");
		var targetPath = "Bundles/" + sceneName + ".unity3d";

		Debug.Log (levels[0]);
		Debug.Log (targetPath);

		BuildPipeline.BuildStreamedSceneAssetBundle(levels, targetPath, BuildTarget.StandaloneWindows64);
	}
}

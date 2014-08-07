using UnityEngine;
using UnityEditor;

public class ExportAssetBundles : MonoBehaviour {
	// Build a streamed unity3d file. This contain one scene that can be downloaded
	// on demand and loaded once its asset bundle has been loaded.

	[MenuItem("Battle of Mages/Map/Export scene as asset bundle [All platforms]")]
	public static void ExportScene() {
		ExportSceneWin64();
		ExportSceneLinux();
	}

	[MenuItem("Battle of Mages/Map/Export scene as asset bundle [Win64]")]
	public static void ExportSceneWin64() {
		var levels = new string[] {
			EditorApplication.currentScene
		};

		var path = EditorApplication.currentScene.Split('/');
		var sceneName = path[path.Length - 1].Replace(".unity", "");
		var targetPath = "Bundles/windows/" + sceneName + ".unity3d";

		BuildPipeline.BuildStreamedSceneAssetBundle(levels, targetPath, BuildTarget.StandaloneWindows64);
	}

	[MenuItem("Battle of Mages/Map/Export scene as asset bundle [Linux]")]
	public static void ExportSceneLinux() {
		var levels = new string[] {
			EditorApplication.currentScene
		};
		
		var path = EditorApplication.currentScene.Split('/');
		var sceneName = path[path.Length - 1].Replace(".unity", "");
		var targetPath = "Bundles/linux/" + sceneName + ".unity3d";
		
		BuildPipeline.BuildStreamedSceneAssetBundle(levels, targetPath, BuildTarget.StandaloneLinuxUniversal);
	}
}

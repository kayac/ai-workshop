using UnityEngine;
using UnityEditor;
using System.Collections;

public class CustomShortcutKey : ScriptableObject {

	[MenuItem("Tools/Shortcut/Active-Inactive &a")]
	public static void SwitchActive () {
		GameObject[] gameObjects = Selection.gameObjects;
		if (gameObjects == null) {
			// Selection.gameObjects is not null.
		} else {
			foreach (GameObject myGameObject in gameObjects) {
				myGameObject.SetActive(!myGameObject.activeSelf);
			}
		}
	}


	/*
	[MenuItem("Tools/Create/Folder %#k")]
	public static void CreateFolder () {

		Object targetObj = null;

		foreach (Object obj in Selection.objects)
		{
			targetObj = obj;
			break;
		}

		if (targetObj == null) return;

		string targetPath = AssetDatabase.GetAssetPath(targetObj);

		Debug.Log("targetPath:" + targetPath);
	}
	*/
}
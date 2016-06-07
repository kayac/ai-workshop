using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;

/// <summary>
/// 新規フォルダ作成時に.gitkeepを自動作成
/// </summary>
public class GitkeepMaker : AssetPostprocessor
{
  private readonly static string folderKeeperName = ".gitkeep";

	public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetsPath)
	{
		if (!Directory.Exists(".git")) return;

		foreach (string path in importedAssets) {
			if (Directory.Exists(path)) {
				string folderKeeperPath = path + "/" + folderKeeperName;
				if ( !File.Exists(folderKeeperPath) ) {
					File.Create(folderKeeperPath).Close();
					Debug.Log(folderKeeperPath + " created");
				}
			}
		}
	}
}
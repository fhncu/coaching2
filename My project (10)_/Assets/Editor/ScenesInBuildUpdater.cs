//  ScenesInBuildUpdater.cs
//  http://kan-kikuchi.hatenablog.com/entry/ScenesInBuildUpdater
//
//  Created by kan.kikuchi on 2016.05.31.

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ビルドするシーンを自動で設定するクラス
/// </summary>
public class ScenesInBuildUpdater : AssetPostprocessor
{

    //ビルドに設定するシーンが入ったディレクトリへのパス
    private const string BUILD_DIRECTORY_PATH = "Assets/Scenes/Build";

    //=================================================================================
    //検知
    //=================================================================================

    //Assets内のファイルが更新、削除などされたときに呼ばれる
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {

        List<string[]> assetsList = new List<string[]>(){
      importedAssets, deletedAssets, movedAssets, movedFromAssetPaths
    };

        List<string> targetDirectoryNameList = new List<string>(){
      BUILD_DIRECTORY_PATH,
    };

        //変更されたファイルに指定ディレクトリ内のものが含まれている時はビルドするシーンを更新
        if (ExistsDirectoryInAssets(assetsList, targetDirectoryNameList))
        {
            UpdateScenesInBuild();
        }

        EditorBuildSettingsScene scene = new EditorBuildSettingsScene("", true);
    }

    //入力されたassetsの中に、ディレクトリのパスがdirectoryNameの物はあるか
    protected static bool ExistsDirectoryInAssets(List<string[]> assetsList, List<string> targetDirectoryNameList)
    {

        return assetsList
          .Any(assets => assets                                       //入力されたassetsListに以下の条件を満たすか要素が含まれているか判定
           .Select(asset => System.IO.Path.GetDirectoryName(asset))   //assetsに含まれているファイルのディレクトリ名だけをリストにして取得
           .Intersect(targetDirectoryNameList)                         //上記のリストと入力されたディレクトリ名のリストの一致している物のリストを取得
           .Count() > 0);                                              //一致している物があるか
    }

    //=================================================================================
    //作成
    //=================================================================================

    //ビルドするシーンの更新
    [MenuItem("Tools/Update/Scenes In Build")]
    private static void UpdateScenesInBuild()
    {

        //Sceneファイルを全て取得、名前とパスで辞書作成
        List<string> pathList = new List<string>();
        string firstScenePath = "";

        foreach (var guid in AssetDatabase.FindAssets("t:Scene"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);

            //指定ディレクトリ以外に入っているシーンはスルー
            if (!path.Contains(BUILD_DIRECTORY_PATH))
            {
                continue;
            }

            //シーン名が被っている時はエラーを表示
            if (pathList.Contains(sceneName))
            {
                Debug.LogError(sceneName + " というシーン名が重複しています");
            }
            //親ディレクトリがFirstならば最初のシーンに設定(するためにパスをfirstScenePathに入れる)
            else if (System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(path)) == "First")
            {
                //二つ以上入ってる場合はエラー
                if (!string.IsNullOrEmpty(firstScenePath))
                {
                    Debug.LogError("Firstディレクトリに複数のシーンが入っています");
                }
                firstScenePath = path;
            }
            //パスをリストに追加
            else
            {
                pathList.Add(path);
            }
        }

        //追加するシーンのリスト作成、追加
        List<EditorBuildSettingsScene> sceneList = new List<EditorBuildSettingsScene>();

        if (!string.IsNullOrEmpty(firstScenePath))
        {
            sceneList.Add(new EditorBuildSettingsScene(firstScenePath, true));
        }

        foreach (string path in pathList)
        {
            sceneList.Add(new EditorBuildSettingsScene(path, true));
        }

        EditorBuildSettings.scenes = sceneList.ToArray();
    }

}
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.Collections.Generic;
using System.IO;

public class SceneBuilder : MonoBehaviour
{
    [MenuItem("Custom/Build Scenes")]
    public static void BuildScenes()
    {
        // ビルド設定をリセット
        EditorBuildSettings.scenes = new EditorBuildSettingsScene[0];

        // シーンが格納されているフォルダのパスを指定
        string sceneFolderPath = "Assets/TwoBitMachines/FlareEngine/Demo/Scenes/New Folder 1/New Folder 1";

        // フォルダ内のすべてのシーンファイルを取得
        string[] sceneFiles = Directory.GetFiles(sceneFolderPath, "*.unity", SearchOption.AllDirectories);

        // シーンファイルをビルド設定に追加
        foreach (string sceneFile in sceneFiles)
        {
            EditorBuildSettingsScene buildScene = new EditorBuildSettingsScene(sceneFile, true);
            EditorBuildSettings.scenes = AddSceneToBuildSettings(EditorBuildSettings.scenes, buildScene);
        }

        //// ビルドを実行
        //BuildReport buildReport = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "Build/MyGame.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

        //// ビルドが成功したかどうかを確認
        //if (buildReport.summary.result == BuildResult.Succeeded)
        //{
        //    Debug.Log("Build succeeded");
        //}
        //else
        //{
        //    Debug.LogError("Build failed");
        //}
    }

    // シーンをビルド設定に追加するヘルパー関数
    private static EditorBuildSettingsScene[] AddSceneToBuildSettings(EditorBuildSettingsScene[] scenes, EditorBuildSettingsScene sceneToAdd)
    {
        List<EditorBuildSettingsScene> sceneList = new List<EditorBuildSettingsScene>(scenes);

        // すでに追加されているかどうかを確認
        bool sceneExists = false;
        for (int i = 0; i < sceneList.Count; i++)
        {
            if (sceneList[i].path == sceneToAdd.path)
            {
                sceneExists = true;
                break;
            }
        }

        // 追加されていなければリストに追加
        if (!sceneExists)
        {
            sceneList.Add(sceneToAdd);
        }

        return sceneList.ToArray();
    }
}

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
        // �r���h�ݒ�����Z�b�g
        EditorBuildSettings.scenes = new EditorBuildSettingsScene[0];

        // �V�[�����i�[����Ă���t�H���_�̃p�X���w��
        string sceneFolderPath = "Assets/TwoBitMachines/FlareEngine/Demo/Scenes/New Folder 1/New Folder 1";

        // �t�H���_���̂��ׂẴV�[���t�@�C�����擾
        string[] sceneFiles = Directory.GetFiles(sceneFolderPath, "*.unity", SearchOption.AllDirectories);

        // �V�[���t�@�C�����r���h�ݒ�ɒǉ�
        foreach (string sceneFile in sceneFiles)
        {
            EditorBuildSettingsScene buildScene = new EditorBuildSettingsScene(sceneFile, true);
            EditorBuildSettings.scenes = AddSceneToBuildSettings(EditorBuildSettings.scenes, buildScene);
        }

        //// �r���h�����s
        //BuildReport buildReport = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "Build/MyGame.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

        //// �r���h�������������ǂ������m�F
        //if (buildReport.summary.result == BuildResult.Succeeded)
        //{
        //    Debug.Log("Build succeeded");
        //}
        //else
        //{
        //    Debug.LogError("Build failed");
        //}
    }

    // �V�[�����r���h�ݒ�ɒǉ�����w���p�[�֐�
    private static EditorBuildSettingsScene[] AddSceneToBuildSettings(EditorBuildSettingsScene[] scenes, EditorBuildSettingsScene sceneToAdd)
    {
        List<EditorBuildSettingsScene> sceneList = new List<EditorBuildSettingsScene>(scenes);

        // ���łɒǉ�����Ă��邩�ǂ������m�F
        bool sceneExists = false;
        for (int i = 0; i < sceneList.Count; i++)
        {
            if (sceneList[i].path == sceneToAdd.path)
            {
                sceneExists = true;
                break;
            }
        }

        // �ǉ�����Ă��Ȃ���΃��X�g�ɒǉ�
        if (!sceneExists)
        {
            sceneList.Add(sceneToAdd);
        }

        return sceneList.ToArray();
    }
}

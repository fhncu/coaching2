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
/// �r���h����V�[���������Őݒ肷��N���X
/// </summary>
public class ScenesInBuildUpdater : AssetPostprocessor
{

    //�r���h�ɐݒ肷��V�[�����������f�B���N�g���ւ̃p�X
    private const string BUILD_DIRECTORY_PATH = "Assets/Scenes/Build";

    //=================================================================================
    //���m
    //=================================================================================

    //Assets���̃t�@�C�����X�V�A�폜�Ȃǂ��ꂽ�Ƃ��ɌĂ΂��
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {

        List<string[]> assetsList = new List<string[]>(){
      importedAssets, deletedAssets, movedAssets, movedFromAssetPaths
    };

        List<string> targetDirectoryNameList = new List<string>(){
      BUILD_DIRECTORY_PATH,
    };

        //�ύX���ꂽ�t�@�C���Ɏw��f�B���N�g�����̂��̂��܂܂�Ă��鎞�̓r���h����V�[�����X�V
        if (ExistsDirectoryInAssets(assetsList, targetDirectoryNameList))
        {
            UpdateScenesInBuild();
        }

        EditorBuildSettingsScene scene = new EditorBuildSettingsScene("", true);
    }

    //���͂��ꂽassets�̒��ɁA�f�B���N�g���̃p�X��directoryName�̕��͂��邩
    protected static bool ExistsDirectoryInAssets(List<string[]> assetsList, List<string> targetDirectoryNameList)
    {

        return assetsList
          .Any(assets => assets                                       //���͂��ꂽassetsList�Ɉȉ��̏����𖞂������v�f���܂܂�Ă��邩����
           .Select(asset => System.IO.Path.GetDirectoryName(asset))   //assets�Ɋ܂܂�Ă���t�@�C���̃f�B���N�g�������������X�g�ɂ��Ď擾
           .Intersect(targetDirectoryNameList)                         //��L�̃��X�g�Ɠ��͂��ꂽ�f�B���N�g�����̃��X�g�̈�v���Ă��镨�̃��X�g���擾
           .Count() > 0);                                              //��v���Ă��镨�����邩
    }

    //=================================================================================
    //�쐬
    //=================================================================================

    //�r���h����V�[���̍X�V
    [MenuItem("Tools/Update/Scenes In Build")]
    private static void UpdateScenesInBuild()
    {

        //Scene�t�@�C����S�Ď擾�A���O�ƃp�X�Ŏ����쐬
        List<string> pathList = new List<string>();
        string firstScenePath = "";

        foreach (var guid in AssetDatabase.FindAssets("t:Scene"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);

            //�w��f�B���N�g���ȊO�ɓ����Ă���V�[���̓X���[
            if (!path.Contains(BUILD_DIRECTORY_PATH))
            {
                continue;
            }

            //�V�[����������Ă��鎞�̓G���[��\��
            if (pathList.Contains(sceneName))
            {
                Debug.LogError(sceneName + " �Ƃ����V�[�������d�����Ă��܂�");
            }
            //�e�f�B���N�g����First�Ȃ�΍ŏ��̃V�[���ɐݒ�(���邽�߂Ƀp�X��firstScenePath�ɓ����)
            else if (System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(path)) == "First")
            {
                //��ȏ�����Ă�ꍇ�̓G���[
                if (!string.IsNullOrEmpty(firstScenePath))
                {
                    Debug.LogError("First�f�B���N�g���ɕ����̃V�[���������Ă��܂�");
                }
                firstScenePath = path;
            }
            //�p�X�����X�g�ɒǉ�
            else
            {
                pathList.Add(path);
            }
        }

        //�ǉ�����V�[���̃��X�g�쐬�A�ǉ�
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
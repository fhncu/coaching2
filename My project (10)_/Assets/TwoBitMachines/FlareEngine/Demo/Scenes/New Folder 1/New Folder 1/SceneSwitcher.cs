using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public int inputNumber;


    // �g�p����V�[���̖��O�����X�g�Ɋi�[���܂�
    public List<string> sceneNames;
    public List<int> sceneNumbers;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("Player"))
        {
            // ���݂̃V�[�����擾���܂�
            string currentScene = SceneManager.GetActiveScene().name;

            // �����_���Ɏ��̃V�[����I�����܂�
            string nextScene = GetRandomSceneNumber();

            // �����_���ɑI�������V�[�������݂̃V�[���Ɠ����ꍇ�A�ēx�ʂ̃V�[����I�����܂�
            while (nextScene == currentScene)
            {
                nextScene = GetRandomSceneNumber();
            }

            // ���̃V�[���ɐ؂�ւ��܂�
            SceneManager.LoadScene(nextScene);
        }
    }

    private string GetRandomScene()
    {
        // �V�[�����X�g����̏ꍇ�́A���݂̃V�[����Ԃ��܂�
        if (sceneNames.Count == 0)
        {
            return SceneManager.GetActiveScene().name;
        }

        // �����_���ȃC���f�b�N�X�𐶐����܂�
        int randomIndex = Random.Range(0, sceneNames.Count);

        // �����_���ɑI�������V�[�����擾���A���X�g����폜���܂�
        string randomScene = sceneNames[randomIndex];
        sceneNames.RemoveAt(randomIndex);

        return randomScene;
    }


    public List<int> numberList;

    private void OnValidate()
    {
        GeneratesceneNumbers();
    }

    private void GeneratesceneNumbers()
    {
        sceneNumbers.Clear();
        for (int i = 1; i <= inputNumber; i++)
        {
            sceneNumbers.Add(i);
        }
    }

    private string GetRandomSceneNumber()
    {
        // �V�[�����X�g����̏ꍇ�́A���݂̃V�[����Ԃ��܂�
        if (sceneNumbers.Count == 0)
        {
            return SceneManager.GetActiveScene().name;
        }

        // �����_���ȃC���f�b�N�X�𐶐����܂�
        int randomIndex = Random.Range(0, sceneNames.Count);

        // �����_���ɑI�������V�[�����擾���A���X�g����폜���܂�
        int randomScene = sceneNumbers[randomIndex];
        sceneNames.RemoveAt(randomIndex);

        return "Vol "+randomScene ;
    }

    public void GetCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
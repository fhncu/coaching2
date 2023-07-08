using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public int inputNumber;


    // 使用するシーンの名前をリストに格納します
    public List<string> sceneNames;
    public List<int> sceneNumbers;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("Player"))
        {
            // 現在のシーンを取得します
            string currentScene = SceneManager.GetActiveScene().name;

            // ランダムに次のシーンを選択します
            string nextScene = GetRandomSceneNumber();

            // ランダムに選択したシーンが現在のシーンと同じ場合、再度別のシーンを選択します
            while (nextScene == currentScene)
            {
                nextScene = GetRandomSceneNumber();
            }

            // 次のシーンに切り替えます
            SceneManager.LoadScene(nextScene);
        }
    }

    private string GetRandomScene()
    {
        // シーンリストが空の場合は、現在のシーンを返します
        if (sceneNames.Count == 0)
        {
            return SceneManager.GetActiveScene().name;
        }

        // ランダムなインデックスを生成します
        int randomIndex = Random.Range(0, sceneNames.Count);

        // ランダムに選択したシーンを取得し、リストから削除します
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
        // シーンリストが空の場合は、現在のシーンを返します
        if (sceneNumbers.Count == 0)
        {
            return SceneManager.GetActiveScene().name;
        }

        // ランダムなインデックスを生成します
        int randomIndex = Random.Range(0, sceneNames.Count);

        // ランダムに選択したシーンを取得し、リストから削除します
        int randomScene = sceneNumbers[randomIndex];
        sceneNames.RemoveAt(randomIndex);

        return "Vol "+randomScene ;
    }

    public void GetCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
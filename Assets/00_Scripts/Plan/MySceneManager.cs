using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static MySceneManager instance;

    [HideInInspector]
    public string nextScene; // 중간씬이 참고할 다음 씬

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환에도 파괴되지 않음
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 씬 전환 호출
    public void LoadScene(SceneNmae targetScene)
    {
        nextScene = targetScene.ToString();
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNmae.SceneChange.ToString());
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChange : MonoBehaviour
{
    public float loadingDelay = 2f;

    void Start()
    {
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(loadingDelay);

        // MySceneManager에서 지정한 다음 씬으로 이동
        SceneManager.LoadScene(MySceneManager.instance.nextScene);
    }
}

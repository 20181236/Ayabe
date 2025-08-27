using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static LoadSceneManager instance;

    public SceneNmae nextScene;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // 버튼 클릭용 Wrapper
    public void OnClickMoveToInGame()
    {
        // enum을 ToString()으로 변환해서 바로 비동기 로드
        StartCoroutine(LoadSceneAsync(SceneNmae.InGame.ToString()));
    }

    public void OnClickMoveToPlan()
    {
        StartCoroutine(LoadSceneAsync(SceneNmae.Plan.ToString()));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        Debug.Log("비동기 로딩 시작: " + sceneName);
        
        float time = 0f;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false; // 로딩이 완료되도 바로 활성화하지 않음

        while (!asyncOperation.isDone)
        {
            time += Time.deltaTime;
            //Debug.Log("로딩 진행률: " + asyncOperation.progress); // 0 ~ 0.9까지 올라감
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
           // Debug.Log("로딩 진행률: " + (progress * 100) + "%");

            // 최소 2초 대기 후 씬 활성화
            if (time > 2f)
            {
                asyncOperation.allowSceneActivation = true;
                Debug.Log("로딩완료");
            }

            yield return null;
        }

        Debug.Log(sceneName + " 로딩 완료 및 씬 활성화");
    }
}

    //private IEnumerator LoadSceneAsync(string sceneName)
    //{
    //    Debug.Log("비동기 로딩 시작: " + sceneName);

    //    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
    //    asyncLoad.allowSceneActivation = true; // 필요하면 false로 두고 UI 완료 후 활성화 가능

    //    while (!asyncLoad.isDone)
    //    {
    //        float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
    //        Debug.Log("로딩 진행률: " + (progress * 100) + "%");

    //        // TODO: 로딩바 UI 업데이트 가능

    //        yield return null;
    //    }

    //    Debug.Log(sceneName + " 로딩 완료");
    //}
    //public void OnClickMoveScene(SceneNmae targetScene)
    //{
    //    StartCoroutine(LoadSceneAsync(targetScene));
    //}

    //private IEnumerator LoadSceneAsync(SceneNmae targetScene)
    //{
    //    yield return null; // 바로 시작해도 되고, 필요시 딜레이 가능

    //    string nextSceneName = targetScene.ToString();
    //    Debug.Log("비동기로 로딩 시작: " + nextSceneName);

    //    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);

    //    // 필요하면 로딩 중에 allowSceneActivation 제어
    //    asyncLoad.allowSceneActivation = true;

    //    while (!asyncLoad.isDone)
    //    {
    //        float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
    //        Debug.Log("로딩 진행률: " + (progress * 100) + "%");
    //        yield return null;
    //    }
    //}

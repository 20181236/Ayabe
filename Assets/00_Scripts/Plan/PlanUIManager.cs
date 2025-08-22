using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanUIManager : MonoBehaviour
{
    public static PlanUIManager instance;

     public Transform popupRoot;         // 팝업들이 들어갈 부모 오브젝트

    private Stack<UIBase> popupStack = new Stack<UIBase>();
    //private Dictionary<string, UIBase> popupDictionary = new Dictionary<string, UIBase>();
    private Dictionary<PopupList, UIBase> popupDictionary = new Dictionary<PopupList, UIBase>();


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    // 팝업 열기
    //public T ShowPopup<T>() where T : UIBase
    //{
    //    string popupName = typeof(T).Name;

    //    if (!popupDictionary.ContainsKey(popupName))
    //    {
    //        GameObject prefab = Resources.Load<GameObject>($"UI/Popups/{popupName}");
    //        GameObject popupObj = Instantiate(prefab, popupRoot);
    //        UIBase popup = popupObj.GetComponent<UIBase>();
    //        popupDictionary[popupName] = popup;
    //    }

    //    UIBase popupToShow = popupDictionary[popupName];
    //    popupToShow.Open();
    //    popupStack.Push(popupToShow);

    //    return popupToShow as T;
    //}
    public T ShowPopup<T>(PopupList popupType) where T : UIBase
    {
        if (!popupDictionary.ContainsKey(popupType))
        {
            string path = ResourcePaths.GetPopupPath(popupType); // "Plan/SetPlayablePopup"
            GameObject prefab = Resources.Load<GameObject>(path);

            if (prefab == null)
            {
                Debug.LogError($"팝업 프리팹을 찾을 수 없습니다: {path}");
                return null;
            }

            GameObject popupObj = Instantiate(prefab, popupRoot);
            UIBase popup = popupObj.GetComponent<UIBase>();
            popupDictionary[popupType] = popup;
        }

        UIBase popupToShow = popupDictionary[popupType];
        popupToShow.Open();
        popupStack.Push(popupToShow);

        return popupToShow as T;
    }

    // 최상단 팝업 닫기
    public void ClosePopup()
    {
        if (popupStack.Count == 0) 
            return;

        UIBase topPopup = popupStack.Pop();
        topPopup.Close();
    }

    // 현재 가장 위에 있는 팝업 가져오기
    public UIBase GetTopPopup()
    {
        return popupStack.Count > 0 ? popupStack.Peek() : null;
    }

    // 모든 팝업 닫기
    public void CloseAllPopups()
    {
        while (popupStack.Count > 0)
        {
            popupStack.Pop().Close();
        }
    }
}

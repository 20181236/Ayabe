using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class UIBase : MonoBehaviour
{
    // UI 열기
    public virtual void Open()
    {
        gameObject.SetActive(true);
        OnOpen();
    }

    // UI 닫기
    public virtual void Close()
    {
        OnClose();
        gameObject.SetActive(false);
    }

    // 자식 클래스가 확장하는 부분
    protected virtual void OnOpen() { }
    protected virtual void OnClose() { }

    // 뒤로가기 버튼 등 처리 (스택 연동 시)
    public virtual void OnBackPressed()
    {
        Close();
    }
}

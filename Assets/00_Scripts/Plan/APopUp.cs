using UnityEngine;

public class APopup : UIBase
{
    protected override void OnOpen()
    {
        Debug.Log("A 팝업 열림 - 데이터 초기화 및 UI 갱신");
    }

    protected override void OnClose()
    {
        Debug.Log("A 팝업 닫힘 - 리소스 정리");
    }

    public override void OnBackPressed()
    {
        Debug.Log("A 팝업에서 뒤로가기 누름 → 닫기 실행");
        Close();
    }
}

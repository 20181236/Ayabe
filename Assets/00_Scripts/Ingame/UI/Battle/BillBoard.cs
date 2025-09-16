using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]    // 편집 모드에서도 회전이 갱신
public class BillBoard : MonoBehaviour
{
    private Camera _camera;

    void Awake()
    {
        //씬에 MainCamera 태그 달린 카메라 찾기
        if (Camera.main != null) _camera = Camera.main;
    }

    void LateUpdate()
    {
        if (_camera == null)    
            return;

        transform.LookAt(transform.position + _camera.transform.rotation * Vector3.forward,
                 _camera.transform.rotation * Vector3.up);

        ////// 카메라의 위치를 기준으로 하되, 높이(y)는 오브젝트의 높이를 그대로 사용합니다.
        ////Vector3 targetPos = new Vector3(_camera.transform.position.x, transform.position.y, _camera.transform.position.z);

        ////// 해당 방향을 바라보도록 회전시킵니다.
        ////transform.LookAt(targetPos);

        //// 1) 월드-스페이스 오브젝트의 앞면(+Z축)을 카메라가 있는 방향으로 향하게
        //Vector3 dir = (_camera.transform.position - transform.position).normalized;
        //// 2) 쳐다보게 회전: forward 가 카메라 방향이 되도록
        //transform.rotation = Quaternion.LookRotation(dir);

        //// Y축 회전만 원하면
        //var lookPos = _camera.transform.position;
        //lookPos.y = transform.position.y;
        //transform.LookAt(lookPos);
    }
}
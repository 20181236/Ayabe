using UnityEngine;
using UnityEngine.EventSystems;

public class CircleRangeController : MonoBehaviour
{
    public Transform circleRange; // CircleRange가 붙은 GameObject
    private Plane groundPlane;
    private bool isPlacing = false;

    void Start()
    {
        groundPlane = new Plane(Vector3.up, Vector3.zero);
        circleRange.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (RayToGround(out Vector3 start))
            {
                isPlacing = true;
                circleRange.gameObject.SetActive(true);
                circleRange.position = start;
            }
        }

        if (Input.GetMouseButton(0) && isPlacing)
        {
            if (RayToGround(out Vector3 pos))
            {
                circleRange.position = pos;
            }
        }

        if (Input.GetMouseButtonUp(0) && isPlacing)
        {
            isPlacing = false;

            // 여기에 스킬 발동 코드 넣기
            // SkillManager.Instance.CastAt(circleRange.position);
        }
    }

    bool RayToGround(out Vector3 point)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (groundPlane.Raycast(ray, out float dist))
        {
            point = ray.GetPoint(dist);
            return true;
        }

        point = Vector3.zero;
        return false;
    }
}

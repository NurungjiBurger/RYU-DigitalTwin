using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // 초기에 카메라가 따라다닐 타겟 설정
    public GameObject initTarget;

    // 카메라 오브젝트
    public Camera mainCamera;

    // 움직이는 속도를 제어할 변수들
    public float moveSpeed = 10f;
    public float lookSpeed = 2f;
    public float focusSpeed = 2f;
    // 카메라의 위치 범위
    public Vector2 xRange = new Vector2(7f, 42f);
    public Vector2 zRange = new Vector2(4f, 40f);

    // 초기 X 사용자가 선택한 타겟
    public Transform target;

    // 카메라가 떨어진 거리
    public Vector3 offset;

    // 따라다닐 타겟 설정 함수
    public void SettingTarget(GameObject obj)
    {
        target = obj.GetComponent<Transform>();
        UpdateCameraPosition();
    }

    private void Start()
    {
        SettingTarget(initTarget);
    }

    void Update()
    {
        if (target == null)
        {
            return; // 타겟이 설정되지 않았다면 업데이트하지 않음
        }

        UpdateCameraPosition();
    }

    // 카메라 위치 업데이트
    private void UpdateCameraPosition()
    {
        if (target == null)
        {
            return; // 타겟이 설정되지 않았다면 위치 업데이트하지 않음
        }

        // 카메라의 위치를 대상에서 오프셋만큼 떨어진 곳에 설정
        Vector3 desiredPosition = target.position - target.forward * Mathf.Abs(offset.z) + Vector3.up * offset.y;

        // 카메라 위치를 범위 내로 제한
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, xRange.x, xRange.y);
        desiredPosition.z = Mathf.Clamp(desiredPosition.z, zRange.x, zRange.y);

        // 카메라가 부드럽게 해당 좌표로 이동할 수 있도록 함.
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, desiredPosition, Time.deltaTime * focusSpeed);

        // 카메라가 항상 대상을 바라보도록 설정합니다.
        mainCamera.transform.LookAt(target.position);
    }
   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject initTarget;

    public Camera mainCamera;
    public float moveSpeed = 10f;
    public float lookSpeed = 2f;
    public float focusSpeed = 2f;
    public Transform target;
    public Vector3 offset;

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

        // 카메라 위치 업데이트
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        if (target == null)
        {
            return; // 타겟이 설정되지 않았다면 위치 업데이트하지 않음
        }

        // 카메라의 위치를 대상의 후방과 위쪽으로 설정합니다.
        Vector3 desiredPosition = target.position - target.forward * Mathf.Abs(offset.z) + Vector3.up * offset.y;
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, desiredPosition, Time.deltaTime * focusSpeed);

        // 카메라가 항상 대상을 바라보도록 설정합니다.
        mainCamera.transform.LookAt(target.position);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target; // 따라다닐 대상 (TestMover 오브젝트)
    public Vector3 offset = new Vector3(0, 5, -10); // 카메라와 대상 사이의 거리

    private void Start()
    {
        // 초기 카메라 위치를 설정합니다.
        if (target != null)
        {
            // 카메라를 대상의 후방으로 이동합니다.
            UpdateCameraPosition();
        }
    }

    private void LateUpdate()
    {
        // 대상의 회전에 따라 카메라 위치 업데이트
        if (target != null)
        {
            UpdateCameraPosition();
        }
    }

    private void UpdateCameraPosition()
    {
        // 카메라의 위치를 대상의 후방으로 설정합니다.
        transform.position = target.position - target.forward * Mathf.Abs(offset.z) + Vector3.up * offset.y;
        // 카메라가 항상 대상을 바라보도록 설정합니다.
        transform.LookAt(target);
    }
}

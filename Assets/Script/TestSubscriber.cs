using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Std;
using Unity.VisualScripting;
using System;

public class TestSubscriber : UnitySubscriber<RosSharp.RosBridgeClient.MessageTypes.Std.String>
{
    public float moveSpeed = 1.0f; // 이동 속도
    public float rotationSpeed = 1.0f; // 회전 속도
    public float moveThreshold = 0.1f; // 위치 도달 기준 거리
    public float rotationThreshold = 0.1f; // 회전 도달 기준 각도

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private bool isMoving = false;
    private bool isRotating = false;

    public float minX = 8.0f; // x축 최소 범위
    public float maxX = 40.0f; // x축 최대 범위
    public float minZ = 5.0f; // z축 최소 범위
    public float maxZ = 62.0f; // z축 최대 범위

    protected override void ReceiveMessage(RosSharp.RosBridgeClient.MessageTypes.Std.String message)
    {
        Debug.Log("Message Received: " + message.data); // 콘솔에 메시지 출력

        try
        {
            // 메시지 데이터를 파싱
            string[] parts = message.data.Split(',');
            if (parts.Length != 7) // 총 7개의 값이 있어야 합니다.
            {
                Debug.LogError("Invalid message format: Expected 7 parts separated by commas.");
                return;
            }

            // 위치 데이터 파싱
            if (!float.TryParse(parts[0], out float x) ||
                !float.TryParse(parts[1], out float y) ||
                !float.TryParse(parts[2], out float z))
            {
                Debug.LogError("Invalid position data: Could not parse float values.");
                return;
            }
            targetPosition = new Vector3(x, y, z);

            // 회전 데이터 파싱
            if (!float.TryParse(parts[3], out float xRot) ||
                !float.TryParse(parts[4], out float yRot) ||
                !float.TryParse(parts[5], out float zRot) ||
                !float.TryParse(parts[6], out float wRot))
            {
                Debug.LogError("Invalid rotation data: Could not parse float values.");
                return;
            }
            targetRotation = new Quaternion(xRot, yRot, zRot, wRot);

            // 이동과 회전 시작
            isMoving = true;
            isRotating = true;

            Debug.Log($"Target Position: {targetPosition}");
            Debug.Log($"Target Rotation: {targetRotation}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception occurred while parsing message: {ex.Message}");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            // 현재 위치에서 목표 위치로 이동
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * UnityEngine.Time.deltaTime);

            // 위치 도달 시 이동 멈추기
            if (Vector3.Distance(transform.position, targetPosition) < moveThreshold)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }

        if (isRotating)
        {
            // 현재 회전에서 목표 회전으로 회전
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * UnityEngine.Time.deltaTime);

            // 회전 도달 시 회전 멈추기
            if (Quaternion.Angle(transform.rotation, targetRotation) < rotationThreshold)
            {
                transform.rotation = targetRotation;
                isRotating = false;
            }
        }

        // 위치 제한 적용
        ClampPosition();
    }

    private void ClampPosition()
    {
        // 현재 위치
        Vector3 pos = transform.position;

        // x축 범위 제한
        pos.x = Mathf.Clamp(pos.x, minX, maxX);

        // z축 범위 제한
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

        // 제한된 위치로 설정
        transform.position = pos;
    }
}

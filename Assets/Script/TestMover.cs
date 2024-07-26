using UnityEngine;

public class TestMover : MonoBehaviour
{
    public float moveSpeed = 1.0f; // 움직임 속도
    public float moveInterval = 5.0f; // 방향 변경 간격 (초 단위)
    private Vector3 direction; // 이동 방향

    private float timeSinceLastChange = 0f; // 마지막 방향 변경 이후 경과 시간

    public float minX = 8.0f; // x축 최소 범위
    public float maxX = 40.0f; // x축 최대 범위
    public float minZ = 5.0f; // z축 최소 범위
    public float maxZ = 62.0f; // z축 최대 범위

    void Start()
    {
        // 좌표 초기화
        transform.position = new Vector3(23.0f, 0.0f, 23.0f);

        // 첫 방향을 설정
        SetRandomDirection();
    }

    void Update()
    {
        // 현재 방향으로 이동
        transform.position += direction * moveSpeed * Time.deltaTime;

        // 위치 제한 확인
        ClampPosition();

        // 경과 시간 업데이트
        timeSinceLastChange += Time.deltaTime;

        // 방향을 변경할 시간에 도달했는지 확인
        if (timeSinceLastChange >= moveInterval)
        {
            SetRandomDirection();
            timeSinceLastChange = 0f; // 타이머 리셋
        }
    }

    void SetRandomDirection()
    {
        // 90도, 180도, 270도 중 랜덤하게 선택하여 회전
        float[] angles = { 90f, 180f, 270f };
        float randomAngle = angles[Random.Range(0, angles.Length)];

        // 현재 방향에서 랜덤하게 회전
        transform.Rotate(0, randomAngle, 0);

        // 현재 회전 방향에 따라 새로운 이동 방향 설정
        direction = transform.forward;
    }

    void ClampPosition()
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

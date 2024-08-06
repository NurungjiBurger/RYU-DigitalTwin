using UnityEngine;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Tf2;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using System.Linq;

public class RosTestSubscriber : MonoBehaviour
{
    public GameObject UIPanel;

    private RosSocket rosSocket;
    public string socketURL;
    public string topicName; // 구독할 토픽 이름

    // 이동 및 회전에 대한 설정
    public float moveSpeed;// 이동 속도
    public float rotationSpeed; // 회전 속도

    public UnityEngine.Vector3 targetPosition;
    //public UnityEngine.Vector3 targetRotation;
    public UnityEngine.Quaternion targetRotation;
    private bool isMoving = false;
    private bool isRotating = false;

    private Rigidbody rb;

    public int RobotID;
    public float RobotTemperature;
    public float WorkTime;

    private void Start()
    {
        RobotID = Random.Range(0, 10000000);
        RobotTemperature = Random.Range(20.0f, 50.0f);
        WorkTime = 0;

        //rb = GetComponent<Rigidbody>();
        //rb.isKinematic = false; // 물리적 이동을 허용
        //rb.freezeRotation = true; // 회전 고정 (옵션)


        // ROS WebSocket 서버의 URL
        socketURL = "ws://192.168.172.149:9090"; //"ws://192.168.56.105:9090"; // 적절한 URL로 수정 필요
        rosSocket = new RosSocket(new RosSharp.RosBridgeClient.Protocols.WebSocketNetProtocol(socketURL));

        // 토픽 구독
        if (topicName != null) rosSocket.Subscribe<TFMessage>(topicName, ReceiveMessage);


        // Collider가 없으면 추가
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>(); // BoxCollider 추가
        }
    }

    private void ReceiveMessage(TFMessage message)
    {
        Debug.Log(message);

        if (message.transforms.Count() > 0)
        {
            // 첫 번째 TransformStamped 메시지 추출
            TransformStamped transform = message.transforms[0];

            Debug.Log("first msg : " + message.transforms[0]);


            //x 증가 -> z 증가
            //y 증가 -> x 감소
            // 위치 데이터 추출 (소수점 4자리 반올림)

            // 0 -> 가로 끝 / 26 / 35
            // 0 -> 세로 끝 / 
            //UnityEngine.Vector3 translation = new UnityEngine.Vector3(
            //    //38
            //    35.0f + Mathf.Round(-(float)transform.transform.translation.y * 152000) / 10000.0f,
            //    0.0f, // Mathf.Round((float)transform.transform.translation.z * 10000) / 10000.0f, // Z와 Y를 교환
            //    5.0f + Mathf.Round((float)transform.transform.translation.x * 142000) / 10000.0f  // Y와 Z를 교환
            //);
            UnityEngine.Vector3 translation = new UnityEngine.Vector3(
                        38.0f + Mathf.Round(-(float)transform.transform.translation.y * 1550) / 100.0f,
                        0.0f, // Y값은 사용되지 않음
                        5.0f + Mathf.Round((float)transform.transform.translation.x * 1450) / 100.0f
                    );

            Debug.Log("진행");

            // 새로운 목표 위치 계산
            targetPosition = translation;

            // translation 값 디버깅 출력
            Debug.Log($"Translation - x: {transform.transform.translation.x}, y: {transform.transform.translation.y}, z: {transform.transform.translation.z}");
            Debug.Log($"Calculated Target Position - x: {targetPosition.x}, y: {targetPosition.y}, z: {targetPosition.z}");

            //직진->z 0 w 1
            //후진->z - 1 w 0
            //좌->z 0.7 w 0.7
            //우->z - 0.7 w 0.7

            // 회전 데이터 추출 (소수점 4자리 반올림)
            float x = Mathf.Round((float)transform.transform.rotation.x * 10000) / 10000.0f;
            float y = Mathf.Round((float)transform.transform.rotation.y * 10000) / 10000.0f;
            float z = Mathf.Round((float)transform.transform.rotation.z * 10000) / 10000.0f;
            float w = Mathf.Round((float)transform.transform.rotation.w * 10000) / 10000.0f;


            // 받은 회전값으로부터 회전 Quaternion 생성
            UnityEngine.Quaternion newRotation = new UnityEngine.Quaternion(0, -z, 0, w);

            // 기존 회전과 새로운 회전의 델타를 누적 회전으로 계산
            targetRotation = newRotation;
            // targetRotation = gameObject.transform.rotation * newRotation;

            // rotation 값 디버깅 출력
            Debug.Log($"Rotation - x: {transform.transform.rotation.x}, y: {transform.transform.rotation.y}, z: {transform.transform.rotation.z}, w: {transform.transform.rotation.w}");
            Debug.Log($"Calculated Target Rotation - x: {targetRotation.eulerAngles.x}, y: {targetRotation.eulerAngles.y}, z: {targetRotation.eulerAngles.z}");

            // 이동 및 회전 시작
            isMoving = true;
            isRotating = true;
        }
    }

    private float NormalizeAngle(float angle)
    {
        angle = angle % 360;
        if (angle < 0)
        {
            angle += 360;
        }
        return angle;
    }

    private void Update()
    {
        WorkTime += Time.deltaTime;

        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 화면에서 클릭 위치로 레이 발사
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform) // 클릭된 오브젝트가 이 스크립트가 붙은 오브젝트인지 확인
                {
                    UIPanel.GetComponent<UIController>().RobotInfoPanel(gameObject);
                    // UIOnOff 함수 실행
                }
            }
        }

        if (isMoving)
        {
            // 현재 위치에서 목표 위치로 이동
            float step = moveSpeed * Time.deltaTime; // 매 프레임 이동 거리
            transform.position = UnityEngine.Vector3.MoveTowards(transform.position, targetPosition, step);

            // 목표 위치에 도달하면 이동 중지
            if (UnityEngine.Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }

        if (isRotating)
        {
            // 현재 회전에서 목표 회전으로 회전
            // UnityEngine.Quaternion targetQuaternion = UnityEngine.Quaternion.Euler(targetRotation.eulerAngles);
            transform.rotation = UnityEngine.Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // 목표 회전 도달 시 회전 중지
            if (UnityEngine.Quaternion.Angle(transform.rotation, targetRotation) < 1.0f)
            {
                transform.rotation = targetRotation;
                isRotating = false;
            }
        }
    }

    private void OnDestroy()
    {
        if (rosSocket != null)
        {
            rosSocket.Close();
        }
    }
}

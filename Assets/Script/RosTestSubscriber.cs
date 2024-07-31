using UnityEngine;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Tf2;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using System.Linq;

public class RosTestSubscriber : MonoBehaviour
{
    public GameObject UIPanel;

    private RosSocket rosSocket;
    public string topicName = "/tf"; // 구독할 토픽 이름

    // 이동 및 회전에 대한 설정
    public float moveSpeed = 2.0f; // 이동 속도
    public float rotationSpeed = 100.0f; // 회전 속도

    private UnityEngine.Vector3 targetPosition;
    private UnityEngine.Quaternion targetRotation;
    private bool isMoving = false;
    private bool isRotating = false;

    public int RobotID;
    public float RobotTemperature;
    public float WorkTime;

    private void Start()
    {
        RobotID = Random.Range(0, 10000000);
        RobotTemperature = Random.Range(20.0f, 50.0f);
        WorkTime = 0;

        // ROS WebSocket 서버의 URL
        string rosBridgeUrl = "ws://192.168.56.105:9090"; // 적절한 URL로 수정 필요
        rosSocket = new RosSocket(new RosSharp.RosBridgeClient.Protocols.WebSocketNetProtocol(rosBridgeUrl));

        // 토픽 구독
        rosSocket.Subscribe<TFMessage>(topicName, ReceiveMessage);


        // Collider가 없으면 추가
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>(); // BoxCollider 추가
        }
    }

    private void ReceiveMessage(TFMessage message)
    {

        if (message.transforms.Count() > 0)
        {
            // 첫 번째 TransformStamped 메시지 추출
            TransformStamped transform = message.transforms[0];

            // 위치 데이터 추출
            targetPosition = new UnityEngine.Vector3(
                (float)transform.transform.translation.x,
                (float)transform.transform.translation.y,
                (float)transform.transform.translation.z);

            // 회전 데이터 추출
            targetRotation = new UnityEngine.Quaternion(
                (float)transform.transform.rotation.x,
                (float)transform.transform.rotation.y,
                (float)transform.transform.rotation.z,
                (float)transform.transform.rotation.w);

            // 이동 및 회전 시작
            isMoving = true;
            isRotating = true;
        }
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
            UnityEngine.Quaternion targetQuaternion = UnityEngine.Quaternion.Euler(targetRotation.eulerAngles);
            transform.rotation = UnityEngine.Quaternion.RotateTowards(transform.rotation, targetQuaternion, rotationSpeed * Time.deltaTime);

            // 목표 회전 도달 시 회전 중지
            if (UnityEngine.Quaternion.Angle(transform.rotation, targetQuaternion) < 1.0f)
            {
                transform.rotation = targetQuaternion;
                isRotating = false;
            }
        }
    }

    private void OnDestroy()
    {
        if (rosSocket != null)
        {
            rosSocket.Unsubscribe(topicName);
            rosSocket.Close();
        }
    }
}

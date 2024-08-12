using UnityEngine;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Tf2;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using System.Linq;

public class RosSubscriber : MonoBehaviour
{
    // 로봇이 눌렸을 때 켜질 UI 설정
    public GameObject UIPanel;

    // ROS2 연결 설정 변수
    // 소켓, URL, 토픽
    private RosSocket rosSocket;
    private string socketURL;
    public string topicName;

    // 이동 및 회전에 대한 설정
    // 이동 속도와 회전 속도
    public float moveSpeed;
    public float rotationSpeed;

    // 목표 좌표와 목표 회전 방향
    public UnityEngine.Vector3 targetPosition;
    public UnityEngine.Quaternion targetRotation;

    // 움직임 제어 변수
    private bool isMoving = false;
    private bool isRotating = false;

    // 로봇에 대해 보여줄 추가적인 정보를 담을 변수
    public int RobotID;
    public float RobotTemperature;
    public float WorkTime;

    private void Start()
    {
        socketURL = Data.Instance.ROSUrl;
        // 현재는 랜덤값으로 설정.
        // 차후 ROS2 통신으로 추가적으로 정보를 받아와서 할당할 예정.
        RobotID = Random.Range(0, 10000000);
        RobotTemperature = Random.Range(20.0f, 50.0f);
        WorkTime = 0;

        // ROS WebSocket 서버의 URL
        if (gameObject.name == "AtwoZ")
        {
            //socketURL = "ws://192.168.153.149:9090";
            rosSocket = new RosSocket(new RosSharp.RosBridgeClient.Protocols.WebSocketNetProtocol(socketURL));

            // 토픽 구독
            if (topicName != null) rosSocket.Subscribe<TFMessage>(topicName, ReceiveMessage);
        }
    }

    private void ReceiveMessage(TFMessage message)
    {
        if (message.transforms.Count() > 0)
        {
            // 첫 번째 TransformStamped 메시지 추출
            TransformStamped transform = message.transforms[0];

            //  현실      가상
            // x 증가 -> z 증가
            // y 증가 -> x 감소
            // 현실좌표와 가상좌표의 스케일차이가 있으므로 각각 맞춰서 보정
            // 정확하지는 않으나 1550, 1450으로 보정
            // 소수점 몇자리까지 사용할지 미정
            // z 4 ~ 40
            // x 7 ~ 42
            UnityEngine.Vector3 translation = new UnityEngine.Vector3(
                        34.0f + (Mathf.Round(-(float)transform.transform.translation.y * 1550000) / 100000.0f),
                        0.0f, // Y값은 사용되지 않음. 날아갈 일 없음
                        4.0f + (Mathf.Round((float)transform.transform.translation.x * 1600000) / 100000.0f)
                    );

            if (translation.z < 4.0f) translation.z = 4.0f;
            else if (translation.z > 40.0f) translation.z = 40.0f;

            if (translation.x < 7.0f) translation.x = 7.0f;
            else if (translation.x > 42.0f) translation.x = 42.0f;

            // 새로운 목표 위치 계산후 할당
            targetPosition = translation;

            // translation 값 디버깅 ....
            Debug.Log($"Translation - x: {transform.transform.translation.x}, y: {transform.transform.translation.y}, z: {transform.transform.translation.z}");
            Debug.Log($"Calculated Target Position - x: {targetPosition.x}, y: {targetPosition.y}, z: {targetPosition.z}");

            // 현실에서 들어오는 방향 데이터
            // 직진 -> z   0   w 1
            // 후진 -> z - 1   w 0
            // 좌   -> z   0.7 w 0.7
            // 우   -> z - 0.7 w 0.7

            // 회전 데이터 추출 (소수점 4자리 반올림)
            float x = Mathf.Round((float)transform.transform.rotation.x * 100000) / 100000.0f;
            float y = Mathf.Round((float)transform.transform.rotation.y * 100000) / 100000.0f;
            float z = Mathf.Round((float)transform.transform.rotation.z * 100000) / 100000.0f;
            float w = Mathf.Round((float)transform.transform.rotation.w * 100000) / 100000.0f;


            // 받은 회전값으로부터 회전 Quaternion 생성
            UnityEngine.Quaternion newRotation = new UnityEngine.Quaternion(0, -z, 0, w);

            // 회전 계산 후 할당
            targetRotation = newRotation;

            // rotation 값 디버깅 ....
            Debug.Log($"Rotation - x: {transform.transform.rotation.x}, y: {transform.transform.rotation.y}, z: {transform.transform.rotation.z}, w: {transform.transform.rotation.w}");
            Debug.Log($"Calculated Target Rotation - x: {targetRotation.eulerAngles.x}, y: {targetRotation.eulerAngles.y}, z: {targetRotation.eulerAngles.z}");

            // 이동 및 회전 시작을 위한 변수 설정
            isMoving = true;
            isRotating = true;
        }
    }

    public void UIOnOff()
    {
        UIPanel.GetComponent<UIController>().RobotInfoPanel(gameObject);
    }

    private void Awake()
    {

    }
    private void Update()
    {
        WorkTime += Time.deltaTime;

        if (UIPanel == null)
        {
            UIPanel = GameObject.Find("Canvas").transform.Find("RobotInfoPanel").gameObject;
        }

        // 움직일 수 있는 상태라면
        if (isMoving)
        {
            // 현재 위치에서 목표 위치로 이동할 것
            // 매 프레임 이동 거리
            float step = moveSpeed * Time.deltaTime;
            transform.position = UnityEngine.Vector3.MoveTowards(transform.position, targetPosition, step);

            // 목표 위치에 도달하면 이동 중지
            // 목표위치와 현재 위치의 차이가 0.1f보다 작다면 정지
            if (UnityEngine.Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }

        // 회전할 수 있는 상태라면
        if (isRotating)
        {
            // 현재 회전에서 목표 회전으로 회전
            transform.rotation = UnityEngine.Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // 목표 회전 도달 시 회전 중지
            // 마찬가지로 목표 회전과 현재 회전 차이가 1.0f보다 작다면 정지 ( 회전은 좀 더 큰 값으로 설정해줘도 큰 차이가 나지 않음 )
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

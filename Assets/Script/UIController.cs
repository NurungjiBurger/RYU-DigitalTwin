using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class UIController : MonoBehaviour, IPointerClickHandler
{
    // http request를 날릴 주소
    public string apiUrlBase;
    // 섹터 정보를 표시할 때 물류 정보를 동적으로 받아와서 생성해주기 위한 슬롯 프리팹
    public GameObject slotPrefab;
    // 섹터 정보를 동적으로 표시할 때 데이터를 표시해 줄 컨테이너
    public Transform content;
    // 섹터 정보를 동적으로 표시할 때 표현될 데이터들
    private List<SlotController> slots = new List<SlotController>();
    // 더미 데이터 배열
    private ItemData[] currentItems;
    // 현재 선택된 섹터
    private GameObject Sector;

    // 현재 선택된 로봇 대상
    private GameObject target;

    // 타이머
    private float Timer;

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
    private string TranslateTime(int timeData)
    {
        // 작업 시간을 포매팅해서 보여주기 위한 변환 작업
        int hours = timeData / 3600;
        int minutes = (timeData % 3600) / 60;
        int seconds = timeData % 60;

        // 두 자리 숫자로 포맷팅
        string str = $"{hours:D2} : {minutes:D2} : {seconds:D2}";
        return str;
    }
    // 로봇 인포 패널을 제어하기 위한 함수
    public void RobotInfoPanel(GameObject obj)
    {
        target = obj;
        UIOnOff(true);
        if (this.name == "RobotInfoPanel")
        {
            // 로봇에게서 정보를 받아와서 업데이트
            transform.Find("LeftBox").Find("Image").GetComponent<Image>().sprite = target.transform.Find("Image").GetComponent<Image>().sprite;
            transform.Find("LeftBox").Find("RobotName").GetComponent<TextMeshProUGUI>().text = target.name;

            transform.Find("RightBox").Find("RobotID").GetComponent<TextMeshProUGUI>().text = target.GetComponent<RosSubscriber>().RobotID.ToString();
            transform.Find("RightBox").Find("RobotTemperature").GetComponent<TextMeshProUGUI>().text = $"{target.GetComponent<RosSubscriber>().RobotTemperature:F1}°C";
            transform.Find("RightBox").Find("RobotWorkTime").GetComponent<TextMeshProUGUI>().text = TranslateTime((int)target.GetComponent<RosSubscriber>().WorkTime);
        }
    }

    // UI 켜고 끄기 위한 함수
    private void UIOnOff(bool val)
    {
        gameObject.SetActive(val);
    }

    // 오브젝트가 겹쳐있을 때 맘대로 켜지고 꺼지지 않도록 하기 위한 처리 함수
    public void UIOnOff(GameObject obj)
    {
        if (obj.name == "RobotMenu")
        {
            gameObject.SetActive(!gameObject.activeSelf);
            if (gameObject.activeSelf) obj.GetComponent<ButtonController>().ChangeSprite("right");
            else obj.GetComponent<ButtonController>().ChangeSprite("left");
        }
        else if (obj.name == "Menu") gameObject.SetActive(!gameObject.activeSelf);
        else if (obj.name == "Cancle") gameObject.SetActive(!gameObject.activeSelf);
        else if (obj == gameObject) gameObject.SetActive(false);
        else gameObject.SetActive(true);
    }
    private void Start()
    {
        // 시작되자마자 메뉴 버튼을 제외한 모든 UI는 사용자에게 보이면 안된다
        gameObject.SetActive(false);

        Timer = 0.0f;
    }

    private void FixedUpdate()
    {
        Timer += Time.deltaTime;
        // 오브젝트가 활성화된 상태라면
        if (gameObject.activeSelf)
        {
            // 정보 업데이트
            if (this.name == "MullyuInfoPanel" && Timer > 2.0f)
            {
                StartCoroutine(FetchDataFromApi(Sector.name));
                Timer = 0.0f;
            }
            else if (this.name == "RobotInfoPanel") transform.Find("RightBox").Find("RobotWorkTime").GetComponent<TextMeshProUGUI>().text = TranslateTime((int)target.GetComponent<RosSubscriber>().WorkTime);
        }
    }

    public void SettingSector(GameObject obj)
    {
        Sector = obj;
        // 데이터 설정
        // 서버로부터 데이터를 받아와서 만들어야하는 작업이므로 코루틴으로 비동기 작업으로 실행
        StartCoroutine(FetchDataFromApi(Sector.name));
        
    }

    // 비동기 작업 진행 데이터를 받아오는 함수
    private IEnumerator FetchDataFromApi(string sectorName)
    {
        string apiUrl = $"{apiUrlBase}?SectorName={sectorName}";
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error fetching data: " + request.error);
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;

            // JSON 데이터를 파싱하여 ItemData[]로 변환
            // 일반적으로 제공되는 JsonUtility는 커스텀 JSON들을 제대로 변환해주지 못함 ...
            try
            {
                JObject jsonObject = JObject.Parse(jsonResponse);

                // 섹터 이름을 키로 사용하여 섹터 데이터를 추출
                JArray itemArray = (JArray)jsonObject[sectorName];

                // ItemData 배열로 변환
                List<ItemData> itemList = itemArray.ToObject<List<ItemData>>();

                // currentItems에 데이터 할당
                currentItems = itemList.ToArray();

                // UI 초기화
                InitializeUI(currentItems, sectorName);
            }
            catch (JsonException e)
            {
                Debug.LogError("Error parsing JSON: " + e.Message);
            }
        }
    }

    private void InitializeUI(ItemData[] items, string sectorName)
    {
        // 기존 슬롯 제거
        slots.Clear();
        foreach (Transform child in content)
        {
            // 섹터 이름 업데이트
            if (child.gameObject.name == "SectorName")
            {
                child.gameObject.transform.GetComponent<TextMeshProUGUI>().text = "Sector" + sectorName;
                continue;
            }
            Destroy(child.gameObject);
        }

        // 새 슬롯 생성
        foreach (var item in items)
        {
            GameObject slot = Instantiate(slotPrefab, content);
            SlotController slotController = slot.GetComponent<SlotController>();
            if (slotController != null)
            {
                slotController.Setup(item);
                slots.Add(slotController);
            }
            else
            {
                Debug.LogError("SlotController component missing on prefab.");
            }
        }
    }
}


[System.Serializable]
public class ItemData
{
    public string productName;  // 물류이름
    public int productQuantity;     // 수량

    public ItemData(string name, int quantity)
    {
        productName = name;
        productQuantity = quantity;
    }
}
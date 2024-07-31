using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIController : MonoBehaviour, IPointerClickHandler
{
    public GameObject slotPrefab;
    public Transform content;
    private List<SlotController> slots = new List<SlotController>();
    private GameObject Sector;

    private GameObject target;
    // 더미 데이터 배열
    private ItemData[] currentItems;

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
    private string TranslateTime(int timeData)
    {
        int hours = timeData / 3600;
        int minutes = (timeData % 3600) / 60;
        int seconds = timeData % 60;

        // 두 자리 숫자로 포맷팅
        string str = $"{hours:D2} : {minutes:D2} : {seconds:D2}";
        return str;
    }
    public void RobotInfoPanel(GameObject obj)
    {
        target = obj;
        UIOnOff();
        if (this.name == "RobotInfoPanel")
        {
            transform.Find("LeftBox").Find("Image").GetComponent<Image>().sprite = target.transform.Find("Image").GetComponent<Image>().sprite;
            transform.Find("LeftBox").Find("RobotName").GetComponent<TextMeshProUGUI>().text = target.name;

            transform.Find("RightBox").Find("RobotID").GetComponent<TextMeshProUGUI>().text = target.GetComponent<RosTestSubscriber>().RobotID.ToString();
            transform.Find("RightBox").Find("RobotTemperature").GetComponent<TextMeshProUGUI>().text = $"{target.GetComponent<RosTestSubscriber>().RobotTemperature:F1}°C";
            transform.Find("RightBox").Find("RobotWorkTime").GetComponent<TextMeshProUGUI>().text = TranslateTime((int)target.GetComponent<RosTestSubscriber>().WorkTime);
        }
    }

    public void UIOnOff()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
    private void Start()
    {
        gameObject.SetActive(false);
        // 예제: SectorA를 테스트하는 경우
        //Sector = new GameObject("SectorA");
        //SettingSector(Sector);
    }

    private void FixedUpdate()
    {
        if (gameObject.activeSelf)
        {
            if (this.name == "MullyuInfoPanel") UpdateItemQuantities(); // 아이템 수량 업데이트
            else if (this.name == "RobotInfoPanel") transform.Find("RightBox").Find("RobotWorkTime").GetComponent<TextMeshProUGUI>().text = TranslateTime((int)target.GetComponent<RosTestSubscriber>().WorkTime);
        }
    }

    public void SettingSector(GameObject obj)
    {
        Sector = obj;
        // 더미 데이터 설정
        if (Sector.name == "SectorA")
        {
            LoadDummyDataForSectorA();
        }
        else if (Sector.name == "SectorB")
        {
            LoadDummyDataForSectorB();
        }
    }

    private void LoadDummyDataForSectorA()
    {
        currentItems = new ItemData[]
        {
            new ItemData { itemName = "chocolate", quantity = 10 },
            new ItemData { itemName = "galaxybook", quantity = 1 },
            new ItemData { itemName = "galaxyring", quantity = 1 },
            new ItemData { itemName = "galaxys24", quantity = 1 },
            new ItemData { itemName = "galaxywatch", quantity = 2 },
        };

        InitializeUI(currentItems);
    }

    private void LoadDummyDataForSectorB()
    {
        currentItems = new ItemData[]
        {
            new ItemData { itemName = "haribo", quantity = 100 },
            new ItemData { itemName = "dongwonchamchi", quantity = 20 },
            new ItemData { itemName = "chocolate", quantity = 20 },
        };

        InitializeUI(currentItems);
    }

    private void InitializeUI(ItemData[] items)
    {
        // 기존 슬롯을 초기화
        slots.Clear();
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // 새 슬롯을 생성하고 설정
        foreach (var item in items)
        {
            GameObject slot = Instantiate(slotPrefab, content);
            SlotController slotController = slot.GetComponent<SlotController>();
            slotController.Setup(item); // SlotController에서 아이템 설정
            slots.Add(slotController);
        }
    }

    private void UpdateItemQuantities()
    {
        // 더미 데이터의 수량을 랜덤으로 변경
        if (currentItems == null) return;

        // 아이템 수량을 랜덤으로 변경
        foreach (var item in currentItems)
        {
            item.quantity += Random.Range(-1, 2);
            item.quantity = Mathf.Max(item.quantity, 0);

            // 슬롯을 찾아서 업데이트
            SlotController slot = slots.Find(s => s.GetItemName() == item.itemName);
            if (slot != null)
            {
                slot.UpdateItem(item);
            }
        }
    }
}


[System.Serializable]
public class ItemData
{
    public string itemName;  // 물류이름
    public int quantity;     // 수량
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotController : MonoBehaviour
{
    // 메뉴에서 로봇 리스트를 보여줄 때 필요한 타겟변수
    public GameObject target;

    // 슬롯에 보여줄 데이터들
    // 이미지, 이름, 수량
    private Image image;
    private TextMeshProUGUI nameText;
    private int quantity;

    // Start is called before the first frame update
    void Start()
    {
        // 메뉴에 있는 슬롯들은 시작부터 타겟을 들고 있으므로 바로 초기화하고 이후로 업데이트 되지 않음.
        if (target != null)
        {
            image = target.transform.Find("Image").GetComponent<Image>();
            name = target.name;

            if (image != null) transform.Find("Image").GetComponent<Image>().sprite = target.transform.Find("Image").GetComponent<Image>().sprite;
            if (name != null) transform.Find("Name").GetComponent<TextMeshProUGUI>().text = name;
        }
    }

    public void Setup(ItemData itemData)
    {
        // 아이템의 스프라이트를 찾아서 설정
        name = itemData.productName;
        quantity = itemData.productQuantity;

        // 컴포넌트 초기화
        image = transform.Find("Image").GetComponent<Image>();
        nameText = transform.Find("Name").GetComponent<TextMeshProUGUI>();
        this.name = itemData.productName;

        // 스프라이트 설정
        Sprite sprite = Resources.Load<Sprite>($"Sprites/{name}");
        if (sprite != null)
        {
            image.sprite = sprite;
        }
        else
        {
            Debug.LogWarning($"Sprite not found for item: {name}");
        }

        // 수량 텍스트 설정
        nameText.text = quantity.ToString();
    }

    public void UpdateItem(ItemData itemData)
    {
        // 수량 텍스트만 업데이트
        nameText.text = itemData.productQuantity.ToString();
    }

    public string GetItemName()
    {
        return name;
    }
}
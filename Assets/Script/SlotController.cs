using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotController : MonoBehaviour
{
    public GameObject target; // 실제로는 사용할 필요 없음, ItemData로 직접 세팅함

    private Image image;
    private TextMeshProUGUI nameText;
    private string name;
    private int quantity;

    // Start is called before the first frame update
    void Start()
    {
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
        Sprite sprite = Resources.Load<Sprite>($"Sprites/{name}"); // "Sprites" 폴더 안에 아이템 이름으로 된 스프라이트가 있어야 합니다.
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
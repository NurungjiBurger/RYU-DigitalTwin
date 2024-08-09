using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Sector : MonoBehaviour
{
    // 섹터를 클릭했을 때 켜질 UI 설정
    public GameObject UIPanel;

    public void UIOnOff()
    {
        UIPanel.GetComponent<UIController>().UIOnOff(gameObject);
        UIPanel.GetComponent<UIController>().SettingSector(gameObject);
    }
    void Start()
    {

    }

    void Update()
    {
        if (UIPanel == null)
        {
            UIPanel = GameObject.Find("Canvas").transform.Find("MullyuInfoPanel").gameObject;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Sector : MonoBehaviour
{
    public GameObject UIPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 화면에서 클릭 위치로 레이 발사
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform) // 클릭된 오브젝트가 이 스크립트가 붙은 오브젝트인지 확인
                {
                    UIPanel.GetComponent<UIController>().UIOnOff(gameObject);
                    UIPanel.GetComponent<UIController>().SettingSector(gameObject);
                    // UIOnOff 함수 실행
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Sector : MonoBehaviour
{
    // 섹터를 클릭했을 때 켜질 UI 설정
    public GameObject UIPanel;
    void Start()
    {
        
    }

    void Update()
    {
        // 마우스 왼쪽 버튼 클릭이 들어왔다면
        if (Input.GetMouseButtonDown(0))
        {
            // 화면에서 클릭 위치로 레이 발사
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 클릭된 오브젝트가 이 스크립트가 붙은 오브젝트인지 확인
                if (hit.transform == transform)
                {
                    Debug.Log("오");
                    // UIOnOff 및 설정 함수 실행
                    UIPanel.GetComponent<UIController>().UIOnOff(gameObject);
                    UIPanel.GetComponent<UIController>().SettingSector(gameObject);
                }
            }
        }
    }
}

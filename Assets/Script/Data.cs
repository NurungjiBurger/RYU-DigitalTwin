using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;

public class Data : MonoBehaviour
{
    public static Data Instance { get; private set; }

    public string ServerUrl;
    public string ROSUrl;

    public GameObject inputText;
    public GameObject warningText;

    private float timer;

    public void ExitProgram()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    public void GoLoginScene()
    {
        SceneManager.LoadScene("LoginScene");
    }
    public void GoNextScene()
    {
        // TextMeshProUGUI에서 텍스트를 가져오기
        string str = inputText.GetComponent<TextMeshProUGUI>().text.Trim();

        str = str.Replace("\u200B", "").Trim();

        // 정규 표현식 패턴
        string pattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
        Regex regex = new Regex(pattern);

        // 패턴 일치
        if (regex.IsMatch(str))
        {
            ROSUrl = "ws://" + str + ":9090";
            SceneManager.LoadScene("MainScene");
        }
        else
        {
            warningText.GetComponent<TextMeshProUGUI>().text = "Is Not Valid IP Address";
            warningText.gameObject.SetActive(true);
            timer = 0.0f;
        }
    }

    private void Awake()
    {
        // 파괴되지 않는 오브젝트를 싱글톤으로 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        // 경고 메시지 초기화
        warningText.GetComponent<TextMeshProUGUI>().text = "";
        timer = 0.0f;

        ServerUrl = "https://i11a201.p.ssafy.io/productSector/sectorInfo";
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "LoginScene")
        {
            if (inputText == null)
            {
               inputText = GameObject.Find("Canvas").transform.Find("Panel").Find("BackGround").Find("IPAddressInputField").Find("Text Area").Find("Text").gameObject;
            }
            if (warningText == null)
            {
                warningText = GameObject.Find("Canvas").transform.Find("Panel").Find("BackGround").Find("Warning").gameObject;
            }
            
            timer += Time.deltaTime;
            if (timer > 5.0f && warningText != null) warningText.gameObject.SetActive(false);
        }

        // 마우스 왼쪽 버튼 클릭이 들어왔다면
        if (Input.GetMouseButtonDown(0))
        {
            // PointerEventData 생성
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            // RaycastAll을 사용하여 모든 UI 오브젝트 감지
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, raycastResults);

            if (raycastResults.Count > 0)
            {
                // UI 오브젝트 클릭 처리
                // UI가 활성화되어 있는 경우, UI를 꺼지게 하는 함수 호출
                // 클릭된 UI 오브젝트
                GameObject clickedUIObject = raycastResults[0].gameObject;

                if (raycastResults[0].gameObject.layer == LayerMask.NameToLayer("UI"))
                {
                    // UI 컨트롤러를 상위 패널에서 찾기
                    Transform parentTransform = clickedUIObject.transform;
                    UIController uiController = null;

                    while (parentTransform != null)
                    {
                        uiController = parentTransform.GetComponent<UIController>();
                        if (uiController != null)
                        {
                            break;
                        }
                        parentTransform = parentTransform.parent;
                    }

                    // UIController가 있는 경우 UI를 켜거나 끄는 함수 호출
                    if (uiController != null)
                    {
                        uiController.UIOnOff(uiController.gameObject);
                    }
                }
            }
            else
            {
                // UI 오브젝트 클릭이 없는 경우 3D 오브젝트 처리
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

                if (hits.Length > 0)
                {
                    // 가장 가까운 물체를 선택
                    RaycastHit closestHit = hits[0];

                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.distance < closestHit.distance)
                        {
                            closestHit = hit;
                        }
                    }

                    int layer = closestHit.collider.gameObject.layer;

                    if (layer == LayerMask.NameToLayer("Robot"))
                    {
                        closestHit.transform.GetComponent<RosSubscriber>().UIOnOff();
                    }
                    else if (layer == LayerMask.NameToLayer("Sector"))
                    {
                        closestHit.transform.GetComponent<Sector>().UIOnOff();
                    }
                }
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
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
            timer += Time.deltaTime;
            if (timer > 5.0f) warningText.gameObject.SetActive(false);
        }
    }
}

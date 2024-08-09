using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public void ChangeSprite(string dir)
    {
        GetComponent<Image>().sprite = Resources.Load<Sprite>($"Sprites/{dir}arrow");
    }
    // Start is called before the first frame update
    void Start()
    {
        // 시작 후 버튼 기능 할당
        if (gameObject.name == "Home") GetComponent<Button>().onClick.AddListener(() => Data.Instance.GoLoginScene());
        else if (gameObject.name == "RobotMenu") GetComponent<Button>().onClick.AddListener(() => gameObject.transform.Find("RobotPanel").GetComponent<UIController>().UIOnOff(gameObject));
        else if (gameObject.name == "Next") GetComponent<Button>().onClick.AddListener(() => Data.Instance.GoNextScene());
        else if (gameObject.name == "Menu") GetComponent<Button>().onClick.AddListener(() => gameObject.transform.Find("MenuPanel").GetComponent<UIController>().UIOnOff(gameObject));
        else if (gameObject.name == "Exit") GetComponent<Button>().onClick.AddListener(() => gameObject.transform.Find("ExitPanel").GetComponent<UIController>().UIOnOff(gameObject));
        else if (gameObject.name == "Cancle") GetComponent<Button>().onClick.AddListener(() => gameObject.transform.parent.GetComponent<UIController>().UIOnOff(gameObject));
        else if (gameObject.name == "Confirm") GetComponent<Button>().onClick.AddListener(() => Data.Instance.ExitProgram());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SimpleJSON;

public class SaveNewAccountDB : MonoBehaviour
{
    public InputField id;
    public InputField password;
    public InputField passwordCheck;
    public Text msg;
    public GameObject newAccountPnl;

    private string memberSaveURL = "http://ldj.dothome.co.kr/account_save.json.php";

    private void Awake()
    {
        id = GameObject.Find("CreateIdField").GetComponent<InputField>();
        password = GameObject.Find("CreatePassField").GetComponent<InputField>();
        passwordCheck = GameObject.Find("CreatePassCheckField").GetComponent<InputField>();
        msg = GameObject.Find("Msg").GetComponent<Text>();
    }
    void Start()
    {
        newAccountPnl.SetActive(false);
    }

    public void SaveNewAccount()
    {
        if (password.text.Length != 0 && password.text == passwordCheck.text)
        {
            StartCoroutine(NewAccount());
        }
        else
        {
            msg.text = "비밀번호를 재 확인하시오";
        }
    }

    public IEnumerator NewAccount()
    {
        WWWForm form = new WWWForm();
        form.AddField("idpost", id.text);
        form.AddField("passwordpost", password.text);

        var www = new WWW(memberSaveURL, form);

        yield return www;
        DisplayCreateAccountResult(www.text);
    }
    void DisplayCreateAccountResult(string strJsonData)
    {
        var jSon = JSON.Parse(strJsonData);

        string returnMsg = jSon["returnMsg"];
        if(returnMsg == "Success Create ID")
        {
            newAccountPnl.SetActive(false);
        }
        msg.text = returnMsg;

        Debug.Log(strJsonData);
    }
}

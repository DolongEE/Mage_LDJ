using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SimpleJSON;

public class LoadAccountDB : MonoBehaviour
{
    public InputField id;
    public InputField password;
    public Text msg;
    public StmenuManager stmenuManger;
    private GoLobby golobby;

    private string memberGetURL = "http://ldj.dothome.co.kr/account_get.json.php";

    private void Awake()
    {
        id = GameObject.Find("IdField").GetComponent<InputField>();
        password = GameObject.Find("PassField").GetComponent<InputField>();
        msg = GameObject.Find("Msg").GetComponent<Text>();
        golobby = GameObject.Find("GoLobby").GetComponent<GoLobby>();
    }
    void Start()
    {
        
    }

    public void LogInAccount()
    {
        if (password.text.Length != 0)
        {
            StartCoroutine("LoginMember");
        }
        else
        {
            msg.text = "비번을 확인해주세요.";
        }
    }

    public IEnumerator LoginMember()
    {
        WWWForm form = new WWWForm();
        form.AddField("idpost", id.text);
        form.AddField("passwordpost", password.text);

        var www = new WWW(memberGetURL, form);
        yield return www;
        DisplayLoadAccountResult(www.text);
    }
    void DisplayLoadAccountResult(string strJsonData)
    {
        var jSon = JSON.Parse(strJsonData);

        string returnMsg = jSon["returnMsg"];
        if(returnMsg == "Login Success")
        {
            golobby.login = true;
        }
        msg.text = returnMsg;

        Debug.Log(strJsonData);
    }
}

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class WebTest : MonoBehaviour
{
    public delegate void TryLoginResult(bool result, string message);
    public static event TryLoginResult OnTryLoginResult;

    public delegate void TryRegisterResult(bool result, string message);
    public static event TryRegisterResult OnTryRegisterResult;

    public delegate void TryGetChar(string message);
    public static event TryGetChar OnTryGetChar;

    //json parse example
    //https://youtu.be/og1zXHzl2cE?list=PLTm4FjoXO7nfn0jB0Ig6UbZU1pUHSLhRU&t=819

    private readonly string phpRegister = "http://localhost/UnityPHP/Register.php";
    private readonly string phpLogin = "http://localhost/UnityPHP/Login.php";
    private readonly string phpCharacterID = "http://localhost/UnityPHP/GetCharacterID.php";
    private readonly string phpCharacter = "http://localhost/UnityPHP/GetCharacter.php";
    private readonly string phpAttributes = "http://localhost/UnityPHP/GetAttributes.php";
    private readonly string phpStats = "http://localhost/UnityPHP/GetStats.php";
    private readonly string phpSkillID = "http://localhost/UnityPHP/GetSkillID.php";

    void Start()
    {   
    }

    IEnumerator GetDate(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    IEnumerator GetUsers(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    public IEnumerator Login(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("loginUser", username);
        form.AddField("loginPass", password);
        using UnityWebRequest www = UnityWebRequest.Post(phpLogin, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
            OnTryLoginResult(false, www.error);       
        else
            OnTryLoginResult(true, www.downloadHandler.text);   
    }

    //Referencia de Input no DB
    public IEnumerator RegisterUser(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("loginUser", username);
        form.AddField("loginPass", password);
        using UnityWebRequest www = UnityWebRequest.Post(phpRegister, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
            OnTryRegisterResult(false,www.error);
        else
            OnTryRegisterResult(true, www.downloadHandler.text);
    }   

    public IEnumerator GetCharacterID(int userID)
    {
        WWWForm form = new WWWForm();
        form.AddField("userID", userID.ToString());

        using UnityWebRequest www = UnityWebRequest.Post(phpCharacterID, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) { }
        else
            OnTryGetChar(www.downloadHandler.text);
    }

    public IEnumerator GetCharacter(int charID, System.Action<string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("characterID", charID.ToString());
        using UnityWebRequest www = UnityWebRequest.Post(phpCharacter, form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)  {}
        else
            callback(www.downloadHandler.text);
    }

    public IEnumerator GetAttributes(int charID, System.Action<string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("characterID", charID.ToString());
        using UnityWebRequest www = UnityWebRequest.Post(phpAttributes, form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success) { }
        else
            callback(www.downloadHandler.text);
    }

    public IEnumerator GetStats(int charID, System.Action<string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("characterID", charID.ToString());
        using UnityWebRequest www = UnityWebRequest.Post(phpStats, form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success) { }
        else
            callback(www.downloadHandler.text);
    }

    public IEnumerator GetSkillsID(int charID, System.Action<string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("characterID", charID.ToString());
        using UnityWebRequest www = UnityWebRequest.Post(phpStats, form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success) { }
        else
            callback(www.downloadHandler.text);
    }
}
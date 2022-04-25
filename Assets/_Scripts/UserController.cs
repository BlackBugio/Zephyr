using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

public class UserController : MonoBehaviour
{
    public TMP_InputField userInput;
    public TMP_InputField passInput;
    public TextMeshProUGUI loginFeedback;
    public Toggle remember;
    public Button loginButton;
    public Button registerButton;
    private WebTest web;
    private UiController uc;
    public UserData playerData;

    [System.Serializable]
    public class UserData
    {
        public int ID;
        public string username;
        public string password;
        public string email;
        public int zCoins;
    }
    

    private void Start()
    {
        uc = GetComponent<UiController>();
        web = GetComponent<WebTest>();
        WebTest.OnTryLoginResult += LoginResult;
        WebTest.OnTryRegisterResult += RegisterResult;
        loginButton.onClick.AddListener(() => { TryLogin(); });
        registerButton.onClick.AddListener(() => { TryRegister(); });

        if (PlayerPrefs.GetInt("Remember") == 1)
        {
            userInput.text = PlayerPrefs.GetString("User");
            passInput.text = PlayerPrefs.GetString("Pass");
        }
         playerData = new UserData();       
    }

    private void TryLogin()
    {
        StartCoroutine(web.Login(userInput.text, passInput.text));
    }
    private void TryRegister()
    {
        StartCoroutine(web.RegisterUser(userInput.text, passInput.text));
    }

    void LoginResult(bool result, string message)
    {
        if (result)
            switch (message)
            {
                case "nouser": loginFeedback.text = "Wrong User"; SaveUser(); break;
                case "wrongpass": loginFeedback.text = "Wrong password"; SaveUser(); break;
                default:
                {
                    SaveUser(); uc.ScreenActive(0);
                    playerData = JsonConvert.DeserializeObject<UserData>(message);
                    UpdatePlayerData();
                    StartCoroutine(web.GetCharacterID(playerData.ID));
                    break;
                }

            }
        else loginFeedback.text = "Connection Error";
    }

    void RegisterResult(bool result, string message)
    {
        Debug.Log(message);
        if (result)
            switch (message)
            {                
                case "taken": loginFeedback.text = "Username Taken"; break;
                default:
                {
                    SaveUser(); uc.ScreenActive(1);
                    playerData = JsonConvert.DeserializeObject<UserData>(message);
                    UpdatePlayerData();
                    break;
                }
            }
        else loginFeedback.text = "Connection Error";
    }

    void SaveUser()
    {
        
        if (remember.isOn)
        {
            PlayerPrefs.SetString("User", userInput.text);
            PlayerPrefs.SetString("Pass", passInput.text);
            PlayerPrefs.SetInt("Remember", 1);
        }
        else
        {
            PlayerPrefs.SetString("User", "");
            PlayerPrefs.SetString("Pass", "");
            PlayerPrefs.SetInt("Remember", 0);
        }
    }

    public void UpdatePlayerData()
    {
        uc.zCoins.text = playerData.zCoins.ToString();
        uc.userName.text = playerData.username;

    }

}

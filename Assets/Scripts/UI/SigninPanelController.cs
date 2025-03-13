using System;
using TMPro;
using UnityEngine;


public class SigninPanelController : MonoBehaviour
{
    [SerializeField] private TMP_InputField _usernameInputField;
    [SerializeField] private TMP_InputField _passwordInputField;

    void Start()
    {
        StartCoroutine(NetworkManage.Instance.VerifySession((userInfo) =>
        {
            GameManager.Instance.userInfo = userInfo;
            Destroy(gameObject);
            UIManager.Instance.GetUI<MainMenuController>(UI_TYPE.MainMenu);
        }, () =>
        {
        }));
    }

    public void OnClickSigninButton()
    {
        //테스트용 코드
        // Destroy(gameObject);
        // UIManager.Instance.GetUI<MainMenuController>(UI_TYPE.MainMenu);

        // return;

        string username = _usernameInputField.text;
        string password = _passwordInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            // TODO: 누락된 값 입렵 요청 팝업 표시
            return;
        }

        var signinData = new SigninData();
        signinData.username = username;
        signinData.password = password;
        
        StartCoroutine(NetworkManage.Instance.Signin(signinData, (userInfo) =>
        {
            GameManager.Instance.userInfo = userInfo;
            Destroy(gameObject);
            UIManager.Instance.GetUI<MainMenuController>(UI_TYPE.MainMenu);
        }, result =>
        {
            if (result == 0)
            {
                _usernameInputField.text = "";
            }
            else if (result == 1)
            {
                _passwordInputField.text = "";
            }
        }));
    }

    public void OnClickSignupButton()
    {
        UIManager.Instance.GetUI<SignupPanelController>(UI_TYPE.SignUp);
    }
}

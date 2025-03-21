using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;


public class SignupPanelController : PanelController
{ 
    [SerializeField] private TMP_InputField _usernameInputField;
    [SerializeField] private TMP_InputField _nicknameInputField;
    [SerializeField] private TMP_InputField _passwordInputField;
    [SerializeField] private TMP_InputField _confirmPasswordInputField;
    
    [SerializeField] private AsnycButton _signupButton;

    private void Awake()
    {
        _signupButton.AddListener(OnClickConfirmButton);
    }

    public async UniTask OnClickConfirmButton()
    {
        AudioManager.Instance.PlaySFX("putStone");

        var username = _usernameInputField.text;
        var nickname = _nicknameInputField.text;
        var password = _passwordInputField.text;
        var confirmPassword = _confirmPasswordInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(nickname) ||
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            UIManager.Instance.GetUI<AlarmPanelController>(UI_TYPE.Alarm).Show("입력 내용이 누락되었습니다.", () =>
            {
                
            });
            return;
        }

        if (password.Equals(confirmPassword))
        {
            SignupData signupData = new SignupData();
            signupData.username = username;
            signupData.nickname = nickname;
            signupData.password = password;

            try
            {
                await SignupAsync(signupData);
                Destroy(gameObject);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        else
        {
            UIManager.Instance.GetUI<AlarmPanelController>(UI_TYPE.Alarm).Show("비밀번호가 서로 다릅니다.", () =>
            {
                _passwordInputField.text = "";
                _confirmPasswordInputField.text = "";
            });
        }
    }

    private async UniTask SignupAsync(SignupData signupData)
    {
        var tcs = new UniTaskCompletionSource();
        
        StartCoroutine(NetworkManage.Instance.Signup(signupData, () =>
        {
            tcs.TrySetResult();
        }, () =>
        {
            _usernameInputField.text = "";
            _nicknameInputField.text = "";
            _passwordInputField.text = "";
            _confirmPasswordInputField.text = "";
            tcs.TrySetException(new Exception("회원가입 실패"));
        }));

        await tcs.Task;
    }

    public void OnClickCancelButton()
    {
        AudioManager.Instance.PlaySFX("putStone");
        Debug.Log("OnClickCancelButton");
        Destroy(gameObject);
    }
}

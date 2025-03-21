using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;


public class SigninPanelController : PanelController
{
    [SerializeField] private TMP_InputField _usernameInputField;
    [SerializeField] private TMP_InputField _passwordInputField;
    [SerializeField] private AsnycButton _signinButton;

    private void Awake()
    {
        _signinButton.AddListener(OnClickSigninButton);
    }

    void Start()
    {
        StartCoroutine(NetworkManage.Instance.VerifySession((userInfo) =>
        {
            GameManager.Instance.SetUserInfo(userInfo);
            Destroy(gameObject);
            UIManager.Instance.GetUI<MainMenuController>(UI_TYPE.MainMenu);
        }, () =>
        {
        }));
    }

    private async UniTask OnClickSigninButton()
    {
        AudioManager.Instance.PlaySFX("putStone");

        string username = _usernameInputField.text;
        string password = _passwordInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            // TODO: 누락된 값 입력 요청 팝업 표시
            return;
        }

        var signinData = new SigninData
        {
            username = username,
            password = password
        };

        try
        {
            // NetworkManage Signin을 UniTask로 래핑
            UserInfo info = await SigninAsync(signinData);
        
            GameManager.Instance.SetUserInfo(info);
            UIManager.Instance.ShowUI<MainMenuController>(UI_TYPE.MainMenu).Forget();
            GameManager.Instance.mainUIUpdate?.Invoke();
            Hide().Forget();
        }
        catch (Exception ex)
        {
            // 오류 처리
            Debug.LogError($"로그인 실패: {ex.Message}");
        }
    }
    
    private async UniTask<UserInfo> SigninAsync(SigninData signinData)
    {
        var tcs = new UniTaskCompletionSource<UserInfo>();
    
        StartCoroutine(NetworkManage.Instance.Signin(signinData,
                info => tcs.TrySetResult(info),
                res => {
                    if (res == 0)
                    {
                        _usernameInputField.text = "";
                    }
                    else if (res == 1)
                    {
                        _passwordInputField.text = "";
                    }
                    tcs.TrySetException(new Exception($"로그인 실패: {res}"));
                }
            )
        );
    
        return await tcs.Task;
    }
   
    public void OnClickSignupButton()
    {
        AudioManager.Instance.PlaySFX("putStone");
        UIManager.Instance.GetUI<SignupPanelController>(UI_TYPE.SignUp);
    }

    public override UniTask Show()
    {
        gameObject.SetActive(true);
        return UniTask.CompletedTask;
    }

    public override UniTask Hide()
    {
        _usernameInputField.text = "";
        _passwordInputField.text = "";

        gameObject.SetActive(false);
        return UniTask.CompletedTask;
    }
}

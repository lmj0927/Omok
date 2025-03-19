using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class NetworkManage : Singleton<NetworkManage>
{
    public IEnumerator Signup(SignupData signupData, Action success, Action failure)
    {
        string jsonString = JsonConvert.SerializeObject(signupData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);

        using (UnityWebRequest www =
               new UnityWebRequest(Constants.ServerURL + "/users/signup", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error: " + www.error);

                if (www.responseCode == 409)
                {
                    UIManager.Instance.GetUI<ConfirmPanelController>(UI_TYPE.Confirm).Show("이미 존재하는 사용자입니다.", () =>
                    {
                        failure?.Invoke();
                    });
                }
            }
            else
            {
                var result = www.downloadHandler.text;
                
                UIManager.Instance.GetUI<ConfirmPanelController>(UI_TYPE.Confirm).Show("회원 가입이 완료 되었습니다.", () =>
                {
                    success?.Invoke();
                });
            }
        }
    }
    
    public IEnumerator Signin(SigninData signinData, Action<UserInfo> success, Action<int> failure)
    {
        string jsonString = JsonConvert.SerializeObject(signinData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);

        using (UnityWebRequest www =
               new UnityWebRequest(Constants.ServerURL + "/users/signin", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                
            }
            else
            {
                var cookie = www.GetResponseHeader("set-cookie");
                if (!string.IsNullOrEmpty(cookie))
                {
                    int lastIndex = cookie.LastIndexOf(";");
                    string sid = cookie.Substring(0, lastIndex);
                    PlayerPrefs.SetString(Constants.SID, sid); 
                }
                
                var resultString = www.downloadHandler.text;
                var result = JsonConvert.DeserializeObject<SigninResult>(resultString);
                

                if (result.result == 0)
                {
                    UIManager.Instance.GetUI<ConfirmPanelController>(UI_TYPE.Confirm).Show("유저네임이 유효하지 않습니다.", () =>
                    {
                        failure?.Invoke(0);
                    });
                }
                else if (result.result == 1)
                {
                    UIManager.Instance.GetUI<ConfirmPanelController>(UI_TYPE.Confirm).Show("패스워드가 유효하지 않습니다.", () =>
                    {
                        failure?.Invoke(1);
                    });
                }
                else if (result.result == 2)
                {
                    UIManager.Instance.GetUI<ConfirmPanelController>(UI_TYPE.Confirm).Show("로그인에 성공하였습니다.", () =>
                    {
                        success?.Invoke(result.userInfo);
                    });
                }
            }
        }
    }

    public IEnumerator VerifySession(Action<UserInfo> success, Action failure)
    {
        using (UnityWebRequest www =
               new UnityWebRequest(Constants.ServerURL + "/users/session", UnityWebRequest.kHttpVerbGET))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            
            string sid = PlayerPrefs.GetString(Constants.SID, "");
            if (!string.IsNullOrEmpty(sid))
            {
                www.SetRequestHeader("Cookie", sid);
            }

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                if (www.responseCode == 403)
                {
                    Debug.Log("로그인이 필요합니다.");
                }
                
                failure?.Invoke();
            }
            else
            {
                var resultString = www.downloadHandler.text;
                var result = JsonConvert.DeserializeObject<SigninResult>(resultString);
                
                success?.Invoke(result.userInfo);
            }
        }
    }

    public IEnumerator GetUserInfo(string userId, Action<UserInfo> success, Action failure)
    {

        using (UnityWebRequest www =
               new UnityWebRequest(Constants.ServerURL + "/users/user/" + userId, UnityWebRequest.kHttpVerbGET))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            
            string sid = PlayerPrefs.GetString("sid", "");
            if (!string.IsNullOrEmpty(sid))
            {
                www.SetRequestHeader("Cookie", sid);
            }

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                if (www.responseCode == 403)
                {
                    Debug.Log("로그인이 필요합니다.");
                }
                
                failure?.Invoke();
            }
            else
            {
                var result = www.downloadHandler.text;
                var userInfoResult = JsonConvert.DeserializeObject<UserInfoResult>(result);
                
                success?.Invoke(userInfoResult.userInfo);
            }
        }
    }

    //GetUserInfo Wrapper
    public void LoadUserInfo(string userId, Action<UserInfo> success, Action failure)
    {
        StartCoroutine(GetUserInfo(userId, success, failure));
    }


    public IEnumerator SetUserInfo(UserInfo userInfo, Action success, Action failure)
    {
        string jsonString = JsonConvert.SerializeObject(userInfo);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);

        using (UnityWebRequest www =
               new UnityWebRequest(Constants.ServerURL + "/users/update", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            
            string sid = PlayerPrefs.GetString("sid", "");
            if (!string.IsNullOrEmpty(sid))
            {
                www.SetRequestHeader("Cookie", sid);
            }

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                if (www.responseCode == 403)
                {
                    Debug.Log("로그인이 필요합니다.");
                }
                
                failure?.Invoke();
            }
            else
            {
                // var result = www.downloadHandler.text;
                // var user = JsonUtility.FromJson<UserInfo>(result);
                
                success?.Invoke();
            }
        }
    }    
    //SetUserInfo Wrapper
    public void SaveUserInfoWrapper(UserInfo userInfo, Action success, Action failure)
    {
        StartCoroutine(SetUserInfo(userInfo, success, failure));
    }
    


    // GetLeaderboard 사용 예시
    // StartCoroutine(NetworkManage.Instance.GetLeaderboard((ret) => {
    //         foreach (var userInfo in ret.userInfos)
    //         {
    //             Debug.Log(userInfo.nickname);
    //         }
    //     }, () => {}));

    public IEnumerator GetLeaderboard(Action<UserInfos> success, Action failure)
    {
        using (UnityWebRequest www =
               new UnityWebRequest(Constants.ServerURL + "/users/leaderboard", UnityWebRequest.kHttpVerbGET))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            
            string sid = PlayerPrefs.GetString(Constants.SID, "");
            if (!string.IsNullOrEmpty(sid))
            {
                www.SetRequestHeader("Cookie", sid);
            }

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                if (www.responseCode == 403)
                {
                    Debug.Log("로그인이 필요합니다.");
                }
                
                failure?.Invoke();
            }
            else
            {
                var result = www.downloadHandler.text;
                var userInfos = JsonConvert.DeserializeObject<UserInfos>(result);
                
                success?.Invoke(userInfos);
            }
        }
    }


    public IEnumerator SendWinner(string userId, Action<UserInfo> success, Action failure)
    {

        Debug.Log("User ID: " + userId);
        using (UnityWebRequest www =
               new UnityWebRequest(Constants.ServerURL + "/users/win/" + userId, UnityWebRequest.kHttpVerbPOST))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            
            string sid = PlayerPrefs.GetString(Constants.SID, "");
            if (!string.IsNullOrEmpty(sid))
            {
                www.SetRequestHeader("Cookie", sid);
            }

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                if (www.responseCode == 403)
                {
                    Debug.Log("로그인이 필요합니다.");
                }
                
                failure?.Invoke();
            }
            else
            {
                var result = www.downloadHandler.text;
                var userInfoResult = JsonConvert.DeserializeObject<UserInfoResult>(result);
                
                success?.Invoke(userInfoResult.userInfo);
            }
        }
    }

    public IEnumerator SendLoser(string userId, Action<UserInfo> success, Action failure)
    {
        using (UnityWebRequest www =
               new UnityWebRequest(Constants.ServerURL + "/users/lose/" + userId, UnityWebRequest.kHttpVerbPOST))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            
            string sid = PlayerPrefs.GetString(Constants.SID, "");
            if (!string.IsNullOrEmpty(sid))
            {
                www.SetRequestHeader("Cookie", sid);
            }

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                if (www.responseCode == 403)
                {
                    Debug.Log("로그인이 필요합니다.");
                }
                
                failure?.Invoke();
            }
            else
            {
                var result = www.downloadHandler.text;
                var userInfoResult = JsonConvert.DeserializeObject<UserInfoResult>(result);
                
                success?.Invoke(userInfoResult.userInfo);
            }
        }
    }

    public IEnumerator SendDraw(string userId, Action<UserInfo> success, Action failure)
    {
        using (UnityWebRequest www =
               new UnityWebRequest(Constants.ServerURL + "/users/draw/" + userId, UnityWebRequest.kHttpVerbPOST))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            
            string sid = PlayerPrefs.GetString(Constants.SID, "");
            if (!string.IsNullOrEmpty(sid))
            {
                www.SetRequestHeader("Cookie", sid);
            }

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                if (www.responseCode == 403)
                {
                    Debug.Log("로그인이 필요합니다.");
                }
                
                failure?.Invoke();
            }
            else
            {
                var result = www.downloadHandler.text;
                var userInfoResult = JsonConvert.DeserializeObject<UserInfoResult>(result);
                
                success?.Invoke(userInfoResult.userInfo);
            }
        }
    }

    public void SendWinnerWrapper(string userId, Action<UserInfo> success, Action failure)
    {
        StartCoroutine(SendWinner(userId, success, failure));
    }

    public void SendLoserWrapper(string userId, Action<UserInfo> success, Action failure)
    {
        StartCoroutine(SendLoser(userId, success, failure));
    }

    public void SendDrawWrapper(string userId, Action<UserInfo> success, Action failure)
    {
        StartCoroutine(SendDraw(userId, success, failure));
    }
}
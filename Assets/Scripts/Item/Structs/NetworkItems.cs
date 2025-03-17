using System;

public struct SigninData
{
    public string username;
    public string password;
}


public struct SigninResult
{
    public int result;
    public UserInfo userInfo { get; set; }
}

public struct UserInfoResult
{
    public UserInfo userInfo { get; set; }
}


public struct UserInfos
{
    public UserInfo[] userInfos { get; set; }
}


public struct SignupData
{
    public string username;
    public string nickname;
    public string password;
}

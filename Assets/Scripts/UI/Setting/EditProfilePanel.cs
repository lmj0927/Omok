using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class EditProfilePanel : PopupController
{
    [SerializeField] List<Button> profileImage;
    private List<Sprite> _profileSprites = new List<Sprite>();
    private int _profileIndex;
    
    UserInfo _userInfo;
    
    void Start()
    {
        _profileSprites.AddRange(ResourceManager.Instance.ProfileSprites);
        for (int i = 0; i < profileImage.Count; i++)
        {
            profileImage[i].GetComponent<Image>().sprite = _profileSprites[i];
        }
        
        Show();
    }
    
    public void OnClickProfile(int index)
    {
        _profileIndex = index;
        GameManager.Instance.playerDataController.SetProfile(index);
        
        Hide();
    }

    public void OnClickCloseButton()
    {
        Hide();
    }
}

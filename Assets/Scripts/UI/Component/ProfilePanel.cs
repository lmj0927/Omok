using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePanel : MonoBehaviour
{
    [SerializeField] Image profileImage;
    [SerializeField] List<Sprite> profileSprites;

    private void Start()
    {
        GameManager.Instance.playerDataController.OnChangedProfile += SetProfileImage;
    }

    public void SetProfileImage(int idx)
    {
        if (idx < 0 || idx >= profileSprites.Count)
        {
            Debug.LogError("Profile Index out of range.");
            return;
        }
        
        profileImage.sprite = profileSprites[idx];
    }
}

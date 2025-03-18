using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    public List<Sprite> ProfileSprites = new List<Sprite>();

    protected override void Awake()
    {
        LoadProfileDatas();
    }
    
    private void LoadProfileDatas()
    {
        ProfileSprites.AddRange(Resources.LoadAll<Sprite>("Sprites/Profiles"));
    }
}

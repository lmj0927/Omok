using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class ResourceManager : Singleton<ResourceManager>
{
    public List<Sprite> ProfileSprites = new List<Sprite>();
    public Sprite endMarker;

    protected override void Awake()
    {
        LoadDatas();
    }
    
    private void LoadDatas()
    {
        ProfileSprites.AddRange(Resources.LoadAll<Sprite>("Sprites/Profiles"));
        endMarker = Resources.Load<Sprite>("Sprites/marker-OmokBg");
    }
}

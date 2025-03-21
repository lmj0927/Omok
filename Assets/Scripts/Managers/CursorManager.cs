using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CursorManager : Singleton<CursorManager>
{
    public Texture2D cursorBrushTexture;
    public Texture2D cursorHandTexture;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(cursorBrushTexture, new Vector2(0, cursorBrushTexture.height / 8f), CursorMode.ForceSoftware);
    }

    public void SetBrushCursorTexture()
    {
        Cursor.SetCursor(cursorBrushTexture, new Vector2(0, cursorBrushTexture.height / 8f), CursorMode.ForceSoftware);
    }

    public void SetHandCursorTexture()
    {
        Cursor.SetCursor(cursorHandTexture, new Vector2(cursorHandTexture.width / 2f ,0), CursorMode.ForceSoftware);
    }
}

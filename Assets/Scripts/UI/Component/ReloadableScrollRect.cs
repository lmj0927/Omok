using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using VFolders.Libs;

public class ReloadableScrollRect : ScrollRect
{
    private List<GameObject> _cells = new List<GameObject>();
    
    public void Reload<T>(List<T> items, GameObject cellPrefab)
    {
        //cell 조정
        if (items.Count < _cells.Count)
        {
            for (var i = items.Count; i < _cells.Count; i++)
            {
                _cells[i].gameObject.SetActive(false);
            }
        }
        else if (items.Count > _cells.Count)
        {
            for (var i = _cells.Count; i < items.Count; i++)
            {
                var cell = Instantiate(cellPrefab, content);
                _cells.Add(cell);
            }
        }
        
        //데이터 설정
        for (var i = 0; i < items.Count; i++)
        {
            if (!_cells[i].IsUnityNull())
            {
                if(_cells[i].TryGetComponent<IReloadableCell<T>>(out var cellComponent))
                {
                    cellComponent.SetData(items[i]);
                }
            }
        }
    }
}
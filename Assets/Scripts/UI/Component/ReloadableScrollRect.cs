using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadableScrollView : ScrollRect
{
    private GameObject _cellPrefab = null;
    private List<RectTransform> _cells = new List<RectTransform>();
    
    public void Initialize<T>(List<T> items, GameObject cellPrefab)
    {
        _cellPrefab = cellPrefab;
        
        // 기존 셀 정리
        foreach (var cell in _cells)
        {
            if (cell != null)
                Destroy(cell.gameObject);
        }
        _cells.Clear();

        foreach (var item in items)
        {
            var cell = GameObject.Instantiate(_cellPrefab, content);
            var rectTransform = cell.GetComponent<RectTransform>();
            _cells.Add(rectTransform);
            
            if(cell.TryGetComponent<IReloadableCell<T>>(out var cellComponent))
            {
                cellComponent.SetData(item);
            }
        }
    }
    
    public void Reload<T>(List<T> items, Action onReload = null)
    {
        Initialize(items, _cellPrefab);
        onReload?.Invoke();
    }
}
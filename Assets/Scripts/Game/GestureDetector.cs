using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureDetector : Singleton<GestureDetector>
{
    [SerializeField] private float speedThreshold;
    [SerializeField] private float heightThreshold;
    
    public Action OnGestureDetected;
    
    private float _dragTime;
    private Vector2 _startPos;
    private Vector2 _endPos;

    void Update()
    {
        if (GameManager.Instance.matchController == null ||
            GameManager.Instance.matchController.GetMatchState() == Constants.MATCH_STATE.End) return;
        
        if (Input.GetMouseButtonDown(1))
        {
            _startPos = Input.mousePosition;
            _dragTime = 0f;
        }
        
        if (Input.GetMouseButtonUp(1))
        {
            _endPos = Input.mousePosition;
            CheckThreshold();
        }
        
        _dragTime += Time.deltaTime;
    }

    private void CheckThreshold()
    {
        var speed = (_endPos - _startPos).magnitude / _dragTime;
        if (speed < speedThreshold || _endPos.y - _startPos.y < heightThreshold) return;
        
        OnGestureDetected?.Invoke();
    }
}

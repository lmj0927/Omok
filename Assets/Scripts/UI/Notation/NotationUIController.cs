using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NotationUIController : PanelController
{
    [Header("UI Elements")]
    [SerializeField] private ReloadableScrollRect notationScrollRect;
    [SerializeField] private GameObject notationCellPrefab;
    [SerializeField] private Button backButton;
    
    private RectTransform _notationPanel;

    private void Awake()
    {
        _notationPanel = gameObject.GetComponent<RectTransform>();
        backButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX("buttonClick");
            GameManager.Instance.mainUIUpdate?.Invoke();
            Hide();
        });
    }

    private void OnEnable()
    {
        var matchInfos = MatchInfoUtil.LoadMatchInfoList();
        notationScrollRect.Reload(matchInfos, notationCellPrefab);
    }
}

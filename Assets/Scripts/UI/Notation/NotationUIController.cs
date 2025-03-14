using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NotationUIController : MonoBehaviour, IGameUI
{
    [Header("UI Elements")]
    [SerializeField] private ReloadableScrollRect notationScrollRect;
    [SerializeField] private GameObject notationCellPrefab;
    [SerializeField] private Button backButton;
    
    private RectTransform _notationPanel;

    private void Awake()
    {
        _notationPanel = gameObject.GetComponent<RectTransform>();
        backButton.onClick.AddListener(Hide);
    }

    private void OnEnable()
    {
        var matchInfos = GameManager.Instance.playerDataController.matchInfos;
        notationScrollRect.Reload(matchInfos, notationCellPrefab);
    }

    public void Show()
    {
        var original = _notationPanel.anchoredPosition;
        _notationPanel.anchoredPosition = new Vector2(Screen.width, original.y);

        gameObject.SetActive(true);
        _notationPanel.DOAnchorPosX(original.x, .3f).SetEase(Ease.InQuint);
    }
    
    public void Hide()
    {
        var original = _notationPanel.anchoredPosition;

        _notationPanel.DOAnchorPosX(Screen.width, .3f).SetEase(Ease.InQuint).OnComplete(() =>
        {
            gameObject.SetActive(false);
            _notationPanel.anchoredPosition = original;
        });
    }
}

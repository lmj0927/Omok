using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardPanelController : PanelController
{
    [SerializeField] private Image profileImage;
    [SerializeField] private Sprite[] profileSprites;
    [SerializeField] private TMP_Text userInfoText;
    [SerializeField] private TMP_Text userWinRateText;
    
    [SerializeField] private GameObject rankCellPrefab;
    [SerializeField] private RectTransform contentTransform;
    [SerializeField] private float cellSize; //140
    
    [SerializeField] private Button closeButton;
    UserInfo _userInfo;
    
    private void Start()
    {
        closeButton.onClick.AddListener(OnClickCloseButton);
        Show();
        InitUserCell();
        CreateRankCell();
    }

    void InitUserCell()
    {
        _userInfo = GameManager.Instance.userInfo;
        
        //TODO: 랭크 계산 후 순위 적용

        int index = _userInfo.profileIndex;
        profileImage.sprite = profileSprites[index];
        
        float winRate = (float)_userInfo.winCount / (_userInfo.winCount + _userInfo.loseCount) * 100f;
        userInfoText.text = $"{_userInfo.tier}급 {_userInfo.nickname}";
        userWinRateText.text = $"{_userInfo.winCount}승 {_userInfo.loseCount}패 ({winRate:F0}%)";
    }

    void CreateRankCell()
    {
        //임시코드
        _userInfo = GameManager.Instance.userInfo;
        
        for (int i = 0; i < 20; i++)
        {
            var cell = Instantiate(rankCellPrefab, contentTransform); 
            cell.GetComponent<RankInfoCell>().SetRankInfo(_userInfo);
            contentTransform.sizeDelta = new Vector2(0, cellSize*i);
        }
    }

    void OnClickCloseButton()
    {
        Hide();
    }
}

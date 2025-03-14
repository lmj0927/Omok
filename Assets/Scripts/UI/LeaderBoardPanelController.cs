using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        _userInfo = GameManager.Instance.playerDataController.UserInfo;
        
        closeButton.onClick.AddListener(OnClickCloseButton);
        
        Show();
    }

    void OnEnable()
    {
        //기존 셀 삭제.
        if (contentTransform.GetComponentsInChildren<Transform>() != null)
        {
            var cells = contentTransform.GetComponentsInChildren<Transform>();
            foreach (var cell in cells)
            {
                if(cell != contentTransform) Destroy(cell.gameObject);
            }
        }
        
        CreateRankCell();
    }

    void InitUserCell(UserInfo userInfo,int rankIndex)
    {
        int index = userInfo.profileIndex;
        profileImage.sprite = profileSprites[index];
        
        float winRate = (float)userInfo.winCount / (userInfo.winCount + userInfo.loseCount) * 100f;
        userInfoText.text = $"{userInfo.tier}급 {userInfo.nickname}";
        userWinRateText.text = $"{rankIndex}위 | {userInfo.winCount}승 {userInfo.loseCount}패 ({winRate:F0}%)";
    }

    void CreateRankCell()
    {
        StartCoroutine(NetworkManage.Instance.GetLeaderboard((userinfos) =>
        {
            List<UserInfo> userInfoList = new List<UserInfo>();
            userInfoList.AddRange(userinfos.userInfos);
            
            //티어, 승률 순으로 정렬
            userInfoList.OrderBy(a => a.tier)
                .ThenByDescending(x=>(x.winCount + x.loseCount) == 0 ? 0 :(float)x.winCount / (x.winCount + x.loseCount) * 100f)
                .ToList();
            
            var rank = 1;
            foreach (var userinfo in userInfoList)
            {
                var cell = Instantiate(rankCellPrefab, contentTransform); 
                cell.GetComponent<RankInfoCell>().SetRankInfo(userinfo, rank);
                    
                //1,2,3등 강조 표시
                switch (rank)
                {
                    case 1:
                        cell.GetComponentsInChildren<Image>()[0].DOColor(new Color32(180,90,255,255),0);
                        break;
                    case 2:
                        cell.GetComponentsInChildren<Image>()[0].DOColor(new Color32(84,255,194,255),0);
                        break;
                    case 3:
                        cell.GetComponentsInChildren<Image>()[0].DOColor(new Color32(255,228,83,255),0);
                        break;
                }

                //랭킹 내 정보가 내 정보면 강조 표시.
                if (_userInfo.userId == userinfo.userId)
                {
                    cell.GetComponentsInChildren<TMP_Text>()[1].DOColor(Color.blue, 0);
                    InitUserCell(userinfo, rank);
                }
                    
                contentTransform.sizeDelta = new Vector2(0, cellSize*rank);
                
                rank++;
            }
        }, () =>
        {
            Debug.Log("Failed to create rank cell");
        }));
    }

    void OnClickCloseButton()
    {
        Hide();
    }
}

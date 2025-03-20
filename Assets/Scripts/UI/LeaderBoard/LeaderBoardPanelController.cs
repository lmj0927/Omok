using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardPanelController : PanelController
{
    [Header("User Cell")]
    [SerializeField] private Image profileImage;
    private List<Sprite> _profileSprites = new List<Sprite>();
    [SerializeField] private TMP_Text userInfoText;
    [SerializeField] private TMP_Text userWinRateText;
    
    [Header("Rank Cells")]
    [SerializeField] private GameObject rankCellPrefab;
    [SerializeField] private ReloadableScrollRect scrollRect;
    
    [SerializeField] private Button closeButton;
    UserInfo _userInfo;
    
    private void Start()
    {
        _profileSprites.AddRange(ResourceManager.Instance.ProfileSprites);
        closeButton.onClick.AddListener(OnClickCloseButton);
    }

    void OnEnable()
    {
        ReloadCell();
    }

    void InitUserCell(UserInfo userInfo,int rankIndex)
    {
        int index = userInfo.profileIndex;
        profileImage.sprite = _profileSprites[index];
        
        float winRate = (float)userInfo.winCount / (userInfo.winCount + userInfo.loseCount) * 100f;
        userInfoText.text = $"{userInfo.tier}급 {userInfo.nickname}";
        userWinRateText.text = $"{rankIndex}위 | {userInfo.winCount}승 {userInfo.loseCount}패 ({(float.IsNaN(winRate) ? 0 : winRate):F0}%)";
        Debug.Log(winRate);
    }

    void ReloadCell()
    {
        StartCoroutine(NetworkManage.Instance.GetLeaderboard((userinfos) =>
        {
            List<UserInfo> userInfoList = new List<UserInfo>();
            userInfoList.AddRange(userinfos.userInfos);

            //티어, 승률 순으로 정렬
            userInfoList.OrderBy(a => a.tier)
                .ThenByDescending(x =>
                    (x.winCount + x.loseCount) == 0 ? 0 : (float)x.winCount / (x.winCount + x.loseCount) * 100f)
                .ToList();
            
            //오브젝트 생성 및 데이터 업데이트
            scrollRect.Reload(userInfoList,rankCellPrefab);
            
            //랭크에 따른 추가 구현
            var cells = scrollRect.content.GetComponentsInChildren<RankInfoCell>();

            int rank = 1;
            foreach (var cell in cells)
            {
                //랭킹 적용
                cell.SetRank(rank);
                
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
                
                //최상단의 플레이어 본인의 정보 업데이트 및 리더보드내 본인 강조표시
                var cellInfo = cell.GetInfo();
                _userInfo = GameManager.Instance.GetUserInfo();
                if (cellInfo.userId == _userInfo.userId)
                {
                    cell.GetComponentsInChildren<TMP_Text>()[1].DOColor(Color.blue, 0);
                    InitUserCell(cellInfo, rank);
                }
                
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
        GameManager.Instance.mainUIUpdate?.Invoke();
    }
}

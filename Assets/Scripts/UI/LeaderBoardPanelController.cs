using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardPanelController : MonoBehaviour
{
    [SerializeField] private GameObject rankCellPrefab;
    [SerializeField] private RectTransform contentTransform;
    [SerializeField] private float cellSize; //
    
    [SerializeField] private Button closeButton;
    UserInfo userInfo;
    
    private void Start()
    {
        closeButton.onClick.AddListener(()=>Hide());
    }

    void CreateRankCell()
    {
        // StartCoroutine(NetworkManager.Instance.GetScore((userinfo) =>
        // {
        //     user = userinfo;
        //     StartCoroutine(NetworkManager.Instance.GetLeaderboard((scores) =>
        //     {
        //         var rank = 1;
        //         foreach (var scoreinfo in scores.scores)
        //         {
        //             var cell = Instantiate(rankCellPrefab, contentTransform); 
        //             cell.GetComponent<RankInfoCell>().SetRankInfo(scoreinfo);
        //             contentTransform.sizeDelta = new Vector2(0, cellSize*rank);
        //             rank++;
        //         }
        //     }, () =>
        //     {
        //         Debug.Log("Failed to create rank cell");
        //     }));
        // }, () =>
        // {
        //
        // }));
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }   
}

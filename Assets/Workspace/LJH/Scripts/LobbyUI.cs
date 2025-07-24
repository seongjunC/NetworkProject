using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button creatRoomButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button previousPageButton;
    [SerializeField] private Button nextPageButton;

    [Header("연결할 패널")]
    [SerializeField] private GameObject mainmenuPanel;
    [SerializeField] private GameObject insideRoomPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        creatRoomButton.onClick.AddListener(OnClickCraetRoom);
        quickJoinButton.onClick.AddListener(OnClickQuickJoin);
        backButton.onClick.AddListener(OnClickBack);
        previousPageButton.onClick.AddListener(OnClickPreviousPage);
        nextPageButton.onClick.AddListener(OnClickNextPage);
    }
    
    private void OnClickCraetRoom()
    {
        //To do : 방 생성 로직 구현
    }
    private void OnClickQuickJoin()
    {
        //To do : 방 자동 참여 로직 구현
    }
    private void OnClickBack()
    {
        gameObject.SetActive(false);         // 로비 패널 닫기
        mainmenuPanel.SetActive(true);       // 메인 메뉴 패널 열기
    }
    private void OnClickPreviousPage()
    {
        //To do : 왼쪽 페이지로 이동하는 로직 구현
    }
    private void OnClickNextPage()
    {
        //To do : 오른쪽 페이지로 이동하는 로직 구현
    }
}

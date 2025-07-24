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

    [Header("������ �г�")]
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
        //To do : �� ���� ���� ����
    }
    private void OnClickQuickJoin()
    {
        //To do : �� �ڵ� ���� ���� ����
    }
    private void OnClickBack()
    {
        gameObject.SetActive(false);         // �κ� �г� �ݱ�
        mainmenuPanel.SetActive(true);       // ���� �޴� �г� ����
    }
    private void OnClickPreviousPage()
    {
        //To do : ���� �������� �̵��ϴ� ���� ����
    }
    private void OnClickNextPage()
    {
        //To do : ������ �������� �̵��ϴ� ���� ����
    }
}

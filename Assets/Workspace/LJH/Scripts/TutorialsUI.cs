using GifImporter;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TutorialPage
{
    public string text;   // 설명 텍스트
    public Sprite image;  // 설명 이미지
    public Gif gif;       // 움직이는 Gif (ScriptableObject)
}

public class TutorialsUI : MonoBehaviour
{
    [Header("UI 버튼")]
    [SerializeField] private Button gachaButton;
    [SerializeField] private Button moveButton;
    [SerializeField] private Button angleButton;
    [SerializeField] private Button powerButton;
    [SerializeField] private Button itemButton;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Button previousPageButton;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button outPageButton;

    [Header("UI 출력")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image tutorialImage;
    [SerializeField] private GifPlayer gifPlayer; // 추가: gif 재생 전용

    [Header("패널 연결")]
    [SerializeField] private GameObject mainmenuPanel;

    [Header("튜토리얼 데이터")]
    [SerializeField] private List<TutorialPage> gachaPages;
    [SerializeField] private List<TutorialPage> movePages;
    [SerializeField] private List<TutorialPage> anglePages;
    [SerializeField] private List<TutorialPage> powerPages;
    [SerializeField] private List<TutorialPage> itemPages;
    [SerializeField] private List<TutorialPage> endTurnPages;

    [Header("페이지 인디케이터")]
    [SerializeField] private Transform dotParent;     // 동그라미 버튼 부모
    [SerializeField] private GameObject dotPrefab;    // 동그라미 버튼 프리팹
    [SerializeField] private Sprite activeDotSprite;  // 활성화 이미지
    [SerializeField] private Sprite inactiveDotSprite;// 비활성화 이미지

    private List<List<TutorialPage>> tutorialData = new List<List<TutorialPage>>();
    private List<Image> dots = new List<Image>();

    private int mode = 0; // 현재 모드 인덱스
    private int currentPage = 0;

    void Awake()
    {
        text.gameObject.SetActive(false);
        tutorialImage.gameObject.SetActive(false);
        previousPageButton.gameObject.SetActive(false);
        nextPageButton.gameObject.SetActive(false);
    }

    void Start()
    {
        // 튜토리얼 데이터 세팅
        tutorialData.Add(gachaPages);   // 0
        tutorialData.Add(movePages);    // 1
        tutorialData.Add(anglePages);   // 2
        tutorialData.Add(powerPages);   // 3
        tutorialData.Add(itemPages);    // 4
        tutorialData.Add(endTurnPages); // 5

        // 버튼 이벤트
        gachaButton.onClick.AddListener(() => ChangeMode(0));
        moveButton.onClick.AddListener(() => ChangeMode(1));
        angleButton.onClick.AddListener(() => ChangeMode(2));
        powerButton.onClick.AddListener(() => ChangeMode(3));
        itemButton.onClick.AddListener(() => ChangeMode(4));
        endTurnButton.onClick.AddListener(() => ChangeMode(5));

        previousPageButton.onClick.AddListener(OnClickPreviousPage);
        nextPageButton.onClick.AddListener(OnClickNextPage);
        outPageButton.onClick.AddListener(OnClickOutPage);

        // 기본 모드 시작
        ChangeMode(0);
    }

    private void ChangeMode(int newMode)
    {
        mode = newMode;
        currentPage = 0;

        CreateDots();
        UpdatePageUI();
    }

    private void OnClickPreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdatePageUI();
        }
    }

    private void OnClickNextPage()
    {
        if (currentPage < tutorialData[mode].Count - 1)
        {
            currentPage++;
            UpdatePageUI();
        }
    }

    private void CreateDots()
    {
        // 기존 Dot 제거
        foreach (Transform child in dotParent)
        {
            Destroy(child.gameObject);
        }
        dots.Clear();

        // 새 Dot 생성
        int totalPages = tutorialData[mode].Count;
        for (int i = 0; i < totalPages; i++)
        {
            GameObject dot = Instantiate(dotPrefab, dotParent);
            dot.SetActive(true); // 생성 직후 강제로 켜줌
            dot.GetComponent<Image>().enabled = true; // 이미지 보이게
            Image dotImage = dot.GetComponent<Image>();
            dots.Add(dotImage);

            int pageIndex = i; // 클릭용 로컬 변수
            dot.GetComponent<Button>().onClick.AddListener(() => GoToPage(pageIndex));
        }
    }

    private void UpdateDots()
    {
        for (int i = 0; i < dots.Count; i++)
        {
            dots[i].sprite = (i == currentPage) ? activeDotSprite : inactiveDotSprite;
        }
    }

    private void GoToPage(int pageIndex)
    {
        currentPage = pageIndex;
        UpdatePageUI();
    }

    private void UpdatePageUI()
    {
        if (tutorialData[mode].Count == 0)
        {
            text.text = "페이지 데이터가 없습니다.";
            tutorialImage.gameObject.SetActive(false);
            gifPlayer.gameObject.SetActive(false);
            return;
        }

        var page = tutorialData[mode][currentPage];

        // 텍스트 업데이트
        text.text = page.text;
        text.gameObject.SetActive(true);

        if (page.gif != null)
        {
            gifPlayer.Gif = page.gif;
            gifPlayer.gameObject.SetActive(true);
            tutorialImage.gameObject.SetActive(false);
        }
        // 이미지 업데이트
        else if (page.image != null)
        {
            tutorialImage.sprite = page.image;
            tutorialImage.gameObject.SetActive(true);
            gifPlayer.gameObject.SetActive(false);
        }
        else
        {
            tutorialImage.gameObject.SetActive(false);
            gifPlayer.gameObject.SetActive(false);
        }

        // 버튼 상태 업데이트
        previousPageButton.gameObject.SetActive(currentPage > 0);
        nextPageButton.gameObject.SetActive(currentPage < tutorialData[mode].Count - 1);

        // Dot 상태 업데이트
        UpdateDots();
    }

    private void OnClickOutPage()
    {
        Debug.Log("튜토리얼 종료 버튼 클릭됨");
        gameObject.SetActive(false);    // 튜토리얼 패널 닫기
        mainmenuPanel.SetActive(true);  // 메인 메뉴 패널 열기
    }
}

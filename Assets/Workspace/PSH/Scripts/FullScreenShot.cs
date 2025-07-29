using System.Collections;
using System.IO;
using UnityEngine;

public class FullScreenshot : MonoBehaviour
{
    [Header("스크린샷 설정")]
    [Tooltip("비어있으면 MainCamera를 자동으로 사용")]
    public Camera captureCamera;

    [Tooltip("저장할 파일 이름 (확장자 제외)")]
    public string fileName = "full_screenshot";

    [Tooltip("Application.persistentDataPath 하위에 생성할 폴더명 (빈 문자열 = 바로 경로)")]
    public string folderName = "";

    [Tooltip("시작할 때 한 번 자동 캡쳐할지 여부")]
    public bool captureOnStart = false;

    [Tooltip("지정 키를 누르면 캡쳐 (기본 F12)")]
    public KeyCode captureKey = KeyCode.F12;

    [Header("해상도 설정:")]
    [Tooltip("Screen.width, Screen.height 대신 사용할 가로 픽셀 수 (0 이면 화면 크기 사용)")]
    public int overrideWidth = 0;
    [Tooltip("Screen.width, Screen.height 대신 사용할 세로 픽셀 수 (0 이면 화면 크기 사용)")]
    public int overrideHeight = 0;
    [Tooltip("해상도를 Screen 크기에 곱할 스케일 (1.0 = 원래 크기)")]
    [Range(0.1f, 4f)]
    public float resolutionScale = 1f;

    private void Awake()
    {
        if (captureCamera == null)
            captureCamera = Camera.main;
    }

    private void Start()
    {
        if (captureOnStart)
            StartCoroutine(CaptureAndSave());
    }

    private void Update()
    {
        if (Input.GetKeyDown(captureKey))
            StartCoroutine(CaptureAndSave());
    }

    public IEnumerator CaptureAndSave()
    {
        yield return new WaitForEndOfFrame();

        // 1) 해상도 계산
        int baseW = Screen.width;
        int baseH = Screen.height;

        int width = overrideWidth > 0 ? overrideWidth : Mathf.RoundToInt(baseW * resolutionScale);
        int height = overrideHeight > 0 ? overrideHeight : Mathf.RoundToInt(baseH * resolutionScale);

        // 2) RenderTexture 생성 및 렌더
        RenderTexture rt = new RenderTexture(width, height, 24);
        captureCamera.targetTexture = rt;
        captureCamera.Render();

        // 3) ReadPixels로 텍스처 생성
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        // 4) 정리
        captureCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // 5) PNG 인코딩 & 저장
        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);

        string dir = string.IsNullOrEmpty(folderName)
            ? Application.persistentDataPath
            : Path.Combine(Application.persistentDataPath, folderName);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string path = Path.Combine(dir, $"{fileName}_{width}x{height}.png");
        File.WriteAllBytes(path, bytes);

        Debug.Log($"[FullScreenshot] 해상도 {width}×{height} 스크린샷 저장 완료 → {path}");
    }
}

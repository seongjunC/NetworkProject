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
        // 1프레임 끝날 때까지 대기
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;

        // 카메라 렌더링을 RenderTexture에
        RenderTexture rt = new RenderTexture(width, height, 24);
        captureCamera.targetTexture = rt;
        captureCamera.Render();

        // 텍스처로 읽어오기
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        // 정리
        captureCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // PNG 인코딩
        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);

        // 저장 경로 준비
        string dir = string.IsNullOrEmpty(folderName)
            ? Application.persistentDataPath
            : Path.Combine(Application.persistentDataPath, folderName);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string path = Path.Combine(dir, $"{fileName}.png");
        File.WriteAllBytes(path, bytes);

        Debug.Log($"[FullScreenshot] 스크린샷 저장 완료 → {path}");
    }
}

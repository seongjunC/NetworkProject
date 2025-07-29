using System.Collections;
using System.IO;
using UnityEngine;

public class FullScreenshot : MonoBehaviour
{
    [Header("��ũ���� ����")]
    [Tooltip("��������� MainCamera�� �ڵ����� ���")]
    public Camera captureCamera;

    [Tooltip("������ ���� �̸� (Ȯ���� ����)")]
    public string fileName = "full_screenshot";

    [Tooltip("Application.persistentDataPath ������ ������ ������ (�� ���ڿ� = �ٷ� ���)")]
    public string folderName = "";

    [Tooltip("������ �� �� �� �ڵ� ĸ������ ����")]
    public bool captureOnStart = false;

    [Tooltip("���� Ű�� ������ ĸ�� (�⺻ F12)")]
    public KeyCode captureKey = KeyCode.F12;

    [Header("�ػ� ����:")]
    [Tooltip("Screen.width, Screen.height ��� ����� ���� �ȼ� �� (0 �̸� ȭ�� ũ�� ���)")]
    public int overrideWidth = 0;
    [Tooltip("Screen.width, Screen.height ��� ����� ���� �ȼ� �� (0 �̸� ȭ�� ũ�� ���)")]
    public int overrideHeight = 0;
    [Tooltip("�ػ󵵸� Screen ũ�⿡ ���� ������ (1.0 = ���� ũ��)")]
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

        // 1) �ػ� ���
        int baseW = Screen.width;
        int baseH = Screen.height;

        int width = overrideWidth > 0 ? overrideWidth : Mathf.RoundToInt(baseW * resolutionScale);
        int height = overrideHeight > 0 ? overrideHeight : Mathf.RoundToInt(baseH * resolutionScale);

        // 2) RenderTexture ���� �� ����
        RenderTexture rt = new RenderTexture(width, height, 24);
        captureCamera.targetTexture = rt;
        captureCamera.Render();

        // 3) ReadPixels�� �ؽ�ó ����
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        // 4) ����
        captureCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // 5) PNG ���ڵ� & ����
        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);

        string dir = string.IsNullOrEmpty(folderName)
            ? Application.persistentDataPath
            : Path.Combine(Application.persistentDataPath, folderName);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string path = Path.Combine(dir, $"{fileName}_{width}x{height}.png");
        File.WriteAllBytes(path, bytes);

        Debug.Log($"[FullScreenshot] �ػ� {width}��{height} ��ũ���� ���� �Ϸ� �� {path}");
    }
}

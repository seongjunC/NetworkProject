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
        // 1������ ���� ������ ���
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;

        // ī�޶� �������� RenderTexture��
        RenderTexture rt = new RenderTexture(width, height, 24);
        captureCamera.targetTexture = rt;
        captureCamera.Render();

        // �ؽ�ó�� �о����
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        // ����
        captureCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // PNG ���ڵ�
        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);

        // ���� ��� �غ�
        string dir = string.IsNullOrEmpty(folderName)
            ? Application.persistentDataPath
            : Path.Combine(Application.persistentDataPath, folderName);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string path = Path.Combine(dir, $"{fileName}.png");
        File.WriteAllBytes(path, bytes);

        Debug.Log($"[FullScreenshot] ��ũ���� ���� �Ϸ� �� {path}");
    }
}

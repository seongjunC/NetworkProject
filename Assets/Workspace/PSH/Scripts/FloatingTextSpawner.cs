using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingTextSpawner : MonoBehaviour
{
    [SerializeField] private GameObject floatingTextPrefab;

    [SerializeField] private float moveSpeed = 50f;
    [SerializeField] float duration = 2f;

    public void SpawnText(string text, Vector3 pos)
    {
        if (floatingTextPrefab == null)
        {
            Debug.LogError("FloatingTextSpawner에 프리팹 또는 캔버스가 설정되지 않았습니다.");
            return;
        }

        // 프리팹 생성
        GameObject textObj = Instantiate(floatingTextPrefab, pos, Quaternion.identity);

        // 텍스트 내용과 색상 설정
        TextMeshProUGUI tmpro = textObj.GetComponentInChildren<TextMeshProUGUI>();
        if (tmpro != null)
        {
            Debug.Log("텍스트 변경 완료");
            tmpro.text = text;
        }

        // 코루틴 시작
        StartCoroutine(AnimateText(textObj, tmpro));
    }

    private IEnumerator AnimateText(GameObject textObj, TextMeshProUGUI tmpro)
    {
        float timer = 0f;

        while (timer < duration)
        {
            // 위로 이동
            textObj.transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

            timer += Time.deltaTime;
            yield return null;
        }

        // 지정된 시간이 지나면 오브젝트 파괴
        Destroy(textObj);
    }
}
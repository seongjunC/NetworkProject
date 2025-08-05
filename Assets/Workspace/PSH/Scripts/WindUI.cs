using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindUI : MonoBehaviour
{
    [SerializeField] private Slider windSlider;

    private void OnEnable()
    {
        // WindManager의 이벤트에 구독하여 바람이 바뀔 때마다 함수를 호출하도록 설정
        WindManager.OnWindChanged += UpdateWindDisplay;
    }

    private void OnDisable()
    {
        // 오브젝트가 비활성화될 때 구독 해제
        WindManager.OnWindChanged -= UpdateWindDisplay;
    }

    private void UpdateWindDisplay(Vector2 wind)
    {
        if (windSlider == null) return;

        windSlider.value = wind.x;      
    }
}

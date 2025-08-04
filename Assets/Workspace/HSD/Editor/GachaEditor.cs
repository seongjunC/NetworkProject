using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Gacha))]
public class GachaEditor : Editor
{
    // 색상을 설정 (색상이 부족하면 순환 사용됨)
    private readonly Color[] segmentColors = new Color[]
    {
        new Color(0.2f, 0.6f, 1f),    // Blue
        new Color(0.2f, 1f, 0.6f),    // Green
        new Color(1f, 0.5f, 0.2f),    // Orange
        new Color(1f, 0.2f, 0.2f),    // Red
        new Color(0.9f, 0.9f, 0.2f),  // Yellow
        new Color(0.6f, 0.4f, 1f),    // Purple
        new Color(0.8f, 0.8f, 0.8f),  // Gray
    };

    public override void OnInspectorGUI()
    {
        // 기본 인스펙터 그리기
        base.OnInspectorGUI();

        // 대상 스크립트 참조
        Gacha gacha = (Gacha)target;

        if (gacha.GachaData == null)
            return;

        float[] chance = new float[4];

        for (int i = 0; i < gacha.GachaData.GachaDatas.Length; i ++)
        {
            chance[i] = gacha.GachaData.GachaDatas[i].chance;
        }

        Rank[] ranks = new Rank[4];

        for (int i = 0; i < gacha.GachaData.GachaDatas.Length; i++)
        {
            ranks[i] = gacha.GachaData.GachaDatas[i].rank;
        }

        // chance 배열이 비어 있으면 그리지 않음
        if (chance == null || chance.Length == 0)
            return;

        // 유효한 chance 총합 계산 (0 이하 무시)
        float total = 0f;
        foreach (float c in chance)
            if (c > 0)
                total += c;

        if(ranks.Length != chance.Length)
        {
            Debug.Log("랭크와 확률의 수는 같아야합니다.");
            return;
        }

        // GUIStyle 지정
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        Color labelColor = Color.cyan;

        // 모든 상태에 동일한 색상 지정
        headerStyle.normal.textColor = labelColor;
        headerStyle.hover.textColor = labelColor;
        headerStyle.focused.textColor = labelColor;
        headerStyle.active.textColor = labelColor;

        // 폰트 크기 설정
        headerStyle.fontSize = 20;
        headerStyle.alignment = TextAnchor.MiddleLeft;

        // UI 여백 및 제목
        GUILayout.Space(10);
        GUILayout.Label("[가챠 확률]", headerStyle);

        // 바가 그려질 영역 Rect 생성
        Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth - 40, 25);
        float x = rect.x;
        float width = rect.width;

        float drawnPercent = 0f; // 현재까지 그려진 누적 비율 (0 ~ 1)

        for (int i = 0; i < chance.Length; i++)
        {
            float value = chance[i];

            // 0 이하 확률은 무시
            if (value <= 0)
                continue;

            // 해당 바의 비율 구하기
            float percent = value / total;

            // 누적 확률이 100% 초과되면 잘라서 그리기
            if (drawnPercent + percent > 1f)
                percent = 1f - drawnPercent;

            float segmentWidth = width * percent;

            // 더 이상 그릴 공간이 없으면 종료
            if (segmentWidth <= 0)
                break;

            Color color = segmentColors[i % segmentColors.Length];

            // 바 배경 그리기
            EditorGUI.DrawRect(new Rect(x, rect.y, segmentWidth, rect.height), color);

            // 텍스트 색상은 배경 색에 따라 자동 결정 (밝으면 검정, 어두우면 흰색)
            Color textColor = (color.r * 0.299f + color.g * 0.587f + color.b * 0.114f) > 0.5f
                ? Color.black
                : Color.white;

            // 텍스트 스타일 설정
            GUIStyle centeredStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = textColor },
                fontStyle = FontStyle.Bold
            };

            centeredStyle.normal.textColor = textColor;
            centeredStyle.hover.textColor = textColor;
            centeredStyle.focused.textColor = textColor;
            centeredStyle.active.textColor = textColor;

            // 텍스트 라벨 출력
            Rect labelRect = new Rect(x, rect.y + 5, segmentWidth, rect.height);
            string label = $"{Mathf.RoundToInt(percent * 100)}%";
            GUI.Label(labelRect, label, centeredStyle);

            // 랭크 라벨 출력
            Rect rankLabelRect = new Rect(x, rect.y + 20, segmentWidth, rect.height);

            // 폰트 크기를 바 넓이에 따라 유동적으로 설정
            int dynamicFontSize = Mathf.Clamp(Mathf.RoundToInt(segmentWidth * 0.2f), 8, 12);

            // 스타일 적용
            GUIStyle rankLabelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = dynamicFontSize
            };

            // 색상 지정
            textColor = GetRankColor(ranks[i]);
            rankLabelStyle.normal.textColor = textColor;
            rankLabelStyle.hover.textColor = textColor;
            rankLabelStyle.focused.textColor = textColor;
            rankLabelStyle.active.textColor = textColor;

            // 텍스트 출력
            GUI.Label(rankLabelRect, $"{ranks[i]} 확률", rankLabelStyle);

            // 다음 구간 시작점으로 이동
            x += segmentWidth;
            drawnPercent += percent;

            // 총 100% 이상이면 루프 종료
            if (drawnPercent >= 1f)
                break;
        }

        GUILayout.Space(10);
    }

    private Color GetRankColor(Rank rank)
    {
        return rank switch
        {
            Rank.S => new Color(1f, 0.5f, 0f),
            Rank.A => new Color(0.5f, 0.8f, 1f),
            Rank.B => new Color(0.6f, 1f, 0.6f),
            Rank.C => Color.gray,
            _ => Color.white
        };
    }
}
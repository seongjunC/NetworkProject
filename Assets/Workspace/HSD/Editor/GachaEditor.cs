using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Gacha))]
public class GachaEditor : Editor
{
    // ������ ���� (������ �����ϸ� ��ȯ ����)
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
        // �⺻ �ν����� �׸���
        base.OnInspectorGUI();

        // ��� ��ũ��Ʈ ����
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

        // chance �迭�� ��� ������ �׸��� ����
        if (chance == null || chance.Length == 0)
            return;

        // ��ȿ�� chance ���� ��� (0 ���� ����)
        float total = 0f;
        foreach (float c in chance)
            if (c > 0)
                total += c;

        if(ranks.Length != chance.Length)
        {
            Debug.Log("��ũ�� Ȯ���� ���� ���ƾ��մϴ�.");
            return;
        }

        // GUIStyle ����
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        Color labelColor = Color.cyan;

        // ��� ���¿� ������ ���� ����
        headerStyle.normal.textColor = labelColor;
        headerStyle.hover.textColor = labelColor;
        headerStyle.focused.textColor = labelColor;
        headerStyle.active.textColor = labelColor;

        // ��Ʈ ũ�� ����
        headerStyle.fontSize = 20;
        headerStyle.alignment = TextAnchor.MiddleLeft;

        // UI ���� �� ����
        GUILayout.Space(10);
        GUILayout.Label("[��í Ȯ��]", headerStyle);

        // �ٰ� �׷��� ���� Rect ����
        Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth - 40, 25);
        float x = rect.x;
        float width = rect.width;

        float drawnPercent = 0f; // ������� �׷��� ���� ���� (0 ~ 1)

        for (int i = 0; i < chance.Length; i++)
        {
            float value = chance[i];

            // 0 ���� Ȯ���� ����
            if (value <= 0)
                continue;

            // �ش� ���� ���� ���ϱ�
            float percent = value / total;

            // ���� Ȯ���� 100% �ʰ��Ǹ� �߶� �׸���
            if (drawnPercent + percent > 1f)
                percent = 1f - drawnPercent;

            float segmentWidth = width * percent;

            // �� �̻� �׸� ������ ������ ����
            if (segmentWidth <= 0)
                break;

            Color color = segmentColors[i % segmentColors.Length];

            // �� ��� �׸���
            EditorGUI.DrawRect(new Rect(x, rect.y, segmentWidth, rect.height), color);

            // �ؽ�Ʈ ������ ��� ���� ���� �ڵ� ���� (������ ����, ��ο�� ���)
            Color textColor = (color.r * 0.299f + color.g * 0.587f + color.b * 0.114f) > 0.5f
                ? Color.black
                : Color.white;

            // �ؽ�Ʈ ��Ÿ�� ����
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

            // �ؽ�Ʈ �� ���
            Rect labelRect = new Rect(x, rect.y + 5, segmentWidth, rect.height);
            string label = $"{Mathf.RoundToInt(percent * 100)}%";
            GUI.Label(labelRect, label, centeredStyle);

            // ��ũ �� ���
            Rect rankLabelRect = new Rect(x, rect.y + 20, segmentWidth, rect.height);

            // ��Ʈ ũ�⸦ �� ���̿� ���� ���������� ����
            int dynamicFontSize = Mathf.Clamp(Mathf.RoundToInt(segmentWidth * 0.2f), 8, 12);

            // ��Ÿ�� ����
            GUIStyle rankLabelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = dynamicFontSize
            };

            // ���� ����
            textColor = GetRankColor(ranks[i]);
            rankLabelStyle.normal.textColor = textColor;
            rankLabelStyle.hover.textColor = textColor;
            rankLabelStyle.focused.textColor = textColor;
            rankLabelStyle.active.textColor = textColor;

            // �ؽ�Ʈ ���
            GUI.Label(rankLabelRect, $"{ranks[i]} Ȯ��", rankLabelStyle);

            // ���� ���� ���������� �̵�
            x += segmentWidth;
            drawnPercent += percent;

            // �� 100% �̻��̸� ���� ����
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
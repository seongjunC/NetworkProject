using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public partial class GameData
{
    public float bgm;
    public float sfx;

    public GameData()
    {
        bgm = -9f; // �� ��
        sfx = 0f;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Image tankIcon;
    [SerializeField] Image rankImage;
    [SerializeField] Button cardButton;
    [SerializeField] TMP_Text tankName;
    [SerializeField] Animator anim;

    [SerializeField] TankData tankData;

    [Header("Color")]
    [SerializeField] Color sColor;
    [SerializeField] Color aColor;
    [SerializeField] Color bColor;
    [SerializeField] Color cColor;

    public void SetUp(TankData tankData, Transform targetTransform, float moveTime)
    {
        this.tankData = tankData;

        tankName.text = this.tankData.tankName;
        tankIcon.sprite = this.tankData.icon;
        rankImage.color = GetRankColor(this.tankData.rank);

        StartCoroutine(MoveRoutine(targetTransform, moveTime));
    }

    private Color GetRankColor(TankRank rank)
    {
        return (rank) switch
        {
            TankRank.C => cColor,
            TankRank.B => bColor,
            TankRank.A => aColor,
            TankRank.S => sColor,
            _ => cColor,
        };
    }

    private void Open()
    {
        cardButton.interactable = false;
    }

    private IEnumerator MoveRoutine(Transform targetTransform, float moveTime)
    {
        float timer = 0;
        Vector2 start = transform.position;
        Vector3 end = targetTransform.position;

        while (timer < moveTime)
        {
            transform.position = Vector2.Lerp(start, end, timer / moveTime);
            timer += Time.deltaTime;

            yield return null;
        }

        transform.position = end;
    }
}

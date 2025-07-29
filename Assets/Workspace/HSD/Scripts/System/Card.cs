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
    [SerializeField] Button card;
    [SerializeField] TMP_Text tankName;
    [SerializeField] Animator anim;

    [SerializeField] TankData tankData;

    [Header("Color")]
    [SerializeField] Color sColor;
    [SerializeField] Color aColor;
    [SerializeField] Color bColor;
    [SerializeField] Color cColor;

    public void SetUp(TankData _tankData, Transform _transform, float _moveTime)
    {
        tankData = _tankData;

        tankName.text = tankData.tankName;
        tankIcon.sprite = tankData.icon;
        rankImage.color = GetRankColor(tankData.rank);

        StartCoroutine(MoveRoutine(_transform, _moveTime));
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
        anim.SetTrigger("Open");
        card.interactable = false;
    }

    private IEnumerator MoveRoutine(Transform _transform, float _moveTime)
    {
        float timer = 0;
        Vector2 start = transform.position;

        while (timer < _moveTime)
        {
            Vector2.Lerp(start, _transform.position, timer / _moveTime);
            timer += Time.deltaTime;

            yield return null;
        }
    }
}

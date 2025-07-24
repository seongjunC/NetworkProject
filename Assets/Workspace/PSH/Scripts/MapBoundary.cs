using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBoundary : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        //플레이어가 맵 밖을 나갈 경우
        if (collision.CompareTag("Player"))
        {
            //플레이어 낙사 판정
        }
        //투사체가 맵 밖을 나갈 경우
        else if (collision.CompareTag("Projectile"))
        {
            //투사체 삭제
        }
    }
}

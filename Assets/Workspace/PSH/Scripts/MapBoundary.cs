using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBoundary : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        //�÷��̾ �� ���� ���� ���
        if (collision.CompareTag("Player"))
        {
            //�÷��̾� ���� ����
        }
        //����ü�� �� ���� ���� ���
        else if (collision.CompareTag("Projectile"))
        {
            //����ü ����
        }
    }
}

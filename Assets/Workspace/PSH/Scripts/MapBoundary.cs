using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBoundary : MonoBehaviour
{
    public SpriteRenderer mapSpriteRenderer;

    public float padding = 2f;

    private BoxCollider2D boxCollider;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();

    }
    private void Start()
    {
        DeformableTerrain terrain = FindObjectOfType<DeformableTerrain>();
        if (terrain != null)
        {
            mapSpriteRenderer = terrain.GetComponent<SpriteRenderer>();

            if (mapSpriteRenderer == null)
            {
                Debug.Log("spriterenderer ����");
                return;
            }
        }
        else
        {
            Debug.Log("deformableterrian ����");
        }

        ResizeCollider();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //�÷��̾ �� ���� ���� ���
        if (collision.CompareTag("Player"))
        {
            //�÷��̾� ���� ����
        }
        //����ü�� �� ���� ���� ���
        else if (collision.CompareTag("Bullet"))
        {
            Projectile p = collision.GetComponent<Projectile>();

            if (p != null)
            {
                p.BeginDestroyRoutine(false);
                Debug.Log("����ü�� �� ������ ����");
            }

        }
    }

    public void ResizeCollider()
    {
        transform.position = mapSpriteRenderer.transform.position;
        Vector2 mapSize = mapSpriteRenderer.bounds.size;
        boxCollider.size = mapSize + new Vector2(padding,padding);
        boxCollider.offset = Vector2.zero;
        Debug.Log("�� ũ�⿡ �°� Ʈ���� ���� ���� �Ϸ�");
    }

}

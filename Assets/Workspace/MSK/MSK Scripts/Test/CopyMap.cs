using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyMap : MonoBehaviour
{
    public SpriteRenderer mapSpriteRenderer;

    public float padding = 2f;

    private BoxCollider2D boxCollider;
    private void Start() { }
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();

    }
    private void OnEnable()
    {
        MapManager.OnMapLoaded += OnMapGenerated;
    }

    private void OnDisable()
    {
        MapManager.OnMapLoaded -= OnMapGenerated;
    }

    private void OnMapGenerated(DeformableTerrain terrain)
    {
        terrain = FindObjectOfType<DeformableTerrain>();
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
            //ū �������� �༭ ����
            var p = collision.GetComponent<PlayerController>();
            p.OnHit(9999999);
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
        boxCollider.size = mapSize + new Vector2(padding, padding);
        boxCollider.offset = Vector2.zero;
        Debug.Log("�� ũ�⿡ �°� Ʈ���� ���� ���� �Ϸ�");
    }
    private void OnDrawGizmos()
    {
        if (mapSpriteRenderer == null)
            return;

        // �ö��̴� ũ��/�������� �ֽ��̶��, Awake/Start���� ResizeCollider()�� �� �� ȣ���صμ���
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();

        // Gizmo ����
        Gizmos.color = Color.green;

        // ���� ��ġ ��� (Transform ��ġ + �ݶ��̴� offset)
        Vector2 center = (Vector2)transform.position + boxCollider.offset;
        Vector2 size = boxCollider.size;

        // 2D �ڽ� �׸���
        Gizmos.DrawWireCube(center, size);
    }
}

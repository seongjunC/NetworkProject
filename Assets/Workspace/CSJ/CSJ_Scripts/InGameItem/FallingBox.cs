using System.Collections;
using System.Collections.Generic;
using ParrelSync.NonCore;
using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;

public class FallingBox : MonoBehaviourPun
{
    [SerializeField] private ItemData itemData;
    private float fallSpeed;
    private float swayAmp;
    private float swayFreq;

    private Vector3 startPos;
    private bool hasLanded;
    private bool FirstLanded;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDist = 0.5f;
    [SerializeField] private float playerCheckDist = 0.5f;

    private Collider2D triggerCol;

    void Awake()
    {
        triggerCol = GetComponent<BoxCollider2D>();
    }

    [PunRPC]
    public void Init(ItemData data, float speed, float amp, float freq)
    {
        itemData = data;
        fallSpeed = speed;
        swayAmp = amp;
        swayFreq = freq;
        startPos = transform.position;


        triggerCol.isTrigger = false;
    }

    void Update()
    {
        bool groundBelow = Physics2D.Raycast
        (transform.position, Vector2.down, groundCheckDist, groundLayer)
        .collider != null;

        if (!groundBelow)
        {
            hasLanded = false;
            triggerCol.isTrigger = false;

            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            if (!FirstLanded)
            {
                float dx = swayAmp * Mathf.Sin(Time.time * swayFreq);
                transform.position = new Vector3(startPos.x + dx,
                                                transform.position.y,
                                                startPos.z);
            }
            return;
        }

        if (!hasLanded)
        {
            Land();
            return;
        }

        var hits = Physics2D.OverlapCircleAll(transform.position, playerCheckDist);
        foreach (var h in hits)
        {
            if (!h.CompareTag("Player")) continue;

            var pc = h.GetComponent<PlayerController>();
            if (pc != null)
            {
                if (TryAcquire(pc)) break;

            }
        }
    }

    private void Land()
    {
        hasLanded = true;
        FirstLanded = true;
        triggerCol.isTrigger = true;
    }

    private bool TryAcquire(PlayerController pc)
    {
        if (!pc.HasFreeItemSlot()) return false;

        bool isAcquired = pc.TryAcquireItem(itemData);
        if (!isAcquired) return false;

        photonView.RPC(nameof(RPC_OpenBox), RpcTarget.All);
        return true;
    }

    [PunRPC]
    private void RPC_OpenBox()
    {
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = hasLanded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, playerCheckDist);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDist);
    }
}

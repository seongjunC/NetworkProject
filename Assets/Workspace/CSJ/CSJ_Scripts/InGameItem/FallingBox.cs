using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private ItemDatabase itemCandidate;

    private Collider2D triggerCol;

    void Awake()
    {
        triggerCol = GetComponent<BoxCollider2D>();
    }

    [PunRPC]
    public void Init(string id, float speed, float amp, float freq)
    {
        itemData = itemCandidate.Get(id);
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
            if (pc != null && pc.photonView.IsMine)
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
        Debug.Log("아이템 획득 체크");
        if (!pc.HasFreeItemSlot()) return false;

        bool isAcquired = pc.TryAcquireItem(itemData);
        Debug.Log($"아이템 획득 시도, {isAcquired}");
        if (!isAcquired) return false;

        Debug.Log($"{pc.myInfo.NickName}이 {itemData.name}을 획득하였습니다.");
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (MSKTurnController.Instance == null || MSKTurnController.Instance.isGameEnd)
            return;
        if (collision.CompareTag("MapBoundary"))
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (MSKTurnController.Instance != null)
            MSKTurnController.Instance.OnItemDestroyed(gameObject);
    }
}

using UnityEngine;
using Photon.Pun;
using System;

public class WindManager : MonoBehaviourPunCallbacks
{
    public static WindManager Instance { get; private set; }

    // 현재 바람 정보 (x: 방향 및 세기, y: 0)
    public Vector2 CurrentWind { get; private set; }

    [Header("바람 설정")]
    [SerializeField] private float maxWindStrength = 10f; // 최대 바람 세기

    // 바람 정보가 변경될 때 UI를 업데이트하기 위한 이벤트
    public static event Action<Vector2> OnWindChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GenerateNewWind()
    {
        // 마스터 클라이언트가 아니면 실행하지 않음
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        // -max ~ +max 사이의 랜덤한 바람 세기 결정
        float windX = UnityEngine.Random.Range(-maxWindStrength, maxWindStrength);
        Vector2 newWind = new Vector2(windX, 0);

        // 모든 클라이언트에게 새로운 바람 정보를 전송하는 RPC 호출
        photonView.RPC("RPC_SetWind", RpcTarget.All, newWind);
    }

    [PunRPC]
    private void RPC_SetWind(Vector2 newWind)
    {
        CurrentWind = newWind;
        Debug.Log($"바람 변경됨: {CurrentWind}");

        // UI 업데이트를 위해 이벤트를 발생시킴
        OnWindChanged?.Invoke(CurrentWind);
    }
}
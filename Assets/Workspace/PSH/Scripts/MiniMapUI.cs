using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapUI : MonoBehaviourPunCallbacks
{
    [Header("References")]
    public DeformableTerrain terrain;
    public RawImage miniMapRaw;
    public RectTransform playerIconPrefab;

    // 플레이어의 ActorNumber를 키로 사용하여 플레이어 정보와 아이콘을 관리
    private Dictionary<int, (PlayerController player, RectTransform icon)> 
        _playerIcons = new Dictionary<int, (PlayerController player, RectTransform icon)>();

    private Vector3 min;
    private Vector3 size;
    private bool _isInit = false;
    private IEnumerator Start()
    {
        yield return null;
        yield return null;

        if (terrain == null)
            terrain = FindObjectOfType<DeformableTerrain>();

        Invoke(nameof(InitMiniMap), .5f);

    }

    private void InitMiniMap()
    {
        if (terrain == null)
        {
            Debug.LogError("DeformableTerrain 못찾음");
            return;
        }

        // 미니맵 설정
        miniMapRaw.texture = terrain.deformableTexture;
        miniMapRaw.uvRect = new Rect(0, 0, 1, 1);

        // 맵 경계 계산
        var spriteRenderer = terrain.GetComponent<SpriteRenderer>();
        min = spriteRenderer.bounds.min;
        size = spriteRenderer.bounds.size;

        // 현재 방에 있는 플레이어 아이콘 생성
        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();
        foreach (var player in allPlayers)
        {
            AddPlayerIcon(player);
        }

        _isInit = true;
    }
    void LateUpdate()
    {
        if (!_isInit) return;

        var destroyedPlayers = new List<int>();

        foreach (var entry in _playerIcons)
        {
            if (entry.Value.player != null && entry.Value.player.gameObject.activeInHierarchy)
            {
                UpdateIconPosition(entry.Value.player, entry.Value.icon);
            }
            else
            {
                destroyedPlayers.Add(entry.Key);
            }
        }

        foreach (int actorNumber in destroyedPlayers)
        {
            RemovePlayerIcon(actorNumber);
        }
    }

    private void UpdateIconPosition(PlayerController player, RectTransform icon)
    {
        // 플레이어 월드 좌표
        Vector3 playerWorldPos = player.transform.position;

        float u = (playerWorldPos.x - min.x) / size.x;
        float v = (playerWorldPos.y - min.y) / size.y;

        u = Mathf.Clamp01(u);
        v = Mathf.Clamp01(v);

        // RawImage 크기
        Rect rect = miniMapRaw.rectTransform.rect;
        float px = u * rect.width;
        float py = v * rect.height;

        icon.anchoredPosition = new Vector2(px, py);
    }

    private void AddPlayerIcon(PlayerController player)
    {
        if (player == null || player.photonView == null || player.photonView.Owner == null) return;

        int actorNumber = player.photonView.Owner.ActorNumber;

        //아이콘 중복 생성 방지
        if (_playerIcons.ContainsKey(actorNumber)) return;

        //아이콘 프리팹으로 새로운 아이콘 생성
        RectTransform newIcon = Instantiate(playerIconPrefab, miniMapRaw.transform);
        newIcon.gameObject.SetActive(true);
        _playerIcons.Add(actorNumber, (player, newIcon));

        //팀에 따라 색상 변경
        Image iconImage = newIcon.GetComponent<Image>();
        if (iconImage != null)
        {
            bool isMyTeam = player.photonView.Owner.GetTeam() == PhotonNetwork.LocalPlayer.GetTeam();
            iconImage.color = isMyTeam ? Color.green : Color.red;

            if (player.photonView.IsMine)
            {
                iconImage.color = Color.yellow;
            }
        }

    }

    private void RemovePlayerIcon(int actorNumber)
    {
        if (_playerIcons.TryGetValue(actorNumber, out var entry))
        {
            if (entry.icon != null)
            {
                Destroy(entry.icon.gameObject);
            }
            _playerIcons.Remove(actorNumber);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        StartCoroutine(AddIconAfterDelay(newPlayer));
    }

    private IEnumerator AddIconAfterDelay(Player newPlayer)
    {
        yield return new WaitForSeconds(1f);

        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();

        foreach (var p in allPlayers)
        {
            if (p.photonView.Owner.ActorNumber == newPlayer.ActorNumber)
            {
                AddPlayerIcon(p);
                break;
            }
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayerIcon(otherPlayer.ActorNumber);
    }
}

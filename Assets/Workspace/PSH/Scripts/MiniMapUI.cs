using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapUI : MonoBehaviourPunCallbacks
{
    [Header("참조 설정")]
    public RawImage miniMapRaw;
    public RectTransform playerIconPrefab;

    private DeformableTerrain terrainFromManager;
    private readonly Dictionary<int, (PlayerController player, RectTransform icon)> _playerIcons = new Dictionary<int, (PlayerController, RectTransform)>();

    private Vector3 _mapMinBounds;
    private Vector3 _mapSize;
    private bool _isInitialized = false;

    public override void OnEnable()
    {
        base.OnEnable();
        Debug.Log("[MiniMapUI] 활성화됨. OnMapLoaded 이벤트 구독을 시작합니다.");
        MapManager.OnMapLoaded += InitializeMiniMap;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        Debug.Log("[MiniMapUI] 비활성화됨. OnMapLoaded 이벤트 구독을 해제합니다.");
        MapManager.OnMapLoaded -= InitializeMiniMap;
    }

    private void InitializeMiniMap(DeformableTerrain terrainFromManager)
    {
        Debug.Log("[MiniMapUI] MapManager로부터 OnMapLoaded 이벤트를 받았습니다.");

        if (terrainFromManager == null)
        {
            Debug.LogError("[MiniMapUI] 전달받은 DeformableTerrain이 null입니다! 초기화를 중단합니다.");
            return;
        }
       

        if (miniMapRaw == null)
        {
            Debug.LogError("[MiniMapUI] RawImage(miniMapRaw)가 할당되지 않았습니다! 인스펙터에서 할당해주세요.");
            return;
        }

        if (playerIconPrefab == null)
        {
            Debug.LogError("[MiniMapUI] 플레이어 아이콘 프리팹(playerIconPrefab)이 할당되지 않았습니다! 인스펙터에서 할당해주세요.");
            return;
        }

        miniMapRaw.texture = terrainFromManager.deformableTexture;
        miniMapRaw.uvRect = new Rect(0, 0, 1, 1);

        var spriteRenderer = terrainFromManager.GetComponent<SpriteRenderer>();
        _mapMinBounds = spriteRenderer.bounds.min;
        _mapSize = spriteRenderer.bounds.size;

        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();
        Debug.Log($"[MiniMapUI] 현재 씬에 있는 플레이어 {allPlayers.Length}명을 찾았습니다. 아이콘을 생성합니다.");
        foreach (var player in allPlayers)
        {
            AddPlayerIcon(player);
        }

        _isInitialized = true;
        Debug.Log("[MiniMapUI] 미니맵 초기화 성공!");
    }

    void LateUpdate()
    {
        if (!_isInitialized) return;

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
        Vector3 playerWorldPos = player.transform.position;
        float u = (playerWorldPos.x - _mapMinBounds.x) / _mapSize.x;
        float v = (playerWorldPos.y - _mapMinBounds.y) / _mapSize.y;
        u = Mathf.Clamp01(u);
        v = Mathf.Clamp01(v);
        Rect rect = miniMapRaw.rectTransform.rect;
        icon.anchoredPosition = new Vector2(u * rect.width, v * rect.height);
    }

    private void AddPlayerIcon(PlayerController player)
    {
        if (player == null || player.photonView == null || player.photonView.Owner == null) return;
        int actorNumber = player.photonView.Owner.ActorNumber;
        if (_playerIcons.ContainsKey(actorNumber)) return;

        RectTransform newIcon = Instantiate(playerIconPrefab, miniMapRaw.transform);
        newIcon.gameObject.SetActive(true);
        _playerIcons.Add(actorNumber, (player, newIcon));

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
            if (entry.icon != null) Destroy(entry.icon.gameObject);
            _playerIcons.Remove(actorNumber);
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (!_isInitialized) return;
        StartCoroutine(AddIconForNewPlayer(newPlayer));
    }

    private System.Collections.IEnumerator AddIconForNewPlayer(Photon.Realtime.Player newPlayer)
    {
        yield return new WaitForSeconds(1.0f);
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

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (!_isInitialized) return;
        RemovePlayerIcon(otherPlayer.ActorNumber);
    }
}
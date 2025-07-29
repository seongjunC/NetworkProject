using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap2 : MonoBehaviourPunCallbacks
{
    public RawImage miniMapRaw;
    public RectTransform playerIconPrefab;

    private DeformableTerrain terrain;
    private readonly Dictionary<int, (PlayerController player, RectTransform icon)> _playerIcons = new Dictionary<int, (PlayerController, RectTransform)>();

    private Vector3 _mapMinBounds;
    private Vector3 _mapSize;
    private bool _isInitialized = false;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);

        terrain = FindObjectOfType<DeformableTerrain>();
        miniMapRaw.texture = terrain.deformableTexture;
        miniMapRaw.uvRect = new Rect(0, 0, 1, 1);

        _mapMinBounds = terrain.GetComponent<SpriteRenderer>().bounds.min;
        _mapSize = terrain.GetComponent<SpriteRenderer>().bounds.size;

        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();
        foreach (var player in allPlayers)
        {
            AddPlayerIcon(player);
        }

        _isInitialized = true;
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
            Game.Team team = player.photonView.Owner.GetTeam();
            iconImage.color = team == Game.Team.Blue ? Color.blue : Color.red;
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

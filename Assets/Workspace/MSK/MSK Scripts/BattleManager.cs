using System.Collections.Generic;
using System.Linq;
using Game;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class TestBattleManager : MonoBehaviourPun
{
    [SerializeField] Button _turnEndButton;
    [SerializeField] private MSKTurnController _turnController;

    private PlayerController _playerController;
    [SerializeField] private InGameUI inGameUI;
    public int redRemain { get; private set; }
    public int blueRemain { get; private set; }
    private bool _gameEnded = false;
    [SerializeField] private List<float> MVPScoreList = new();


    #region Unity LifeCycle
    private void Awake()
    {
        InitRemains();

        _turnEndButton.onClick.AddListener(TestTurnEnd);
        _turnController.OnPlayerDied += HandlePlayerDied;
    }

    #endregion

    public void TestTurnEnd()
    {
        if (_playerController != null)
            _playerController.EndPlayerTurn();

        if (_turnController.IsMyTurn())
            _turnController.TurnFinished();
    }

    private void InitRemains()
    {
        redRemain = PhotonNetwork.PlayerList.Count(p => p.GetTeam() == Team.Red);
        blueRemain = PhotonNetwork.PlayerList.Count(p => p.GetTeam() == Team.Blue);
        Debug.Log($"[BattleManager] 초기 생존자 R:{redRemain}, B:{blueRemain}");
    }

    private void HandlePlayerDied(PlayerController victim, PlayerInfo killerInfo)
    {
        var team = CustomProperty.GetTeam(victim.photonView.Owner);
        if (team == Game.Team.Red) redRemain--;
        else blueRemain--;

        killerInfo.RecordKillCount();
        Debug.Log($"[BattleManager] {killerInfo.NickName}가 {victim.myInfo.NickName} 처치. 남은 R:{redRemain}, B:{blueRemain}");

        if (!_gameEnded && (blueRemain == 0 || redRemain == 0))
        {
            _gameEnded = true;
            Team winnerTeam = redRemain == 0 ? Team.Blue : Team.Red;

            int mvpActor = ReturnMVP(winnerTeam);

            Debug.Log($"게임 종료!\n {(winnerTeam == Team.Red ? "레드" : "블루")}팀의 승리");
            photonView.RPC(nameof(_turnController.RPC_GameEnded), RpcTarget.All, winnerTeam, mvpActor);
        }

        Debug.Log($"GameEndCheck : 블루팀 : {blueRemain} , 레드팀 : {redRemain}");
    }

    private int ReturnMVP(Team WinnerTeam)
    {
        PlayerInfo mvp = null;
        float mvpScore = float.MinValue;
        Debug.Log($"AllPlayer : {_turnController.allPlayers.Count}");
        foreach (PlayerInfo p in _turnController.allPlayers.Values)
        {
            if (p.player.GetTeam() != WinnerTeam) continue;
            float playerScore = 0;
            playerScore += Mathf.Min(p.damageDealt * 0.01f, 50) + p.KillCount * 5;
            if (_turnController.DeadPlayer.Contains(p.ActorNumber)) playerScore -= 10;

            MVPScoreList.Add(playerScore);
            Debug.Log($"{p.NickName}의 score : {playerScore}");

            if (playerScore > mvpScore)
            {
                mvpScore = playerScore;
                mvp = p;
            }
            else if (Mathf.Approximately(playerScore, mvpScore))
            {
                if (Random.value < 0.5f)
                {
                    mvp = p;
                }
            }
        }
        return mvp.player.ActorNumber;
    }

    //     private void PlayerAttacked()
    //     {
    //         if (_playerController._hp <= 0)
    //         {
    //             // 사망 처리
    //             _turnController.photonView.RPC("RPC_PlayerDead", RpcTarget.MasterClient);
    //             // 죽은 유저가 자신의 턴이었다면, 턴도 종료
    //             if (_turnController.IsMyTurn())
    //             {
    //                 _turnController.TurnFinished();
    //             }
    //             return;
    //         }
    //         // 살아있고 내 턴이면 턴 종료
    //         if (_turnController.IsMyTurn())
    //         {
    //             _turnController.TurnFinished();
    //         }
    // 
    //     }

    public void RegisterPlayer(PlayerController playerController)
    {
        if (_playerController != null)
            _playerController.OnPlayerAttacked = null;
        _playerController = playerController;
        //_playerController.OnPlayerAttacked += PlayerAttacked;
        if (!playerController.photonView.IsMine) return;

        inGameUI.RegisterPlayer(playerController);

        Debug.Log($"▶ TestBattleManager ▶ {inGameUI.gameObject.name} 구독 완료!");
    }

    private void OnDestroy()
    {

        _turnEndButton.onClick.RemoveAllListeners();
        _turnController.OnPlayerDied -= HandlePlayerDied;
    }
}

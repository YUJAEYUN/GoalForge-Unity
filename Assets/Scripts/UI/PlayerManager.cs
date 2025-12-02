using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    // 팀 인덱스 (GameManager에서 프리팹 찾을 때 사용)
    public int player1TeamIndex = -1;
    public int player2TeamIndex = -1;

    // 팀 이름 (CoinToss, UI 텍스트 등에 사용)
    public string player1Team;  // 예: "KOREA"
    public string player2Team;  // 예: "PORTUGAL"

    // 동전 던지기 결과: true면 1P가 왼쪽 진영
    public bool player1IsLeftSide;

    // 점수 (원하면 진행 중 점수로도 쓸 수 있음)
    public int player1Score = 0;
    public int player2Score = 0;

    // ✅ 결과 씬에서 사용할 "마지막 경기 스코어"
    public int lastP1Score = 0;
    public int lastP2Score = 0;

    public enum SelectingPlayer { Player1, Player2 }
    public SelectingPlayer currentSelectingPlayer = SelectingPlayer.Player1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 🔥 팀 "인덱스 + 이름" 둘 다 저장 (TeamSelectController에서 사용)
    public void SelectTeamIndex(int index, string teamName)
    {
        if (currentSelectingPlayer == SelectingPlayer.Player1)
        {
            player1TeamIndex = index;
            player1Team = teamName;
            currentSelectingPlayer = SelectingPlayer.Player2;
        }
        else
        {
            player2TeamIndex = index;
            player2Team = teamName;
        }
    }

    // 동전 던지기 결과 저장 (CoinTossController에서 사용)
    public void SetCoinTossResult(bool player1IsLeft)
    {
        player1IsLeftSide = player1IsLeft;
    }

    // ✅ GameManager에서 경기 끝났을 때 호출해서 최종 점수 저장
    public void SetLastMatchScore(int p1, int p2)
    {
        player1Score = p1;
        player2Score = p2;

        lastP1Score = p1;
        lastP2Score = p2;
    }

    // 새 게임 시작할 때 값 초기화
    public void ResetGame()
    {
        player1TeamIndex = -1;
        player2TeamIndex = -1;
        player1Team = "";
        player2Team = "";

        player1Score = 0;
        player2Score = 0;
        lastP1Score = 0;
        lastP2Score = 0;

        player1IsLeftSide = false;
        currentSelectingPlayer = SelectingPlayer.Player1;
    }
}

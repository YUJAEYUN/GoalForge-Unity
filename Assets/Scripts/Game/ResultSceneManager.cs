using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultSceneManager : MonoBehaviour
{
    [Header("Team Sprites (팀 선택 UI에 사용한 같은 순서로 넣기)")]
    [SerializeField] private Sprite[] teamSprites;

    [Header("Portrait Images (캐릭터 UI 이미지)")]
    [SerializeField] private Image leftPortraitImage;
    [SerializeField] private Image rightPortraitImage;

    [Header("Team Name Text")]
    [SerializeField] private TextMeshProUGUI leftTeamNameText;
    [SerializeField] private TextMeshProUGUI rightTeamNameText;

    [Header("1P, 2P Text")]
    [SerializeField] private TextMeshProUGUI leftTeamText;
    [SerializeField] private TextMeshProUGUI rightTeamText;

    [Header("Bubble Images (말풍선 UI 이미지)")]
    [SerializeField] private Image leftBubbleImage;
    [SerializeField] private Image rightBubbleImage;

    [SerializeField] private Sprite winBubbleSprite;
    [SerializeField] private Sprite loseBubbleSprite;
    [SerializeField] private Sprite drawBubbleSprite;

    [Header("Score Text (왼쪽 / 오른쪽 점수 TMP)")]
    [SerializeField] private TextMeshProUGUI leftScoreText;
    [SerializeField] private TextMeshProUGUI rightScoreText;

    [Header("Team Background Images")]
    [SerializeField] private Image leftScoreBackground;
    [SerializeField] private Image rightScoreBackground;

    // 승리 / 패배 색상
    private Color loseColor = new Color32(128, 128, 128, 255);
    private Color winColor = Color.white;

    private void Start()
    {
        ApplyTeamSpritesAndNames();
        SetupResultUI();
    }

    // ================================================
    // 1) 선택한 팀의 스프라이트 + 팀 이름 UI 적용
    // ================================================
    private void ApplyTeamSpritesAndNames()
    {
        if (PlayerManager.Instance == null) return;

        int p1Index = PlayerManager.Instance.player1TeamIndex;
        int p2Index = PlayerManager.Instance.player2TeamIndex;

        string p1Name = PlayerManager.Instance.player1Team;
        string p2Name = PlayerManager.Instance.player2Team;

        // 팀 스프라이트 적용
        if (IsValidSpriteIndex(p1Index))
            leftPortraitImage.sprite = teamSprites[p1Index];

        if (IsValidSpriteIndex(p2Index))
            rightPortraitImage.sprite = teamSprites[p2Index];

        // 팀 이름 텍스트 적용
        if (leftTeamNameText != null)
            leftTeamNameText.text = p1Name;

        if (rightTeamNameText != null)
            rightTeamNameText.text = p2Name;
    }

    private bool IsValidSpriteIndex(int idx)
    {
        return idx >= 0 && idx < teamSprites.Length && teamSprites[idx] != null;
    }

    // ================================================
    // 2) 승패 점수 기반 UI 적용
    // ================================================
    private void SetupResultUI()
    {
        if (PlayerManager.Instance == null) return;

        int p1 = PlayerManager.Instance.lastP1Score;
        int p2 = PlayerManager.Instance.lastP2Score;

        leftScoreText.text = p1.ToString();
        rightScoreText.text = p2.ToString();

        if (p1 > p2) SetWinLose(true);
        else if (p2 > p1) SetWinLose(false);
        else SetDraw();
    }

    // ================================================
    // 승리 / 패배 UI 설정
    // ================================================
    private void SetWinLose(bool isLeftWinner)
    {
        // 말풍선
        leftBubbleImage.sprite = isLeftWinner ? winBubbleSprite : loseBubbleSprite;
        rightBubbleImage.sprite = isLeftWinner ? loseBubbleSprite : winBubbleSprite;

        // 캐릭터 색
        leftPortraitImage.color = isLeftWinner ? winColor : loseColor;
        rightPortraitImage.color = isLeftWinner ? loseColor : winColor;

        // 팀 이름 텍스트 색
        leftTeamNameText.color = isLeftWinner ? winColor : loseColor;
        rightTeamNameText.color = isLeftWinner ? loseColor : winColor;

        // 1P, 2P 텍스트 색
        leftTeamText.color = isLeftWinner ? winColor : loseColor;
        rightTeamText.color = isLeftWinner ? loseColor : winColor;

        // 스코어 배경
        leftScoreBackground.color = isLeftWinner ? winColor : loseColor;
        rightScoreBackground.color = isLeftWinner ? loseColor : winColor;

        // 스코어 텍스트 색
        leftScoreText.color = winColor;
        rightScoreText.color = winColor;
    }

    // ================================================
    // 무승부 UI 처리
    // ================================================
    private void SetDraw()
    {
        leftBubbleImage.sprite = drawBubbleSprite;
        rightBubbleImage.sprite = drawBubbleSprite;

        leftPortraitImage.color = winColor;
        rightPortraitImage.color = winColor;

        leftTeamNameText.color = winColor;
        rightTeamNameText.color = winColor;

        leftTeamText.color = winColor;
        rightTeamText.color = winColor;

        leftScoreBackground.color = winColor;
        rightScoreBackground.color = winColor;

        leftScoreText.color = winColor;
        rightScoreText.color = winColor;
    }
}

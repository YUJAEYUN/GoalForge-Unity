using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TeamSelectController : MonoBehaviour
{
    [Header("Team Buttons")]
    public Button[] teamButtons;      // 국가 선택 버튼들

    [Header("Team Data")]
    public string[] teamNames;        // UI에 보여줄 팀 이름 (선택 텍스트용)
    public Sprite[] teamSprites;      // 각 팀의 선수 이미지 스프라이트

    [Header("VS Images")]
    public Image p1Image;             // VS 왼쪽 (1P)
    public Image p2Image;             // VS 오른쪽 (2P - 좌우 반전)

    [Header("Confirm")]
    public Button confirmButton;      // 항상 화면에 있는 Confirm 버튼

    private bool isSelectingP1 = true;    // true면 1P 차례, false면 2P 차례
    private int currentSelectedIndex = -1;   // 이번 차례에 고른 팀 인덱스

    private void Start()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.ResetGame();
        }

        if (p1Image != null) p1Image.gameObject.SetActive(false);
        if (p2Image != null) p2Image.gameObject.SetActive(false);

        if (p2Image != null)
        {
            Vector3 s = p2Image.rectTransform.localScale;
            p2Image.rectTransform.localScale = new Vector3(-Mathf.Abs(s.x), s.y, s.z);
        }

        for (int i = 0; i < teamButtons.Length; i++)
        {
            int index = i;
            if (teamButtons[i] != null)
            {
                teamButtons[i].onClick.AddListener(() => OnTeamButtonClicked(index));
            }
        }

        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirm);
        }
    }

    private void OnTeamButtonClicked(int index)
    {
        if (index < 0 || index >= teamSprites.Length)
        {
            Debug.LogWarning("[TeamSelect] 잘못된 팀 인덱스: " + index);
            return;
        }

        currentSelectedIndex = index;   // 🔥 인덱스 저장

        Sprite sprite = teamSprites[index];

        if (isSelectingP1)
        {
            if (p1Image == null) return;

            p1Image.sprite = sprite;
            p1Image.color = Color.white;
            p1Image.gameObject.SetActive(true);
        }
        else
        {
            if (p2Image == null) return;

            p2Image.sprite = sprite;
            p2Image.color = Color.white;
            p2Image.gameObject.SetActive(true);
        }
    }

    private void OnConfirm()
    {
        if (currentSelectedIndex < 0)
        {
            Debug.Log("[TeamSelect] 선택된 팀이 없습니다. 먼저 팀을 선택하세요.");
            return;
        }

        if (PlayerManager.Instance != null)
        {
            string teamName = "";
            if (teamNames != null &&
                currentSelectedIndex >= 0 &&
                currentSelectedIndex < teamNames.Length)
            {
                teamName = teamNames[currentSelectedIndex];
            }

            PlayerManager.Instance.SelectTeamIndex(currentSelectedIndex, teamName);
            Debug.Log($"[TeamSelect] {(isSelectingP1 ? "P1" : "P2")} 팀 확정: {teamName} (index {currentSelectedIndex})");
        }

        if (isSelectingP1)
        {
            isSelectingP1 = false;
            currentSelectedIndex = -1;
            Debug.Log("[TeamSelect] Player1 선택 확정. 이제 Player2 차례.");
        }
        else
        {
            Debug.Log("[TeamSelect] Player2 선택 확정. CoinScene 으로 이동.");
            SceneManager.LoadScene("CoinScene");
        }
    }
}

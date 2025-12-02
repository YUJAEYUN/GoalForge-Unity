using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CursorHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Text uiText;        // 기본 UI Text
    private TMP_Text tmpText;   // TextMeshPro
    private Color normalColor;
    private Color hoverColor = Color.black;

    void Awake()
    {
        // 자식에서 Text / TMP_Text 자동 탐색
        uiText = GetComponentInChildren<Text>();
        tmpText = GetComponentInChildren<TMP_Text>();

        if (uiText != null)
        {
            normalColor = uiText.color;
        }
        else if (tmpText != null)
        {
            normalColor = tmpText.color;
        }
        else
        {
            Debug.LogWarning($"[CursorHoverUI] {name} 에서 Text나 TMP_Text를 찾지 못했습니다.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 커서 매니저에 hover 알리기 (있을 때만)
        if (SoccerCursor.Instance != null)
            SoccerCursor.Instance.SetHover(true);

        // 텍스트 색 변경
        if (uiText != null)
            uiText.color = hoverColor;
        if (tmpText != null)
            tmpText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (SoccerCursor.Instance != null)
            SoccerCursor.Instance.SetHover(false);

        // 텍스트 색 원래대로
        if (uiText != null)
            uiText.color = normalColor;
        if (tmpText != null)
            tmpText.color = normalColor;
    }
}

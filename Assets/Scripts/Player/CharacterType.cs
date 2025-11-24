using UnityEngine;

public class CharacterType : MonoBehaviour
{
    public enum Type
    {
        Geodaino,  // 거다이노
        Lightningman // 번개맨
    }
    
    [Header("Character Settings")]
    [SerializeField] private Type characterType = Type.Geodaino;
    
    [Header("Visual Effects")]
    [SerializeField] private GameObject superModeVFX;
    [SerializeField] private Color superModeColor = Color.yellow;
    
    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;
    
    private Vector3 originalScale;
    private Color originalColor;
    
    private bool isSuperModeActive = false;
    
    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        originalScale = transform.localScale;
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }
    
    public void ActivateSuperMode()
    {
        if (isSuperModeActive) return;
        
        isSuperModeActive = true;
        
        switch (characterType)
        {
            case Type.Geodaino:
                ActivateGeodainoSuperMode();
                break;
            case Type.Lightningman:
                ActivateLightningmanSuperMode();
                break;
        }
        
        ApplyVisualEffects();
    }
    
    public void DeactivateSuperMode()
    {
        if (!isSuperModeActive) return;
        
        isSuperModeActive = false;
        
        transform.localScale = originalScale;
        
        if (playerController != null)
        {
            playerController.superModeSpeedMultiplier = 1f;
            playerController.superModeJumpMultiplier = 1f;
            playerController.isSuperMode = false;
        }
        
        RemoveVisualEffects();
    }
    
    void ActivateGeodainoSuperMode()
    {
        transform.localScale = originalScale * 3f;

        if (playerController != null)
        {
            playerController.superModeSpeedMultiplier = 0.5f;
            playerController.superModeJumpMultiplier = 1f;
            playerController.isSuperMode = true;
        }
    }

    void ActivateLightningmanSuperMode()
    {
        transform.localScale = originalScale * 0.8f;
        
        if (playerController != null)
        {
            playerController.superModeSpeedMultiplier = 3f;
            playerController.superModeJumpMultiplier = 2f;
            playerController.isSuperMode = true;
        }
        
        Debug.Log("Lightningman Super Mode: Speed x3, Jump x2, Size x0.8");
    }
    
    void ApplyVisualEffects()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = superModeColor;
        }
        
        if (superModeVFX != null)
        {
            superModeVFX.SetActive(true);
        }
    }
    
    void RemoveVisualEffects()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        
        if (superModeVFX != null)
        {
            superModeVFX.SetActive(false);
        }
    }
    
    public Type GetCharacterType() => characterType;
    public bool IsSuperModeActive() => isSuperModeActive;
}


using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerStateManager : MonoBehaviour
{
    public bool IsInvincible { get; private set; }
    public bool IsInvisible { get; private set; }

    private SpriteRenderer spriteRenderer;
    private int originalLayer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalLayer = gameObject.layer;
    }

    public void SetStatus(PlayerStatus status, bool enabled)
    {
        switch (status)
        {
            case PlayerStatus.Invincible:
                IsInvincible = enabled;
                break;

            case PlayerStatus.Invisible:
                IsInvisible = enabled;
                ApplyInvisibility(enabled);
                break;
        }
    }

    private void ApplyInvisibility(bool enable)
    {
        // 找到当前物体和所有子物体的 SpriteRenderer
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();

        float alpha = enable ? 0.5f : 1f;

        foreach (var renderer in renderers)
        {
            Color color = renderer.color;
            color.a = alpha;
            renderer.color = color;
        }

        gameObject.layer = enable ? LayerMask.NameToLayer("Invisible") : originalLayer;
    }

    public bool HasStatus(PlayerStatus status)
    {
        return status switch
        {
            PlayerStatus.Invincible => IsInvincible,
            PlayerStatus.Invisible => IsInvisible,
            _ => false,
        };
    }
}

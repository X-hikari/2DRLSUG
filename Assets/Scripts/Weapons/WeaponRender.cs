using UnityEngine;

/// <summary>
/// 控制武器的渲染效果（Sprite 或动画）
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class WeaponRenderer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 设置静态图片
    /// </summary>
    public void SetSprite(Sprite sprite)
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = sprite;
    }

    /// <summary>
    /// 播放 Animator 中的某个动画状态
    /// </summary>
    public void PlayAnimation(string animationName)
    {
        if (animator != null && animator.runtimeAnimatorController != null)
            animator.Play(animationName);
    }

    /// <summary>
    /// 设置动画控制器（从 WeaponData 动态载入）
    /// </summary>
    public void SetAnimatorController(RuntimeAnimatorController controller)
    {
        if (animator == null)
        {
            animator = gameObject.AddComponent<Animator>();
        }

        animator.runtimeAnimatorController = controller;
    }
}

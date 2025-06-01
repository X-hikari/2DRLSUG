using UnityEngine;
using System.Collections.Generic;

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
        {
            spriteRenderer.sprite = sprite;
        }

        if (animator != null)
        {
            animator.enabled = false; // 强制使用静态图时禁用动画
        }
    }

    /// <summary>
    /// 播放 Animator 中的某个动画状态
    /// </summary>
    public void PlayAnimation(string animationName)
    {
        if (animator != null && animator.runtimeAnimatorController != null && animator.enabled)
        {
            animator.SetTrigger(animationName);
        }
    }

    /// <summary>
    /// 设置动画控制器（从 WeaponData 动态载入）
    /// </summary>
    public void SetAnimatorOverride(RuntimeAnimatorController baseController, AnimationClip idleClip, AnimationClip attackClip)
    {
        if (animator == null)
        {
            animator = gameObject.AddComponent<Animator>();
        }

        if (baseController == null)
        {
            animator.runtimeAnimatorController = null;
            animator.enabled = false;
            return;
        }

        animator.enabled = true;

        var overrideController = new AnimatorOverrideController(baseController);
        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();

        foreach (var clip in overrideController.animationClips)
        {
            if (clip.name == "Idle" && idleClip != null)
                overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, idleClip));
            else if (clip.name == "Attack" && attackClip != null)
                overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, attackClip));
        }

        overrideController.ApplyOverrides(overrides);
        animator.runtimeAnimatorController = overrideController;
    }
}

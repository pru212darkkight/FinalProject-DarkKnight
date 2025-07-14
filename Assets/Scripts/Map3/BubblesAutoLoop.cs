using UnityEngine;

/// <summary>
/// Simple script to ensure bubbles animation loops automatically
/// </summary>
public class BubblesAutoLoop : MonoBehaviour
{
    private Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        
        if (animator != null)
        {
            // Force play the animation
            animator.Play("Bubbles", 0, 0f);
        }
    }
    
    void Update()
    {
        // Check if animation stopped and restart it
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            
            // If animation finished, restart it
            if (stateInfo.normalizedTime >= 1.0f && !animator.IsInTransition(0))
            {
                animator.Play("Bubbles", 0, 0f);
            }
        }
    }
}

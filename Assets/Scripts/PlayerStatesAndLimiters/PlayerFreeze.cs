using UnityEngine;

public class PlayerFreeze : MonoBehaviour
{
    [Tooltip("Skripty pohybu/otočky, které se mají vypnout (např. PlayerMovement, kamera...)")]
    public MonoBehaviour[] behavioursToDisable;

    int freezeCount = 0;

    PlayerMovement movement;
    Animator animator;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        if (movement != null)
            animator = movement.animator;
    }

    public void SetFrozen(bool frozen)
    {
        freezeCount += frozen ? 1 : -1;
        freezeCount = Mathf.Max(0, freezeCount);

        bool shouldFreeze = freezeCount > 0;

        foreach (var b in behavioursToDisable)
            if (b) b.enabled = !shouldFreeze;

        Cursor.visible   = shouldFreeze;
        Cursor.lockState = shouldFreeze ? CursorLockMode.None : CursorLockMode.Locked;

        if (shouldFreeze)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.StopWalking();

            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
                animator.SetBool("IsRunning", false);
            }
        }
    }
}

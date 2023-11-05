using UnityEngine;

public class HitBehaviour : StateMachineBehaviour
{
    private static readonly int Interact = Animator.StringToHash("Interact");
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<AnimationBehavior>().HitRestore?.Invoke();
        animator.ResetTrigger(Interact);
    }
}

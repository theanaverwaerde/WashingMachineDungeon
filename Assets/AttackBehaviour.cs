using UnityEngine;

public class AttackBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<AnimationBehavior>().AttackEnd?.Invoke();
    }
}

using UnityEngine;

public class EnemyAnimationHandler: MonoBehaviour {
    public Animator animator;

    public virtual void PlayMoveAnimation(Vector3 direction) { }
    public virtual void PlayAttackAnimation() { }
    public virtual void CleanseAnimation() { }
}

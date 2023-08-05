using UnityEngine;

public class GroundEnemyAnimation: EnemyAnimationHandler {
    public bool reachedEnd = true;

    public override void PlayMoveAnimation(Vector3 direction) {
        if (direction.x >= 0.8f) {
            animator.SetInteger("direction", 1);
        }
        else if (direction.x <= -0.8f) {
            animator.SetInteger("direction", 3);
        }
        else if (direction.y >= 0.8f) {
            animator.SetInteger("direction", 2);
        }
        else if (direction.y <= -0.8f) {
            animator.SetInteger("direction", 0);
        }
    }

    public override void PlayAttackAnimation() {
        if (reachedEnd) {
            // Stop other movement animation
            animator.SetInteger("direction", -1);
            reachedEnd = false;
        }

        animator.SetBool("attack", true);
    }

    public override void CleanseAnimation() {
        reachedEnd = true;
    }
}

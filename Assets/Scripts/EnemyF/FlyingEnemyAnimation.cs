using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyAnimation : EnemyAnimationHandler {
    public override void CleanseAnimation() {
        base.CleanseAnimation();
    }

    public override void PlayAttackAnimation() {
        base.PlayAttackAnimation();
    }

    public override void PlayMoveAnimation(Vector3 direction) {
        base.PlayMoveAnimation(direction);
    }
}

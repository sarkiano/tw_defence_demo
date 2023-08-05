using UnityEngine;

public class Enemy : MonoBehaviour {
  public int Id { get; set; }
  public float Health { get; set; }
  public float Speed { get; set; }
  public float Damage { get; set; }
  public float Gold { get; set; }
  public string EnemyType { get; set; }
  public int TargetIndex { get; set; }

    public EnemyAnimationHandler enemyAnimator;

    public Transform target;
    private Vector3 nextFlagDirection;
    private bool reachedEnd = false;

    public void FixedUpdate() {
        if (Health <= 0) {
            Kill();
            return;
        }
        nextFlagDirection = target.position - transform.position;

        if (!reachedEnd) {
            Move();
        }
        else {
            DoDamage();
        }
    }

    public void CleanseEnemy() {
        GM.createdEnemies++;
        reachedEnd = false;
        target = GM.flags[TargetIndex].transform;
        nextFlagDirection = target.position - transform.position;
        enemyAnimator.CleanseAnimation();
        InvokeRepeating(nameof(FindNextTarget), .0f, .1f);
    }

    public void Move() {
        transform.Translate(Speed * Time.fixedDeltaTime * nextFlagDirection.normalized, Space.World);
        enemyAnimator.PlayMoveAnimation(nextFlagDirection);
    }

    public void Kill() {
        //Destroy(gameObject);
        CancelInvoke(nameof(FindNextTarget));
        GM.waveEnemiesKilled += 1;
        GM.totalEnemiesKilled += 1;
        GM.money += Gold;
        gameObject.SetActive(false);
    }


    public void FindNextTarget() {
        if (nextFlagDirection.magnitude <= Random.Range(0.3f, 0.7f)) {
            TargetIndex++;

            try {
                target = GM.flags[TargetIndex].transform;
            }
            catch {
                // Enemy reached the end of road
                CancelInvoke(nameof(FindNextTarget));
                reachedEnd = true;
            }
        }
    }

    public void DoDamage() {
        enemyAnimator.PlayAttackAnimation();
        GM.castleHealth -= Damage;
    }
}

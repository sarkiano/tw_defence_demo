using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float Speed { get; set; }
    public float Health { get; set; }
    public float Damage { get; set; }
    public float Gold { get; set; }
    public int Id { get; set; }
    public int TargetIndex { get; set; }

    [SerializeField] private Animator animator;

    private Transform target;
    private Vector3 nextFlagDirection;
    private bool reachedEnd = false;

    private void Start()
    {
        GM.createdEnemies.Add(gameObject);
        target = GM.flags[TargetIndex].transform;
        InvokeRepeating(nameof(FindNextTarget), .5f, .1f);
    }

    private void Update()
    {
        nextFlagDirection = target.position - transform.position;
    }

    private void FixedUpdate()
    {
        if (!reachedEnd)
        {
            transform.Translate(Speed * Time.fixedDeltaTime * nextFlagDirection.normalized, Space.World);
            WalkAnimation(nextFlagDirection);
        }
        else
        {
            DoDamage();
        }
    }

    private void LateUpdate()
    {
        if (Health <= 0)
        {
            Destroy(gameObject);
            GM.waveEnemiesKilled += 1;
            GM.totalEnemiesKilled += 1;
            GM.money += Gold;
        }
    }
    private void FindNextTarget()
    {
        if (nextFlagDirection.magnitude <= Random.Range(0.3f, 0.7f))
        {
            TargetIndex++;

            try
            {
                target = GM.flags[TargetIndex].transform;
            }
            catch
            {
                // Enemy reached the end of road
                CancelInvoke();
                reachedEnd = true;

                // Stop other movement animation
                animator.SetInteger("direction", -1);
            }
        }
    }

    private void WalkAnimation(Vector3 direction)
    {
        if (direction.x >= 0.8f)
        {
            animator.SetInteger("direction", 1);
        }
        else if (direction.x <= -0.8f)
        {
            animator.SetInteger("direction", 3);
        }
        else if (direction.y >= 0.8f)
        {
            animator.SetInteger("direction", 2);
        }
        else if (direction.y <= -0.8f)
        {
            animator.SetInteger("direction", 0);
        }
    }

    private void DoDamage()
    {
        animator.SetBool("attack", true);
        GM.castleHealth -= Damage;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public float fireRate;
    public float fireRange;
    public float projectileSpeed;
    public float damage;
    public float upgradeCost;
    public bool canUpgrade;

    [SerializeField] private Projectile bullet;
    [SerializeField] private Transform firePoint;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite upgradeSprite;

    private Transform target;
    private Vector3 dirToEnemy;
    private bool allowFire;

    private void Start()
    {
        target = null;
        firePoint = transform.GetChild(0);
        allowFire = true;
        InvokeRepeating(nameof(GetEnemies), .0f, 0.05f);
    }

    private void Update()
    {
        if (target)
        {
            dirToEnemy = (transform.position - target.position).normalized;
        }
    }

    private void FixedUpdate()
    {
        if (target && (target.position - transform.position).magnitude < fireRange)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dirToEnemy.y, dirToEnemy.x) * Mathf.Rad2Deg + 90f);
            if (allowFire)
            {
                StartCoroutine(Shoot());
            }
        }
    }

    private void GetEnemies()
    {
        if (target)
        {
            if ((target.position - transform.position).magnitude >= fireRange)
                target = null;
            return;
        }

        List<Collider2D> collidersInArea = new List<Collider2D>();
        int enemiesCount = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y), fireRange, new ContactFilter2D().NoFilter(), collidersInArea);
        
        if (enemiesCount > 0)
        {
            // TODO implement hit last, first, strongest, weakest enemy strategy
            foreach(Collider2D collider in collidersInArea)
            {
                if (collider.CompareTag("Enemy"))
                target = collider.transform;
                //Debug.Log("ID: " + collider.GetComponent<Enemy>().Id);
                break;
            }
        }
        else
        {
            target = null;
        }
    }

    IEnumerator Shoot()
    {
        if (!target.gameObject.activeInHierarchy)
        {
            target = null;
        }
        else
        {
            allowFire = false;
            var firedBullet = Instantiate(bullet, firePoint.position, firePoint.rotation);
            firedBullet.TargetDir = -dirToEnemy;
            firedBullet.Speed = projectileSpeed;
            firedBullet.Damage = damage;
            yield return new WaitForSeconds(fireRate);
            allowFire = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, fireRange);
    }

    private void OnMouseDown()
    {
        if (canUpgrade)
        {
            UIScript.Instance.UpgradeMenu(gameObject);
        }
    }

    public void UpgradeTower()
    {
        if (GM.money >= upgradeCost)
        {
            spriteRenderer.sprite = upgradeSprite;
            fireRange = 7f;
            fireRate = 0.6f;
            damage = 10f;
            GM.money -= upgradeCost;
            canUpgrade = false;
        }
    }
}

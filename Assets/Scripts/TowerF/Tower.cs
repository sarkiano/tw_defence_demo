using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {
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
    private float cooldown;

    private void Start() {
        target = GetEnemies();
        firePoint = transform.GetChild(0);
        cooldown = fireRate;
    }

    private void FixedUpdate() {
        if (target) {
            if (target.gameObject.activeInHierarchy & (target.position - transform.position).magnitude < fireRange) {
                dirToEnemy = (transform.position - target.position).normalized;
                transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dirToEnemy.y, dirToEnemy.x) * Mathf.Rad2Deg + 90f);

                if (cooldown > fireRate) {
                    Shoot();
                }
                else {
                    cooldown += Time.deltaTime;
                }
            }
            else {
                target = null;
            }
        }
        else {
            target = GetEnemies();
        }
    }

    private Transform GetEnemies() {
        List<Collider2D> collidersInArea = new List<Collider2D>();
        int enemiesCount = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y), fireRange, new ContactFilter2D().NoFilter(), collidersInArea);
        
        if (enemiesCount > 0) {
            // TODO implement hit last, first, strongest, weakest enemy strategy
            foreach(Collider2D collider in collidersInArea) {
                if (collider.CompareTag("Enemy")) {
                    return collider.transform;
                }
            }
        }
        return null;
    }

    private void Shoot() {
        var firedBullet = PoolObjects.Instance.GetPooledObject(ProjectileTypesEnum.SMALLBULLET);
        firedBullet.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
        //Debug.Log($"firedBullet {firedBullet}");
        Projectile firedBulletScript = firedBullet.GetComponent<Projectile>();
        firedBulletScript.TargetDir = -dirToEnemy;
        firedBulletScript.Speed = projectileSpeed;
        firedBulletScript.Damage = damage;
        firedBullet.SetActive(true);
        cooldown = 0;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, fireRange);
    }

    private void OnMouseDown() {
        if (canUpgrade) {
            UIScript.Instance.UpgradeMenu(gameObject);
        }
    }

    public void UpgradeTower() {
        if (GM.money >= upgradeCost) {
            spriteRenderer.sprite = upgradeSprite;
            fireRange = 7f;
            fireRate = 0.6f;
            damage = 10f;
            GM.money -= upgradeCost;
            canUpgrade = false;
        }
    }
}

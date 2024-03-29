using UnityEngine;

public class Projectile : MonoBehaviour {
    public float Speed { get; set; }
    public float Damage { get; set; }
    public Vector3 TargetDir { get; set; }

    [SerializeField] private Rigidbody2D rb;

    private void FixedUpdate() {
        rb.AddForce(Speed * Time.deltaTime * TargetDir, ForceMode2D.Force);
        
        // Projectiles deactivated after some time of existence
        if (transform.position.magnitude >= 100f) {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Enemy")) {
            collision.gameObject.GetComponent<Enemy>().Health -= Damage;
            gameObject.SetActive(false);
        }
    }
}

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    public float speed = 12f;
    public int damage = 25;
    public float lifeTime = 3f;
    private Rigidbody2D rb;
    private float timer;
    private string poolKey = "bullet";

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
            Despawn();
    }

    public void OnSpawn()
    {
        rb.velocity = transform.right * speed;
        timer = lifeTime;
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            var health = collision.GetComponent<Health>();
            if (health != null)
                health.TakeDamage(damage);
        }
        // Возврат пули в Пул
        Despawn();
    }

    private void Despawn()
    {
        rb.velocity = Vector2.zero;
        if (ObjectPool.Instance != null)
            ObjectPool.Instance.Release(poolKey, gameObject);
        else
            Destroy(gameObject);
    }
}
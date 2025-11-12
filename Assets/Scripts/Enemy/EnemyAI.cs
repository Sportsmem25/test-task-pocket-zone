using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed;
    public float detectRadius;
    public float attackRadius;
    public float attackCooldown = 2f;
    [Range(0f, 1f)] public float dropChance1 = 0.5f;
    public int attackDamage = 10;
    public GameObject dropPrefab;
    public GameObject dropPrefab2;
    private Transform target;
    private Health health;
    private float attackTimer;
    private bool isFacingRight = true;

    private void Start()
    {
        health = GetComponent<Health>();
        health.onDie.AddListener(OnDie);
    }

    private void Update()
    {
        if (target == null)
        {
            var p = GameObject.FindWithTag("Player"); 
            if (p != null)
                target = p.transform;
        }
        if (target == null) return;
        attackTimer -= Time.deltaTime;
        Vector2 targetPos = GetTargetCenter(target);
        float dist = Vector2.Distance(transform.position, target.position);
        if (dist <= detectRadius)
        {
            Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
            transform.position += (Vector3)dir * speed * Time.deltaTime;
            Flip(dir.x);
            if (dist <= attackRadius && attackTimer <= 0)
            {
                var pHealth = target.GetComponent<Health>();
                if (pHealth != null) pHealth.TakeDamage(attackDamage);
                attackTimer = attackCooldown;
            }
        }
    }

    private Vector2 GetTargetCenter(Transform t)
    {
        var col = t.GetComponent<Collider2D>();
        if (col != null)
            return col.bounds.center;

        return t.position;
    }

    private void Flip(float moveX)
    {
        if (moveX > 0 && !isFacingRight)
            SetFacing(true);
        else if (moveX < 0 && isFacingRight)
            SetFacing(false);
    }

    private void SetFacing(bool right)
    {
        isFacingRight = right;
        transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);
    }

    private void OnDie()
    {
        DropLoot();
        Destroy(gameObject);
    }

    private void DropLoot()
    {
        GameObject drop = null;

        if (Random.value <= dropChance1 && dropPrefab != null)
            drop = dropPrefab;
        else if (dropPrefab2 != null)
            drop = dropPrefab2;

        if (drop != null)
            Instantiate(drop, transform.position, Quaternion.identity);
    }
}
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float detectRadius;
    [SerializeField] private float attackRadius;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private GameObject dropPrefab;
    [SerializeField] private GameObject dropPrefab2;

    private Transform target;
    private Health health;
    private float attackTimer;
    private float dropChance1 = 0.5f;
    private bool isFacingRight = true;

    private void Start()
    {
        health = GetComponent<Health>();
        health.OnDie.AddListener(OnDie);
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
                Health pHealth = target.GetComponent<Health>();
                if (pHealth != null) pHealth.TakeDamage(attackDamage);
                attackTimer = attackCooldown;
            }
        }
    }

    private Vector2 GetTargetCenter(Transform t)
    {
        Collider2D col = t.GetComponent<Collider2D>();
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
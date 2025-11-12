using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public UnityEvent onDie;
    public Slider sliderHP;
    public GameObject canvasHP;

    private void Awake()
    {
        currentHealth = maxHealth;
        sliderHP.value = currentHealth;
    }

    private void Update()
    {
        if (canvasHP.transform.rotation != Camera.main.transform.rotation)
            canvasHP.transform.rotation = Camera.main.transform.rotation;

        var ls = canvasHP.transform.localScale;
        canvasHP.transform.localScale = new Vector3(Mathf.Abs(ls.x), Mathf.Abs(ls.y), Mathf.Abs(ls.z));
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;
        currentHealth -= damage;
        sliderHP.value = currentHealth;
        if (currentHealth <= 0) 
        { 
            currentHealth = 0; 
            Dead();
            if (CompareTag("Player"))
            {
                var goUI = FindObjectOfType<GameOverUI>();
                if (goUI != null) goUI.ShowGameOver();
            }
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        currentHealth += amount;
        if(currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public bool IsDead() => currentHealth <= 0;

    private void Dead()
    {
        onDie?.Invoke();
    }
}
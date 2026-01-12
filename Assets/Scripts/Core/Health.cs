using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int MaxHealth = 100;
    public int CurrentHealth;
    public UnityEvent OnDie;
    public Slider SliderHP;

    [SerializeField] private GameObject canvasHP;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
        SliderHP.value = CurrentHealth;
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
        CurrentHealth -= damage;
        SliderHP.value = CurrentHealth;
        if (CurrentHealth <= 0) 
        { 
            CurrentHealth = 0; 
            Dead();
            if (gameObject.layer == 3)
            {
                GameOverUI goUI = FindObjectOfType<GameOverUI>();
                if (goUI != null) goUI.ShowGameOver();
            }
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        CurrentHealth += amount;
        if(CurrentHealth > MaxHealth)
            CurrentHealth = MaxHealth;
    }

    public bool IsDead() => CurrentHealth <= 0;

    private void Dead()
    {
        OnDie?.Invoke();
    }
}
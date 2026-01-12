using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform FirePoint;
    public int CurrentAmmo = 10;
    public int MaxAmmo = 30;

    private bool isReloading = false;
    private string bulletPoolKey = "bullet";
    private float fireRate = 0.3f;
    private float reloadTime = 2f;
    private float _cooldown;

    private void Start()
    {
        CurrentAmmo = MaxAmmo;
    }

    private void Update()
    {
        if (isReloading) return;
        _cooldown -= Time.deltaTime;
        if(CurrentAmmo <= 0 && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    public void TryFire()
    {
        if (isReloading) return;
        if (_cooldown > 0) return;

        CurrentAmmo--;
        _cooldown = fireRate;
        if (ObjectPool.Instance == null)
        {
            Debug.LogWarning("No ObjectPool in scene"); 
            return;
        }
        var bullet = ObjectPool.Instance.Get(bulletPoolKey);
        if (bullet == null) return;
        bullet.transform.position = FirePoint.position;
        bullet.transform.rotation = FirePoint.rotation;
        var b = bullet.GetComponent<Bullet>();
        if (b != null) 
            b.OnSpawn();
    }

    public void ReturnBullet(GameObject bullet)
    {
        if (ObjectPool.Instance != null)
            ObjectPool.Instance.Release(bulletPoolKey, bullet);
        else Destroy(bullet);
    }

    public void AddAmmo(int amount)
    {
        CurrentAmmo = Mathf.Min(MaxAmmo, CurrentAmmo + amount);
    }

    public void OnEnable()
    {
        if (PlayerInput.Instance != null)
            PlayerInput.Instance.OnFirePressed += TryFire;
    }

    public void OnDisable()
    {
        if (PlayerInput.Instance != null)
            PlayerInput.Instance.OnFirePressed -= TryFire;
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log($"Reloading... ({reloadTime}s)");
        yield return new WaitForSeconds(reloadTime);

        CurrentAmmo = MaxAmmo;
        isReloading = false;
        Debug.Log("Reload complete");
    }
}
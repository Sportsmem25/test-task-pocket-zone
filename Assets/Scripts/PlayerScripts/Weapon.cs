using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform firePoint;
    public string bulletPoolKey = "bullet";
    public bool isReloading = false;
    public int currentAmmo = 10;
    public int maxAmmo = 30;
    public float fireRate = 0.3f;
    public float reloadTime = 2f;
    private float _cooldown;

    private void Start()
    {
        currentAmmo = maxAmmo;
    }

    private void Update()
    {
        if (isReloading) return;
        _cooldown -= Time.deltaTime;
        if(currentAmmo <= 0 && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    public void TryFire()
    {
        if (isReloading) return;
        if (_cooldown > 0) return;

        currentAmmo--;
        _cooldown = fireRate;
        if (ObjectPool.Instance == null)
        {
            Debug.LogWarning("No ObjectPool in scene"); 
            return;
        }
        var bullet = ObjectPool.Instance.Get(bulletPoolKey);
        if (bullet == null) return;
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
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
        currentAmmo = Mathf.Min(maxAmmo, currentAmmo + amount);
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

        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log("Reload complete");
    }
}
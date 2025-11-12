using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private Text ammoText;

    private void Update()
    {
        if (weapon == null || ammoText == null) return;
        ammoText.text = $"{weapon.currentAmmo}/{weapon.maxAmmo}";
    }
}
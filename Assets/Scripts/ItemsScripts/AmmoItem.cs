using UnityEngine;

[CreateAssetMenu(menuName = "Items/Ammo")]
public class AmmoItem : Item
{
    private int ammoAmount = 10;

    public override void Use(GameObject user)
    {
        if (user == null) return;
        Weapon weapon = user.GetComponentInChildren<Weapon>();
        if (weapon != null) weapon.AddAmmo(ammoAmount);
    }
}
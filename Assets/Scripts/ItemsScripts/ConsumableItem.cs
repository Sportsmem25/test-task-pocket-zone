using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumable")]
public class ConsumableItem : Item
{
    private int healAmount = 20;

    public override void Use(GameObject user)
    {
        Health h = user.GetComponent<Health>();
        if (h != null)
        {
            h.Heal(healAmount);
        }
    }
}
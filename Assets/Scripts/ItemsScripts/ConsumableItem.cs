using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumable")]
public class ConsumableItem : Item
{
    public int healAmount = 20;

    public override void Use(GameObject user)
    {
        var h = user.GetComponent<Health>();
        if (h != null)
        {
            h.Heal(healAmount);
        }
    }
}

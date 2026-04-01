using UnityEngine;

public class HealthPackItem : MonoBehaviour, IItem
{
    public int amount = 40;

    public void Use(GameObject target)
    {
        var livingEntity = target.GetComponent<LivingEntity>();
        if (livingEntity != null)
        {
            livingEntity.Heal(amount);
        }
    }
}

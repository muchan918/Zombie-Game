using UnityEngine;

public class AmmoPackItem : MonoBehaviour, IItem
{
    public int amount = 100;

    public void Use(GameObject target)
    {
        var shooter = target.GetComponent<PlayerShooter>();
        if (shooter != null)
        {
            shooter?.gun?.AddRemainAmmo(amount);
        }
    }
}

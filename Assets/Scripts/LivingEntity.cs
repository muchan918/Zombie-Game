using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class LivingEntity : MonoBehaviour, IDamagable
{
    public float startingHealth = 100f;
    public float Health { get; private set; }
    public bool IsDead { get; private set; }

    public UnityEvent OnDead;

    // 초기화 하는 것
    protected virtual void OnEnable()
    {
        IsDead = false;
        Health = startingHealth;

    }

    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Health = 0;
            Die();
        }
    }

    public virtual void Die()
    {
        IsDead = true;
        OnDead?.Invoke();
    }

    public virtual void Heal(int add)
    {
        Health = Mathf.Min(startingHealth, Health + add);
    }
}

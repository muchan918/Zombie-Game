using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : LivingEntity
{
    public Image uiHealth;

    public AudioClip deathClip;
    public AudioClip hitClip;

    private AudioSource playerAudioSource;
    private Animator playerAnimator;

    // 게임 오브젝트를 없애는게 아니고 컴포넌트를 없애야한다
    private PlayerMovement playerMovement;
    private PlayerShooter playerShooter;

    private void Awake()
    {
        playerAudioSource = GetComponent<AudioSource>();
        playerAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooter = GetComponent<PlayerShooter>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        uiHealth.gameObject.SetActive(true);
        uiHealth.fillAmount = 1f;

        playerMovement.enabled = true;
        playerShooter.enabled = true;
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!IsDead)
        {
            playerAudioSource.PlayOneShot(hitClip);
        }

        base.OnDamage(damage, hitPoint, hitNormal);

        uiHealth.fillAmount = Health / startingHealth;
    }

    public override void Die()
    {
        if (IsDead)
        {
            return;
        }

        base.Die();

        uiHealth.gameObject.SetActive(false);
        playerAudioSource.PlayOneShot(deathClip);
        playerAnimator.SetTrigger("Die");

        playerMovement.enabled = false;
        playerShooter.enabled = false;
    }

    public override void Heal(int add)
    {
        base.Heal(add);
        uiHealth.fillAmount = Health / startingHealth;
    }
}

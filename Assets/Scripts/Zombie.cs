using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : LivingEntity
{
    public enum Status
    {
        Idle,
        Trace,
        Attack,
        Die,
    }

    public Transform target;
    public HitBox hitBox;
    public LayerMask targetLayer;

    private NavMeshAgent agent;
    private Animator zombieAnimator;

    public float traceDistance = 10f;
    public float attackDistance = 1f;

    public float attackInterval = 1f;
    private float lastAttackTime = 0f;

    private float damage;

    private Status currentStatus;

    public AudioClip deathClip;
    public AudioClip hitClip;

    private AudioSource zombieAudioSource;
    public ParticleSystem bloodEffect;
    public Collider zombieCollider;

    public Renderer zombieRenderer;

    public Status CurrentStatus
    {
        get { return currentStatus; }
        set
        {
            var prevStatus = currentStatus;
            currentStatus = value;
            Debug.Log(currentStatus);

            // 상태가 변경될 때 초기화 해야하는 자리
            switch (currentStatus)
            {
                case Status.Idle:
                    zombieAnimator.SetBool("HasTarget", false);
                    agent.isStopped = true;
                    break;
                case Status.Trace:
                    zombieAnimator.SetBool("HasTarget", true);
                    agent.isStopped = false;
                    break;
                case Status.Attack:
                    zombieAnimator.SetBool("HasTarget", false);
                    agent.isStopped = true;
                    break;
                case Status.Die:
                    zombieAnimator.SetTrigger("Die");
                    agent.isStopped = true;
                    zombieCollider.enabled = false;
                    hitBox.Colliders.Clear();
                    hitBox.gameObject.SetActive(false);
                    zombieAudioSource.PlayOneShot(deathClip);
                    break;
            }
        }
    }

    public void Setup(ZombieData data)
    {
        gameObject.SetActive(false);

        startingHealth = data.maxHP;
        damage = data.damage;
        agent.speed = data.speed;
        zombieRenderer.material.color = data.skinColor;

        gameObject.SetActive(true);
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        zombieAnimator = GetComponent<Animator>();
        zombieAudioSource = GetComponent<AudioSource>();
        zombieCollider = GetComponent<Collider>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        // NavMesh를 사용할거면 이걸 추가해야한다
        agent.enabled = true;
        agent.isStopped = false;
        agent.ResetPath();
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }

        zombieCollider.enabled = true;
        hitBox.gameObject.SetActive(true);

        CurrentStatus = Status.Idle;
    }

    private void Update()
    {
        // 상태가 업데이트 될때마다 실행해야될 함수 -> 메서드로 만들어서 실행
        switch (currentStatus)
        {
            case Status.Idle:
                UpdateIdle();
                break;
            case Status.Trace:
                UpdateTrace();
                break;
            case Status.Attack:
                UpdateAttack();
                break;
            case Status.Die:
                UpdateDie();
                break;
        }
    }

    // 뒷부분 조건 검사 안하기 위해 상태 바꾸면 바로 return
    private void UpdateIdle()
    {
        if (target != null &&
            Vector3.Distance(target.position, transform.position) < traceDistance)
        {
            CurrentStatus = Status.Trace;
            return;
        }

        target = FindTarget(traceDistance);
    }

    private void UpdateTrace()
    {
        if (target == null || Vector3.Distance(target.position, transform.position) > traceDistance)
        {
            target = null;
            CurrentStatus = Status.Idle;
            return;
        }

        var lookAt = target.position;
        lookAt.y = transform.position.y;
        transform.LookAt(lookAt);

        // if (target != null &&
        //     Vector3.Distance(target.position, transform.position) < attackDistance)
        // {
        //     CurrentStatus = Status.Attack;
        //     return;
        // }

        if (target != null)
        {
            var find = hitBox.Colliders.Find(x => x.transform == target);
            if (find != null)
            {
                CurrentStatus = Status.Attack;
                return;
            }
        }

        // target이 자꾸 움직이기 때문에 Update에서 호출해야함
        agent.SetDestination(target.position);
    }

    private void UpdateAttack()
    {
        // if (target == null || Vector3.Distance(target.position, transform.position) > attackDistance)
        // {
        //     target = null;
        //     CurrentStatus = Status.Trace;
        //     return;
        // }

        if (target == null)
        {
            CurrentStatus = Status.Trace;
            return;
        }
        var find = hitBox.Colliders.Find(x => x.transform == target);
        if (find == null)
        {
            CurrentStatus = Status.Trace;
            return;
        }

        var lookAt = target.position;
        lookAt.y = transform.position.y;
        transform.LookAt(lookAt);

        if (Time.time > lastAttackTime + attackInterval)
        {
            lastAttackTime = Time.time;
            var livingEntity = target.GetComponent<LivingEntity>();
            if (livingEntity != null)
            {
                if (!livingEntity.IsDead)
                {
                    Debug.Log("Attack Attack");
                    livingEntity.OnDamage(10f, transform.position, -transform.forward);
                }
            }
        }

        // target이 자꾸 움직이기 때문에 Update에서 호출해야함
        agent.SetDestination(target.position);
    }

    private void UpdateDie()
    {

    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);

        zombieAudioSource.PlayOneShot(hitClip);
        bloodEffect.transform.position = hitPoint;
        bloodEffect.transform.forward = hitNormal;
        bloodEffect.Play();
    }

    public override void Die()
    {
        CurrentStatus = Status.Die;
        base.Die();
    }

    private Transform FindTarget(float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, targetLayer);

        if (colliders.Length == 0)
        {
            return null;
        }

        var target = colliders.OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).First();
        return target.transform;
    }
}

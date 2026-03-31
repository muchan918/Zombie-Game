using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
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

    private Status currentStatus;

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
                    break;
            }
        }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        zombieAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        CurrentStatus = Status.Idle;
    }

    private void Update()
    {
        //agent.SetDestination(target.position);

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
            Debug.Log("Attack Attack");
            lastAttackTime = Time.time;
            var damagable = target.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.OnDamage(10f, transform.position, -transform.forward);
            }
        }

        // target이 자꾸 움직이기 때문에 Update에서 호출해야함
        agent.SetDestination(target.position);
    }

    private void UpdateDie()
    {
        throw new NotImplementedException();
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

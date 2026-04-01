using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Gun : MonoBehaviour
{
    // 상태 머신은 열거형으로 정의하기
    public enum Stats
    {
        Ready,
        Empty,
        Reloading
    }
    public UIManager uiManager;

    public Stats State { get; private set; }

    public Transform fireTransform;

    // 얘네들은 자식 오브젝트에 있는거라 public
    public ParticleSystem muzzleEffect;
    public ParticleSystem shellEffect;

    public LayerMask targetLayer;

    // 얘네들은 컴포넌트로 가져올거라 private
    private LineRenderer bulletLineEffect;
    private AudioSource gunAudioPlayer;

    // 스크립팅 오브젝트에서 가져올 데이터
    public GunData gunData;

    // 사정거리는 아니고 한 Level에서 끝 점을 계산하는 용도
    private float fireDistance = 50f;

    public int ammoRemain = 100;
    public int magAmmo;

    // 시간 간격을 줘서 한발씩 나가도록 구현
    private float lastFireTime;

    private Coroutine coShot;
    private Coroutine coReload;

    private void Awake()
    {
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineEffect = GetComponent<LineRenderer>();

        bulletLineEffect.positionCount = 2;
        bulletLineEffect.enabled = false;
    }

    private void OnEnable()
    {
        ammoRemain = gunData.startAmmoRemain;
        magAmmo = gunData.magCapacity;

        State = Stats.Ready;
        lastFireTime = 0f;
        uiManager.SetAmmoText(magAmmo, ammoRemain);
    }

    // 바깥에서 호출할 함수들

    // 총 상태 확인하는 함수 + 발사할 수 있는 시간이 됐는지 확인 => 이후 Shot  호출
    public void Fire()
    {
        if (State == Stats.Ready &&
            Time.time > lastFireTime + gunData.timeBetFire) // 발사할 수 있는 시간
        {
            lastFireTime = Time.time;

            Shot();
        }
    }

    // 총을 쏘고 데미지 연출하는 함수 -> 코루틴으로 작성
    // 연출 + 데미지 주는 것 + 총알 빼주는 것
    // 쏠 때마다 끝점이 달라진다 -> 레이캐스팅으로 가져올 것
    private void Shot()
    {
        // 초기화
        Vector3 hitPosition = Vector3.zero;

        // ray의 시작점과 방향으로 생성
        Ray ray = new Ray(fireTransform.position, fireTransform.forward);

        // 충돌했을때 충돌체의 정보를 갖고 있는 구조체
        // 충돌하면 fireDistance까지만 검사
        if (Physics.Raycast(ray, out RaycastHit hit, fireDistance, targetLayer))
        {
            // ray가 충돌한 world 좌표
            hitPosition = hit.point;

            var target = hit.collider.GetComponent<LivingEntity>();
            if (target != null)
            {
                // ray로 충돌하는 충돌체의 법선은 충돌체의 모양에 따라 달라진다.
                target.OnDamage(gunData.damage, hit.point, hit.normal);
            }
        }
        else // 안맞았을때
        {
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;
        }

        if (coShot != null)
        {
            StopCoroutine(coShot);
            coShot = null;
        }

        // 항상 Start코루틴으로 코루틴을 호출해야함
        // 게임 오브젝트 단위로 동작한다.
        coShot = StartCoroutine(CoShotEffect(hitPosition));

        // 총알 빼주기
        magAmmo--;
        uiManager.SetAmmoText(magAmmo, ammoRemain);
        if (magAmmo <= 0)
        {
            State = Stats.Empty;
        }
    }

    // using System.Collections; 적어야함
    // 함수명 앞에 Co 적는게 국룰
    // 코루틴에 몇초 있다가 돌아오게 하는 함수가 있다 
    private IEnumerator CoShotEffect(Vector3 hitPosition)
    {
        // shoot 효과 
        muzzleEffect.Play();
        shellEffect.Play();

        gunAudioPlayer.PlayOneShot(gunData.shotClip);

        bulletLineEffect.SetPosition(0, fireTransform.position);
        bulletLineEffect.SetPosition(1, hitPosition);
        bulletLineEffect.enabled = true;

        // 0.03초 이후에 아래 줄이 실행된다
        yield return new WaitForSeconds(0.03f);

        bulletLineEffect.enabled = false;
        coShot = null;

        // shoot 끝난 뒤 효과
    }

    public bool Reload()
    {
        if (coReload != null)
        {
            return false;
        }

        if (State != Stats.Reloading && magAmmo != gunData.magCapacity && ammoRemain != 0)
        {
            coReload = StartCoroutine(CoReload());
            return true;
        }

        return false;
    }

    private IEnumerator CoReload()
    {
        int temp = Math.Min(gunData.magCapacity - magAmmo, ammoRemain);
        State = Stats.Reloading;

        yield return new WaitForSeconds(gunData.reloadTime);

        coReload = null;
        magAmmo += temp;
        ammoRemain -= temp;
        uiManager.SetAmmoText(magAmmo, ammoRemain);
        State = Stats.Ready;
    }

    public void AddRemainAmmo(int add)
    {
        ammoRemain += add;
        uiManager.SetAmmoText(magAmmo, ammoRemain);
    }
}

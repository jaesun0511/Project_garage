using System.Collections;
using UnityEngine;

/// <summary>
/// 즉발 공격 모듈입니다.
/// - 같은 GameObject에 붙은 `Character`의 `teamId`를 기준으로 주변의 적을 탐지합니다.
/// - `attackInterval` 주기마다 `Physics.OverlapSphere`로 반경 내 콜라이더를 수집하고,
///   가장 가까운 적 `Character`에게 즉시 `TakeDamage`를 호출합니다.
/// - 씬 뷰에서 사거리를 확인할 수 있도록 `OnDrawGizmosSelected`를 구현합니다.
/// </summary>
[DisallowMultipleComponent]
public class InstantCombatModule : MonoBehaviour
{
    [Header("Attack Settings")]
    [Tooltip("적에게 입힐 즉시 데미지 양")]
    public float damage = 10f;

    [Tooltip("공격 탐지 반경 (meters)")]
    public float attackRange = 3.0f;

    [Tooltip("공격 탐색 주기 (초)")]
    public float attackInterval = 1.0f;

    [Tooltip("탐색에 사용할 레이어 마스크. Character가 포함된 레이어를 설정하세요.")]
    public LayerMask characterLayer = ~0;

    // 이 모듈을 소유한 Character 컴포넌트
    private Character ownerCharacter;

    // 공격 루프 코루틴 참조
    private Coroutine attackCoroutine;

    void Awake()
    {
        ownerCharacter = GetComponent<Character>();
        if (ownerCharacter == null)
        {
            Debug.LogWarningFormat("InstantCombatModule on '{0}' requires a Character component on the same GameObject.", name);
        }
    }

    void OnEnable()
    {
        // 컴포넌트 활성화 시 코루틴 시작
        if (attackCoroutine == null)
            attackCoroutine = StartCoroutine(AttackLoop());
    }

    void OnDisable()
    {
        // 컴포넌트 비활성화 시 코루틴 정지
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    /// <summary>
    /// 주기적으로 반경 내 가장 가까운 적을 찾아 즉시 데미지를 가합니다.
    /// - OverlapSphere에 LayerMask를 적용하므로 불필요한 충돌 검사 비용을 줄일 수 있습니다.
    /// - 찾은 대상은 Character 컴포넌트의 teamId로 아군/적을 판별합니다.
    /// </summary>
    IEnumerator AttackLoop()
    {
        // 최소값 보장
        float interval = Mathf.Max(0.01f, attackInterval);
        var wait = new WaitForSeconds(interval);

        while (true)
        {
            yield return wait;

            if (ownerCharacter == null) continue;

            // LayerMask.value는 내부적으로 int로 변환됩니다.
            Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, characterLayer.value);

            Character nearestEnemy = null;
            float nearestSqrDist = float.MaxValue;

            foreach (var col in hits)
            {
                if (col == null) continue;

                // 콜라이더가 속한 오브젝트 또는 부모에서 Character를 검색합니다.
                Character other = col.GetComponentInParent<Character>();
                if (other == null) continue;

                // 자기 자신 또는 같은 팀이면 건너뜁니다.
                if (other == ownerCharacter) continue;
                if (other.teamId == ownerCharacter.teamId) continue;

                float sqrDist = (other.transform.position - transform.position).sqrMagnitude;
                if (sqrDist < nearestSqrDist)
                {
                    nearestSqrDist = sqrDist;
                    nearestEnemy = other;
                }
            }

            if (nearestEnemy != null)
            {
                // IDamageable 인터페이스를 통해 데미지 적용
                IDamageable damageable = nearestEnemy.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                }
                else
                {
                    // 안전장치: Character가 IDamageable을 구현하지 않은 경우 직접 호출
                    nearestEnemy.TakeDamage(damage);
                }
            }
        }
    }

    /// <summary>
    /// Scene 뷰에서 선택되었을 때 사거리 표시
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

using UnityEngine;

/// <summary>
/// Basic Character implementing IDamageable.
/// - maxHp: initial maximum HP
/// - currentHp: current HP (initialized to maxHp)
/// - teamId: team identifier used for ally/enemy checks
/// </summary>
[DisallowMultipleComponent]
public class Character : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public float maxHp = 100f;
    public float currentHp;

    [Header("Team")]
    public int teamId = 1;

    void Awake()
    {
        currentHp = maxHp;
    }

    void Start()
    {
        Debug.LogFormat("{0} initialized with HP {1}/{2} (Team {3})", name, currentHp, maxHp, teamId);
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;

        currentHp -= amount;
        Debug.LogFormat("{0} took {1} damage. Current HP: {2}/{3}", name, amount, currentHp, maxHp);

        if (currentHp <= 0f)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.LogFormat("{0} (Team {1}) died.", name, teamId);
        Destroy(gameObject);
    }
}

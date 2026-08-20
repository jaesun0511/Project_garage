using UnityEngine;

/// <summary>
/// Interface for objects that can receive damage.
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Apply immediate damage to this object.
    /// </summary>
    /// <param name="amount">Damage amount</param>
    void TakeDamage(float amount);
}

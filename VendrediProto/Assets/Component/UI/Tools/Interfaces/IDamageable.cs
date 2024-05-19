using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interace to link to objects which can be damageable
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Taking damage from somewhere
    /// </summary>
    void TakeDamage(int damage);

    /// <summary>
    /// Called when the object is dead
    /// </summary>
    void OnDeath();
}

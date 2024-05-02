using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] protected WeaponSO _weaponSO;
	[SerializeField] protected Rigidbody _rigidbody;

    public WeaponSO WeaponSO => _weaponSO;

	/// <summary>
	/// Fire the current weapon of the ship (need to extend this to more weapons/ammo etc)
	/// </summary>
	public void FireWeapon(Vector3 opponentPosition)
	{
		Vector3 direction = (opponentPosition - transform.position).normalized;
		_rigidbody.velocity = direction * _weaponSO.WeaponSpeed;
	}

	#region TRIGGER
	protected void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out IDamageable damageableObject))
		{
			//We damage the gameObject
			damageableObject.TakeDamage(_weaponSO.WeaponDamage);
			
		}

		Destroy(this);
	}

	#endregion TRIGGER
}

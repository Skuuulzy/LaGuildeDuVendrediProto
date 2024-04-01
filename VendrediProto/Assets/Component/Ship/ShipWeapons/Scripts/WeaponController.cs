using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] protected WeaponSO _weaponSO;
	[SerializeField] protected Rigidbody _rigidbody;

    public WeaponSO WeaponSO => _weaponSO;


	public void LaunchWeapon(bool isRight)
	{
		_rigidbody.velocity = (isRight ? transform.right : -transform.right) * _weaponSO.WeaponSpeed;
	}
	#region TRIGGER
	protected void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out IDamageable damageableObject))
		{
			damageableObject.TakeDamage(_weaponSO.WeaponDamage);
			Destroy(this);
		}
	}

	#endregion TRIGGER
}

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VComponent.Multiplayer;

public class MultiplayerShipMilitaryController : MonoBehaviour, IDamageable
{
    [SerializeField] private ushort _lifeValue = 100; 
    [SerializeField] private ushort _attackSpeedValue = 2; 
    [SerializeField] private ushort _defenseValue = 10;
    [SerializeField] private WeaponController _weaponController;
    [SerializeField] private Transform _rightSpawnWeaponTransform;
    [SerializeField] private Transform _leftSpawnWeaponTransform;

    private bool _isTargetable = false;

    public ushort LifeValue => _lifeValue;
    public ushort AttackSpeedValue => _attackSpeedValue;
    public ushort DefenseValue => _defenseValue;
    public bool IsTargetable => _isTargetable;

    public async void AttackEnemy(MultiplayerShipMilitaryController enemyShip, bool isRight)
	{
		while (enemyShip.IsTargetable)
		{
            WeaponController weaponController = Instantiate(_weaponController, isRight ? _rightSpawnWeaponTransform : _leftSpawnWeaponTransform);
            weaponController.LaunchWeapon(isRight);
            await UniTask.Delay(_attackSpeedValue * 1000);
		}
	}

	public void TakeDamage(int damage)
	{
		_lifeValue -= (ushort)damage;
        if(_lifeValue <= 0)
		{
            OnDeath();
		}
	}

	public void OnDeath()
	{
		throw new System.NotImplementedException();
	}

	public void SetTargetableValue(bool value)
	{
        _isTargetable = value;
	}

	#region TRIGGER

	private void OnTriggerEnter(Collider other)
	{
        //OTHER SHIP
        if (other.TryGetComponent(out MultiplayerShipMilitaryController multiplayerShipMilitaryController))
        {
            string shipName = other.gameObject.GetComponent<OwnerComponentManager>().PlayerNameTxt.text;
            Debug.Log($"Encounter ship {shipName}");
            multiplayerShipMilitaryController.SetTargetableValue(true);
        }
    }

	private void OnTriggerExit(Collider other)
	{
        //OTHER SHIP
        if (other.TryGetComponent(out MultiplayerShipMilitaryController multiplayerShipMilitaryController))
        {
            multiplayerShipMilitaryController.SetTargetableValue(false);
        }
    }

	#endregion TRIGGER
}

using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using VComponent.InputSystem;

namespace VComponent.Ship
{
	public class ShipMilitaryController : MonoBehaviour
	{
		[SerializeField] private ushort _attackSpeedValue = 2;
		[SerializeField] private ushort _defenseValue = 10;
		[SerializeField] private WeaponController _weaponController;
		[SerializeField] private Transform _spawnWeaponTransform;

		private readonly List<ShipMilitaryController> _opponentShipController = new();
		private bool _currentlyAttackingOpponent;
		private bool _stopAttacking;
		public ushort AttackSpeedValue => _attackSpeedValue;
		public ushort DefenseValue => _defenseValue;

		private bool _initialized;
		private ushort _lifeValue;

		public ushort LifeValue => _lifeValue;

		public void Initialize()
		{
			_initialized = true;
		}

		private void Update()
		{
			if (!_initialized)
			{
				return;
			}
			
			if (_opponentShipController.Count != 0 && InputsManager.Instance.OnInteract)
			{
				// Raycast to detect if the player clicked on another ship
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;

				if (Physics.Raycast(ray, out hit))
				{
					if (hit.collider.CompareTag("Ship")) // Assuming boats have a "Ship" tag
					{
						if (_currentlyAttackingOpponent == false)
						{
							//Attack The enemy for his position
							AttackEnemy(hit.collider.transform.position);
						}
					}
					else
					{
						//We want to cancel the attack
						_stopAttacking = true;
					}
				}
			}
		}

		private async void AttackEnemy(Vector3 enemyShipPosition, ServerRpcParams rpcParams = default)
		{
			//Checking if we have opponent on our range and if we can attack (if we start the loop)
			if (_opponentShipController.Count != 0 && _stopAttacking == false)
			{
				_currentlyAttackingOpponent = true;

				//Instantiate and fire the weapon that we carried (Cannonball by default)
				WeaponController weaponController = Instantiate(_weaponController, transform.position, transform.rotation, null);
				weaponController.GetComponent<NetworkObject>().SpawnAsPlayerObject(rpcParams.Receive.SenderClientId, true);
				NetworkObjectReference reference = new NetworkObjectReference(weaponController.GetComponent<NetworkObject>());

				weaponController.FireWeapon(enemyShipPosition);

				//Wait the attackSpeedValue to reattack again
				await UniTask.Delay(_attackSpeedValue * 1000);

				//Start the loop of re attacking (I am currently taking the first ship on the list, but we need the reference of the ship that we clicked on (mais flemme de le faire ce soir xDDDDD))
				AttackEnemy(_opponentShipController[0].transform.position);
			}
			else
			{
				_currentlyAttackingOpponent = false;
				_stopAttacking = false;
			}
		}
		
		#region TRIGGER

		private void OnTriggerEnter(Collider other)
		{
			//OTHER SHIP
			if (other.TryGetComponent(out ShipMilitaryController multiplayerShipMilitaryController))
			{
				//string shipName = other.gameObject.GetComponent<OwnerComponentManager>().PlayerNameTxt.text;
				//Debug.Log($"Encounter ship {shipName}");

				//Add ship to our ranged opponent ship list
				_opponentShipController.Add(multiplayerShipMilitaryController);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			//OTHER SHIP
			if (other.TryGetComponent(out ShipMilitaryController multiplayerShipMilitaryController))
			{
				//Remove ship to our ranged opponent ship list
				_opponentShipController.Remove(multiplayerShipMilitaryController);
			}
		}

		#endregion TRIGGER

		#region DAMAGE

		public void TakeDamage(int damage)
		{
			_lifeValue -= (ushort)damage;
		}

		#endregion
	}
}
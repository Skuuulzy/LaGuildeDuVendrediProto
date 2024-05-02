using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VComponent.InputSystem;
using VComponent.Multiplayer;

public class MultiplayerShipMilitaryController : MonoBehaviour
{

    [SerializeField] private ushort _attackSpeedValue = 2; 
    [SerializeField] private ushort _defenseValue = 10;
    [SerializeField] private WeaponController _weaponController;
    [SerializeField] private Transform _spawnWeaponTransform;

    private List<MultiplayerShipMilitaryController> _opponentShipController;
	private bool _currentlyAttackingOpponent = false;
	private bool _stopAttacking = false;
    public ushort AttackSpeedValue => _attackSpeedValue;
    public ushort DefenseValue => _defenseValue;


	private void Start()
	{
        _opponentShipController = new List<MultiplayerShipMilitaryController>();
	}

	private void Update()
	{
		if(_opponentShipController.Count != 0 && InputsManager.Instance.OnInteract == true)
		{
			// Raycast to detect if the player clicked on another ship
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit))
			{
				if (hit.collider.CompareTag("Ship")) // Assuming boats have a "Boat" tag
				{
					if(_currentlyAttackingOpponent == false)
					{
						//Attack The ennemy for his position
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
	public async UniTask AttackEnemy(Vector3 enemyShipPosition)
	{
		//Checking if we have opponent on our range and if we can attack (if we start the loop)
		if(_opponentShipController.Count != 0 && _stopAttacking == false)
		{
			_currentlyAttackingOpponent = true;

			//Instantiate and fire the weapon that we carried (Cannonball by default)
            WeaponController weaponController = Instantiate(_weaponController, transform.position, transform.rotation, null);
            weaponController.FireWeapon(enemyShipPosition);

			//Wait the attackSpeedValue to reattack again
            await UniTask.Delay(_attackSpeedValue * 1000);

			//Start the loop of reattacking (I am currently taking the first ship on the list but we need the reference of the ship that we clicked on (mais flemme de le faire ce soir xDDDDD))
			AttackEnemy(_opponentShipController[0].transform.position);
		}
		else
		{
			_currentlyAttackingOpponent = false;
			_stopAttacking = false;
			return;
		}
	}


	#region TRIGGER

	private void OnTriggerEnter(Collider other)
	{
        //OTHER SHIP
        if (other.TryGetComponent(out MultiplayerShipMilitaryController multiplayerShipMilitaryController))
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
        if (other.TryGetComponent(out MultiplayerShipMilitaryController multiplayerShipMilitaryController))
        {
			//Remove ship to our ranged opponent ship list
			_opponentShipController.Remove(multiplayerShipMilitaryController);
		}
	}

	#endregion TRIGGER
}

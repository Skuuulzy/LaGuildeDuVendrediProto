using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickUpBoxInteraction : MonoBehaviour, IInteractable
{
	[SerializeField] private GameObject _popUpCanvas;
	[SerializeField] private TMP_Text _popUpText;
	[SerializeField] private TMP_Text _nameOfItemsStored;
	[SerializeField] private Item _item;
	[SerializeField] private int _quantityOfItems;

	public void GetName()
	{
		_nameOfItemsStored.text = _item.ItemType.ToString() + " : " + _quantityOfItems;
	}

	public void GetPriority()
	{
		
	}

	public void HideInteractPopUp()
	{
		_popUpCanvas.SetActive(false);
	}

	public void Interact()
	{
		if (this.gameObject.activeInHierarchy == false)
		{
			return;
		}

		if(_quantityOfItems == 0)
		{
			Debug.Log("No more items to take");
			return;
		}
		
		_quantityOfItems--;
		//Raise a event to give player an item
		GetName();
	}

	public void ShowInteractPopUp(string key)
	{
		_popUpText.text = key;
		_popUpCanvas.SetActive(true);
		GetName();
	}
}

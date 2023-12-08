using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GDV/ScriptableObjects/Items")]
public class Item : ScriptableObject 
{
	[SerializeField] private ItemsType _itemType;

	public ItemsType ItemType => _itemType;
}

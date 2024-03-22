using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VComponent.Items.Merchandise;

[CreateAssetMenu(menuName = "ProjectV/Island/MerchandiseIslandSO")]
public class RessourcesIslandSO : IslandSO
{
	[SerializeField] protected SerializableDictionary<MerchandiseSO, ushort> _merchandisesToSell;
	[SerializeField] protected uint _merchandiseToSellTimeInterval = 180;
	public SerializableDictionary<MerchandiseSO, ushort> MerchandisesToSell => _merchandisesToSell;
	public float MerchandiseToSellTimeInterval => _merchandiseToSellTimeInterval;
}
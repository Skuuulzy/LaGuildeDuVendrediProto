using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VComponent.Items.Merchandise;

public class IslandSO : ScriptableObject
{
	[SerializeField] protected string _islandName;
	public string IslandName => _islandName;
	[SerializeField] protected uint _resourcesTimeLoadAndSell = 2;
	[SerializeField] protected ushort _resourceAmountLoadAndSell = 50;

	public uint ResourceTimeLoadAndSell => _resourcesTimeLoadAndSell;
	public ushort ResourceAmountLoadAndSell => _resourceAmountLoadAndSell;

}
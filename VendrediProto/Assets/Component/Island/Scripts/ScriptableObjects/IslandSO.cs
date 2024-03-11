using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VComponent.Items.Merchandise;

public class IslandSO : ScriptableObject
{
	[SerializeField] protected string _islandName;
	public string IslandName => _islandName;

}

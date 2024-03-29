using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VComponent.Items.Merchandise;

[CreateAssetMenu(menuName = "ProjectV/Island/MerchandiseIslandSO")]
public class ResourcesIslandSO : IslandSO
{
	[SerializeField] protected ResourcesSO _merchandisesToSell;
	public ResourcesSO  MerchandisesToSell => _merchandisesToSell;
}
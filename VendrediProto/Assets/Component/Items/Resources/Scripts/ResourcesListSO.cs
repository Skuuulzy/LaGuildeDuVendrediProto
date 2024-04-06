using System.Collections.Generic;
using UnityEngine;

namespace VComponent.Items.Merchandise
{
	[CreateAssetMenu(menuName = "ProjectV/Item/Merchandise List")]
	public class ResourcesListSO : ScriptableObject
	{
		[SerializeField] private List<ResourcesSO> _allMerchandises;
		public List<ResourcesSO> AllMerchandise => _allMerchandises;

		public ResourcesSO GetMerchandiseByType(ResourceType merchandiseType)
		{
			return _allMerchandises.Find(x => x.Type == merchandiseType);
		}
	}
}
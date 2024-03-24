using System.Collections.Generic;
using UnityEngine;

namespace VComponent.Items.Merchandise
{
	[CreateAssetMenu(menuName = "ProjectV/Item/Merchandise List")]
	public class RessourcesListSO : ScriptableObject
	{
		[SerializeField] private List<RessourcesSO> _allMerchandises;
		public List<RessourcesSO> AllMerchandise => _allMerchandises;

		public RessourcesSO GetMerchandiseByType(RessourceType merchandiseType)
		{
			return _allMerchandises.Find(x => x.Type == merchandiseType);
		}
	}
}
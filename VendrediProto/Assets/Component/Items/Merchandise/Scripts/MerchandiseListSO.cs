using System.Collections.Generic;
using UnityEngine;

namespace VComponent.Items.Merchandise
{
	[CreateAssetMenu(menuName = "ProjectV/Item/Merchandise List")]
	public class MerchandiseListSO : ScriptableObject
	{
		[SerializeField] private List<MerchandiseSO> _allMerchandises;
		public List<MerchandiseSO> AllMerchandise => _allMerchandises;

		public MerchandiseSO GetMerchandiseByType(MerchandiseType merchandiseType)
		{
			return _allMerchandises.Find(x => x.Type == merchandiseType);
		}
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ProjectV/Merchandise")]
public class MerchandiseSO : ScriptableObject
{
	[SerializeField] private List<MerchandiseData> _allMerchandises;

	public List<MerchandiseData> AllMerchandise => _allMerchandises;

	public MerchandiseData GetMerchandiseData(MerchandiseType merchandiseType)
	{
		return _allMerchandises.Find(x => x.MerchandiseType == merchandiseType);
	}
}

[Serializable]
public struct MerchandiseData
{
	public MerchandiseType MerchandiseType;
	public Sprite MerchandiseSprite;
	public int SellValue;
}

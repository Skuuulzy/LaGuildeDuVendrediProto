using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIIslandInfoController : MonoBehaviour
{
	[SerializeField] private PlayerUIIslandInfoView _islandInfoView;

	private IslandController _islandController;
	private ShipController _shipController;

	[Header("Data")]
	[SerializeField] private MerchandiseSO _allMerchandise;
	public void Init(IslandController islandController, ShipController shipController)
	{
		_islandController = islandController;
		islandController.OnUpdateMerchandise -= UpdateView;
		islandController.OnUpdateMerchandise += UpdateView;
		_islandController = islandController;
		_shipController = shipController;

		MerchandiseInfos merchandiseInfos = GetMerchandiseInfos();
		_islandInfoView.Init(_islandController, _shipController, merchandiseInfos.MerchandiseData, merchandiseInfos.MerchandiseCarriedNumber, merchandiseInfos.SellPrice);
	}
	private void OnDestroy()
	{
		if (_islandController != null)
		{
			_islandController.OnUpdateMerchandise -= UpdateView;
		}
	}
	public void UpdateView()
	{
        if (_islandController == null || _shipController == null)
        {
			Debug.LogError("No IslandController or ShipController saved");
			return;
        }

		MerchandiseInfos merchandiseInfos = GetMerchandiseInfos();
		_islandInfoView.Init(_islandController, _shipController, merchandiseInfos.MerchandiseData, merchandiseInfos.MerchandiseCarriedNumber, merchandiseInfos.SellPrice);
	}

	public void RemoveUpdateIslandListener()
	{
		if (_islandController == null )
		{
			Debug.LogError("No IslandController saved");
			return;
		}

		_islandController.OnUpdateMerchandise -= UpdateView;
		_islandController = null;
		_shipController = null;
	}

	public void SellMerchandise()
	{
		MerchandiseInfos merchandiseInfos = GetMerchandiseInfos();
		_shipController.SellMerchandise(merchandiseInfos.SellPrice);

		//Update Island Asked Merchandise
		//Update view
		//Update Ship Merchandise (selled merchandise)
	}
	
	private MerchandiseInfos GetMerchandiseInfos()
	{
		MerchandiseInfos merchandiseInfos = new MerchandiseInfos();
		merchandiseInfos.MerchandiseData = _allMerchandise.GetMerchandiseData(_islandController.CurrentMerchandiseAsked);
		merchandiseInfos.MerchandiseCarriedNumber = _shipController.CurrentMerchandiseCarriedType == merchandiseInfos.MerchandiseData.MerchandiseType ? _shipController.CurrentMerchandiseCarriedNumber : 0;
		merchandiseInfos.SellPrice = merchandiseInfos.MerchandiseCarriedNumber * merchandiseInfos.MerchandiseData.SellValue;
		return merchandiseInfos;
	}

	#region STRUCTS

	private struct MerchandiseInfos
	{
		public MerchandiseData MerchandiseData;
		public int MerchandiseCarriedNumber;
		public int SellPrice;
	}

	#endregion STRUCTS
}



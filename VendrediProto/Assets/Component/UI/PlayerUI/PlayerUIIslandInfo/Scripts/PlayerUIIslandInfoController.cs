using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VComponent.Items.Merchandise;

public class PlayerUIIslandInfoController : MonoBehaviour
{
	[SerializeField] private PlayerUIIslandInfoView _islandInfoView;

	private IslandController _islandController;
	private ShipController _shipController;

	[Header("Data")]
	[SerializeField] private MerchandiseListSO _allMerchandiseList;
	public void Init(IslandController islandController, ShipController shipController)
	{
		_islandController = islandController;
		islandController.OnUpdateMerchandise -= UpdateView;
		islandController.OnUpdateMerchandise += UpdateView;
		_islandController = islandController;
		_shipController = shipController;

		MerchandiseSO merchandiseData = GetMerchandiseInfos();
		_islandInfoView.Init(_islandController, _shipController, merchandiseData);
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

        MerchandiseSO merchandiseData = GetMerchandiseInfos();
		_islandInfoView.Init(_islandController, _shipController, merchandiseData);
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
		if(_islandController.CurrentMerchandiseAsked != _shipController.CurrentMerchandiseCarriedType)
		{
			return; 
		}

		MerchandiseSO merchandiseData = GetMerchandiseInfos();
		_islandController.ReceiveMerchandise(_shipController.CurrentMerchandiseCarriedNumber);
		_shipController.SellMerchandise(merchandiseData.SellValue);

		UpdateView();
		//Update Island Asked Merchandise
		//Update view
		//Update Ship Merchandise (selled merchandise)
	}
	
	private MerchandiseSO GetMerchandiseInfos()
	{
		MerchandiseSO merchandiseData = _allMerchandiseList.GetMerchandiseByType(_islandController.CurrentMerchandiseAsked);
		return merchandiseData;
	}

}



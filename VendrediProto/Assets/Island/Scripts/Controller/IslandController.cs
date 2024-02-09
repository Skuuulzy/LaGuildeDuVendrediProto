using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IslandController : MonoBehaviour
{
    [SerializeField] private IslandView _islandView;

    [SerializeField] private IslandSO _islandSO;

    private MerchandiseType _currentMerchandiseAsked;
    private int _currentMerchandiseAskedValue;
    private bool _isAskingForAMerchandise;
    public void AskForMerchandise()
    {
        int randomNumber = Random.Range(0, _islandSO.MerchandisesRequested.ToDictionary().Count);
        _currentMerchandiseAsked = _islandSO.MerchandisesRequested.ToDictionary().ElementAt(randomNumber).Key;
		_currentMerchandiseAskedValue = _islandSO.MerchandisesRequested.ToDictionary().ElementAt(randomNumber).Value;
        _islandView.DisplayMerchandiseAsked(_currentMerchandiseAsked, _currentMerchandiseAskedValue);
	}
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class RessourcesIslandController : MonoBehaviour
{
    [SerializeField] private RessourcesIslandSO _islandData;
	[SerializeField] private RessourcesIslandView _view;

    public RessourcesIslandSO IslandData => _islandData;

	private void Start()
	{
		_view.Init(_islandData);
	}

	public void GetMerchandise(int value)
    {

    }
}

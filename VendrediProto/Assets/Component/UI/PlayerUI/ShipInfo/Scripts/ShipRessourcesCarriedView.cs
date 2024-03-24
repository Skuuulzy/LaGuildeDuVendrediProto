using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VComponent.Items.Merchandise;

public class ShipRessourcesCarriedView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currentRessourceCarrriedNumber;
    [SerializeField] private Image _currentRessourceCarriedImage;

    private RessourceType _currentRessourceType = RessourceType.NONE;
    public RessourceType CurrentRessourceType => _currentRessourceType;

    public void Init(int numberOfRessourceCarried, RessourcesSO ressourcesSO)
    {
        UpdateNumberOfRessourceCarried(numberOfRessourceCarried);
        _currentRessourceType = ressourcesSO.Type;
		_currentRessourceCarriedImage.sprite = ressourcesSO.Sprite;
        this.gameObject.SetActive(true);
	}

    public void UpdateNumberOfRessourceCarried(int numberOfRessourceCarried)
    {
		_currentRessourceCarrriedNumber.text = numberOfRessourceCarried.ToString();
	}

	internal void Hide()
	{
		this.gameObject.SetActive(false);
	}
}

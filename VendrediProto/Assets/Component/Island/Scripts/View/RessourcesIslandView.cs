using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RessourcesIslandView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _islandNameText;
    [SerializeField] private Image _islandRessourcesImage;

    public void Init(RessourcesIslandSO islandSO)
    {
        _islandNameText.text = islandSO.IslandName;
        _islandRessourcesImage.sprite = islandSO.MerchandisesToSell.Sprite;
    }


}

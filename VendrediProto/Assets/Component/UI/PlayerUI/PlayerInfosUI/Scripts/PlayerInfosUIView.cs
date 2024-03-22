using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInfosUIView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _goldValue;

    public void SetGoldValue(int goldValue)
    {
        _goldValue.text = goldValue.ToString();
    }
}

using System;
using TMPro;
using UnityEngine;

public class PlayerInfosUIView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _goldValue;
    [SerializeField] private TextMeshProUGUI _gameTimeTxt;

    public void SetGoldValue(int goldValue)
    {
        _goldValue.text = goldValue.ToString();
    }

    public void DisplayTimeRemaining(float timeRemaining)
    {
        var timeSpan = TimeSpan.FromSeconds(timeRemaining);

        _gameTimeTxt.text = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
    }
}
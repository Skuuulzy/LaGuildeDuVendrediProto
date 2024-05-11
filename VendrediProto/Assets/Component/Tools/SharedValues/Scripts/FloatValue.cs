using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Shared Values/Float")]
public class FloatValue : ScriptableObject
{
    public float Value => _value;
    public Action<float> OnValueUpdated;
    
    [SerializeField] private float _value;
    [SerializeField] private bool _readOnly;
    
    public void SetValue(float newValue)
    {
        if (_readOnly)
        {
            Debug.LogError($"Cannot set the value of {name}, the shared value is set to read only.");
            return;
        }
        
        _value = newValue;
        OnValueUpdated?.Invoke(_value);
    }
}
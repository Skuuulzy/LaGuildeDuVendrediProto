using UnityEngine;

[CreateAssetMenu(menuName = "ProjectV/Weapons/WeaponSO")]
public class WeaponSO : ScriptableObject
{
    [SerializeField] private string _weaponName;
    [SerializeField] private WeaponType _weaponType;
    [SerializeField] private int _weaponDamage;
    [SerializeField] private int _weaponSpeed;

    public string WeaponName => _weaponName;
    public WeaponType WeaponType => _weaponType;
    public int WeaponDamage => _weaponDamage;
    public int WeaponSpeed => _weaponSpeed;

}

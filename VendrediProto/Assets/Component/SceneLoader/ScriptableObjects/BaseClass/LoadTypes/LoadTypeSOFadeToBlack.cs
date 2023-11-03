using UnityEngine;

/// <summary>
/// Used for a fade to black -> load -> fade out loading type.
/// </summary>
[CreateAssetMenu(menuName = "Tools/Loader/Fade to black", order = 1)]
public class LoadTypeSOFadeToBlack : LoadTypeSO
{
    [SerializeField] private float _fadeInTime;
    [SerializeField] private float _fadeOutTime;

    public float FadeInTime => _fadeInTime;
    public float FadeOutTime => _fadeOutTime;
}

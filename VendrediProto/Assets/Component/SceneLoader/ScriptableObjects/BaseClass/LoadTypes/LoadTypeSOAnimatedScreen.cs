using UnityEngine;

/// <summary>
/// References a prefab holding an animation that will be instanciated, then destroyed once the loading is complete
/// </summary>
[CreateAssetMenu(menuName = "Tools/Loader/Animated Screen", order = 1)]
public class LoadTypeSOAnimatedScreen : LoadTypeSO
{
    [SerializeField] private Animator _animationPrefab;

    public Animator AnimationPrefab => _animationPrefab;
}

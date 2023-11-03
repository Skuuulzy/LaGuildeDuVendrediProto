using UnityEngine;

/// <summary>
/// Classic static loading screen, references the sprite to display
/// </summary>
[CreateAssetMenu(menuName = "Tools/Loader/Static Screen", order = 1)]
public class LoadTypeSOStaticScreen : LoadTypeSO
{
    [SerializeField] private Sprite _spriteToDisplay;

    public Sprite SpriteToDisplay => _spriteToDisplay;
}

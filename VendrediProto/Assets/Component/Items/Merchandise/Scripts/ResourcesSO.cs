using UnityEngine;

namespace VComponent.Items.Merchandise
{
    [CreateAssetMenu(menuName = "ProjectV/Item/Ressources")]
    public class ResourcesSO : ScriptableObject
    {
        [SerializeField] private ResourceType _type;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private int _sellValue;

        public ResourceType Type => _type;
        public Sprite Sprite => _sprite;
        public int SellValue => _sellValue;
    }
}
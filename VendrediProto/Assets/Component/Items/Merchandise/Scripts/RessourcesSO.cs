using UnityEngine;

namespace VComponent.Items.Merchandise
{
    [CreateAssetMenu(menuName = "ProjectV/Item/Ressources")]
    public class RessourcesSO : ScriptableObject
    {
        [SerializeField] private RessourceType _type;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private int _sellValue;

        public RessourceType Type => _type;
        public Sprite Sprite => _sprite;
        public int SellValue => _sellValue;
    }
}
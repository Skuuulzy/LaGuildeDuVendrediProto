using UnityEngine;

namespace VComponent.Items.Merchandise
{
    [CreateAssetMenu(menuName = "ProjectV/Item/Merchandise")]
    public class MerchandiseSO : ScriptableObject
    {
        [SerializeField] private MerchandiseType _type;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private int _sellValue;

        public MerchandiseType Type => _type;
        public Sprite Sprite => _sprite;
        public int SellValue => _sellValue;
    }
}
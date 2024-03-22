using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VComponent.Tools.Singletons
{
    public class SingletonHolder : MonoBehaviour
    {
        private async void Start()
        {
            await UniTask.WaitForEndOfFrame(this);
            Destroy(gameObject);
        }
    }
}
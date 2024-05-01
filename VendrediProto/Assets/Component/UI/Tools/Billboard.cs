using UnityEngine;

namespace VComponent.UI.Tools
{
    public class Billboard : MonoBehaviour
    {
        private Transform _cameraTransform;
        private bool _cameraFound;

        private void TryFindCam()
        {
            var cam = Camera.main;
            if (cam == null) return;
            
            _cameraTransform = cam.transform;
            _cameraFound = true;
        }

        private void Update()
        {
            if (!_cameraFound)
            {
                // TODO : Make this more efficient
                TryFindCam();
                return;
            }

            transform.rotation = _cameraTransform.rotation;
        }
    }
}


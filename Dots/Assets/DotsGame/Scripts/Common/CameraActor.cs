using Dots.AC;
using UnityEngine;

namespace DotsGame.Common
{
    [RequireComponent(typeof(Camera))]
    public class CameraActor : BaseActor
    {
        private Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        public void SetBackgroundColor(Color color)
        {
            _camera.backgroundColor = color;
        }
    }
}
using Dots.Extras;
using UnityEngine;

namespace Dots.AC
{
    public interface IActor : IDisposableOwner
    {
        Transform Transform { get; }
        bool IsActive { get; set; }

        void SetParent(Transform parent, bool worldPositionsStays);

        void Destroy();
    }
}
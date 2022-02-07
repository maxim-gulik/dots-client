using System;

namespace Dots.Extras
{
    public interface IDisposableOwner
    {
        void AddOwnership(IDisposable disposable);
        void RemoveOwnership(IDisposable disposable, bool dispose = false);
    }
}
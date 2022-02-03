using System.Collections;
using UnityEngine;

namespace Dots.Infra.Utils
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
    }
}
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public interface IUnloadSceneCommand
{
    UniTask ExecuteAsync(string name, UnloadSceneOptions options = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
}

public class UnloadSceneCommand : IUnloadSceneCommand
{
    public UniTask ExecuteAsync(string name, UnloadSceneOptions options = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects)
    {
        return SceneManager.UnloadSceneAsync(name, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects).ToUniTask();
    }
}
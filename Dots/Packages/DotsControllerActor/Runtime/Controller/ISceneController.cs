using Cysharp.Threading.Tasks;

namespace Dots.AC
{
    public interface ISceneController : IController
    {
        UniTask StartAsync();
    }
}
using Cysharp.Threading.Tasks;

namespace Dots.Infra.AC
{
    public interface ISceneController : IController
    {
        UniTask StartAsync();
    }
}
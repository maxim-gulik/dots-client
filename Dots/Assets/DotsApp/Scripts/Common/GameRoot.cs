using Cysharp.Threading.Tasks;
using Dots.Infra.AC;

namespace DotsApp.Common
{
    public class GameRoot : BaseSceneRootMonoBehavior<GameRoot.Controller>
    {
        protected override string SceneId => "Game";

        public class Controller : BaseController, ISceneController
        {
            private readonly ICreateControllerCommand _createControllerCommand;

            public Controller(ICreateControllerCommand createControllerCommand)
            {
                _createControllerCommand = createControllerCommand;
            }

            protected override void OnDispose() { }

            public UniTask StartAsync()
            {


                return UniTask.CompletedTask;
            }
        }
    }
}

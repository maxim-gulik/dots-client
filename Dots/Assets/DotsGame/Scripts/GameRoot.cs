using Cysharp.Threading.Tasks;
using Dots.Infra.AC;
using DotsGame.EndlessMode;

namespace DotsGame
{
    /// <summary>
    /// Root monobehavior for all business logic in the game.
    /// Contains RootController responsible for the start of all game subsystems
    /// </summary>
    public class GameRoot : BaseSceneRootMonoBehavior<GameRoot.Controller>
    {
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
                //so far there is only one mode :)
                var dotsFieldController = _createControllerCommand.Execute<EndlessGameController>(this);
                return dotsFieldController.PlayDotsAsync();
            }
        }
    }
}

using DotsGame.Common;
using DotsGame.EndlessMode;
using UnityEngine;
using Zenject;

namespace DotsGame
{
    /// <summary>
    /// Zenject installer only for game injections
    /// </summary>
    public class GameInstaller : MonoInstaller
    {
        [Header("General")]
        [SerializeField] private GridActor _gridActor;
        [SerializeField] private DotActor _dotActorPrefab;
        [SerializeField] private CameraActor _mainCameraActor;

        [Header("Game modes settings")]
        [SerializeField] private EndlessModeGameSettings _endlessModeSettings;

        public override void InstallBindings()
        {
            Container.Bind<GameRoot.Controller>().AsTransient();

            Container.Bind<DotsSelectionController>().AsTransient();
            Container.Bind<LinePainterController>().AsTransient();
            Container.Bind<DotsFieldController>().AsTransient();

            Container.Bind<GridActor>().FromInstance(_gridActor).AsTransient();
            Container.Bind<DotActor>().FromInstance(_dotActorPrefab).AsTransient();
            Container.Bind<CameraActor>().WithId("MainCamera").FromInstance(_mainCameraActor).AsTransient();

            Container.Bind<EndlessGameController>().AsTransient();
            Container.Bind<EndlessModeGameSettings>().FromInstance(_endlessModeSettings).AsTransient();
        }
    }
}

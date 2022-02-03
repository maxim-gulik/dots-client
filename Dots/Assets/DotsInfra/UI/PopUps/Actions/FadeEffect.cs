using System.Threading;
using Cysharp.Threading.Tasks;
using Dots.Infra.AC;

namespace Dots.Infra.UI
{
    public class FadeEffect : BasePopULifecycleAction
    {
        private readonly IInstantiateActorCommand _instantiateActorCommand;

        private IFaderActor _fader;

        public FadeEffect(IInstantiateActorCommand instantiateActorCommand)
        {
            _instantiateActorCommand = instantiateActorCommand;
        }

        public override int Id => (int) PopUpActions.Fade;

        public override async UniTask OnOpeningAsync(IPopUpView view, CancellationToken token)
        {
            await base.OnOpeningAsync(view, token);

            _fader = _instantiateActorCommand.Execute<IFaderActor>(view, worldPositionStays: false);
            _fader.Transform.SetAsFirstSibling();
        }

        public override async UniTask OnActivatedAsync(bool silent, IPopUpView view, CancellationToken token)
        {
            await base.OnActivatedAsync(silent, view, token);
            _fader.Show();
        }

        public override async UniTask OnDeactivatingAsync(bool silent, IPopUpView view, CancellationToken token)
        {
            await base.OnDeactivatingAsync(silent, view, token);

            _fader?.Hide(); //null check in case if scene destroyed the fader on its own before the deactivation
        }

        public override void OnDispose()
        {
            base.OnDispose();

            _fader?.Destroy();
            _fader = null;
        }
    }
}
using System.Threading;
using Cysharp.Threading.Tasks;
using Dots.Infra.AC;

namespace Dots.Infra.UI
{
    public class InformationPopupController: BasePopUpController<IInformationPopupView, InformationPopUpData>
    {
        private readonly IInstantiateActorCommand _instantiateActorCommand;

        public InformationPopupController(IInstantiateActorCommand instantiateActorCommand)
        {
            _instantiateActorCommand = instantiateActorCommand;

            AddLifecycleActions(new FadeEffect(instantiateActorCommand));
        }

        protected override UniTask<IInformationPopupView> CreatePopupViewAsync(CancellationToken token)
        {
            return _instantiateActorCommand.ExecuteAsync<IInformationPopupView>("InformationPopUp.prefab", "CommonUI", token);
        }

        protected override async UniTask OnOpeningAsync(IInformationPopupView view, InformationPopUpData data, CancellationToken token)
        {
            await base.OnOpeningAsync(view, data, token);

            view.SetData(data);
        }

        protected override async UniTask OnOpenedAsync(IInformationPopupView view, InformationPopUpData data, CancellationToken token)
        {
            await base.OnOpenedAsync(view, data, token);

            view.CloseClick += OnCloseClick;

            void OnCloseClick()
            {
                view.CloseClick -= OnCloseClick;
                ClosePopUpAsync(DisposeToken).Forget();
            }
        }
    }
}
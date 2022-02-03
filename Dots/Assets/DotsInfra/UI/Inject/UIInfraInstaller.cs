using System;
using UnityEngine;
using Zenject;

namespace Dots.Infra.UI
{
    public class UIInfraInstaller : MonoInstaller
    {
        [SerializeField] private FaderActor _fader;
        [SerializeField] private UILayoutActor _layout;

        public override void InstallBindings()
        {
            Container.Bind(typeof(IPopUpsStack), typeof(IDisposable)).To<PopUpsStack>().AsSingle();

            Container.Bind<IUILayoutActor>().To<UILayoutActor>()
                .FromInstance(_layout)
                .AsSingle();

            Container.Bind<IFaderActor>().To<FaderActor>()
                .FromInstance(_fader)
                .AsSingle()
                .OnInstantiated<IFaderActor>((c, f) => f.IsActive = true);

            Container.Bind<InformationPopupController>().AsTransient();
            Container.Bind<IInformationPopupView>().To<InformationPopupView>().AsTransient();
        }
    }
}

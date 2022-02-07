using System;
using Dots.Extras;
using Zenject;

namespace Dots.Infra.AC
{
    public interface ICreateControllerCommand
    {
        TController Execute<TController>(IDisposableOwner owner = default) where TController : IController;
        TController Execute<TController>(Type type, IDisposableOwner owner = default) where TController : IController;
        TController Execute<TController, TParameters>(TParameters parameters, IDisposableOwner owner = default)
            where TController : IController
            where TParameters : class;
    }

    /// <summary>
    /// Base class for all controllers (classes responsible for the business logic of a game)
    /// Disposable and can be as an owner for a set of disposable instances
    /// </summary>
    public class CreateControllerCommand : ICreateControllerCommand
    {
        private readonly DiContainer _container;

        public CreateControllerCommand(DiContainer container)
        {
            _container = container;
        }

        public TController Execute<TController>(IDisposableOwner owner = default) where TController : IController
        {
            var controller = _container.Resolve<TController>();
            owner?.AddOwnership(controller);

            return controller;
        }

        public TController Execute<TController, TParameters>(TParameters parameters, IDisposableOwner owner = default)
            where TController : IController
            where TParameters : class
        {
            _container.Rebind<TParameters>().FromInstance(parameters);
            var controller = _container.Resolve<TController>();
            _container.Unbind<TParameters>();

            owner?.AddOwnership(controller);

            return controller;
        }

        public TController Execute<TController>(Type type, IDisposableOwner owner = default) where TController : IController
        {
            var controller = (TController) _container.Resolve(type);
            owner?.AddOwnership(controller);
            return controller;
        }
    }
}
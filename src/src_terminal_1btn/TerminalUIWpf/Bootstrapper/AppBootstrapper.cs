using System;
using System.Windows;
using Caliburn.Micro;
using Castle.Windsor;
using TerminalUIWpf.ViewModels;


namespace TerminalUIWpf.Bootstrapper
{
    public class AppBootstrapper : BootstrapperBase
    {
        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<AppViewModel>();
        }





        private readonly IWindsorContainer _container = new WindsorContainer();

        protected override void Configure()
        {
            _container.Install(new WindsorConfig());
        }


        protected override object GetInstance(Type service, string key)
        {
            return string.IsNullOrWhiteSpace(key)

                ? _container.Kernel.HasComponent(service)
                    ? _container.Resolve(service)
                    : base.GetInstance(service, key)

                : _container.Kernel.HasComponent(key)
                    ? _container.Resolve(key, service)
                    : base.GetInstance(service, key);
        }
    }

}
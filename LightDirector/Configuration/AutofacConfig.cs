using System.Windows;
using Autofac;
using LightDirector.Domain;
using LightDirector.Services;
using LightDirector.ViewModels;
using LightDirector.ViewModels.Dialogs.NewProject;
using Prism.Autofac;
using LightDirector.Modules.About;
using LightDirector.Modules.Lights;
using LightDirector.Modules.Cues;
using LightDirector;
using LightDirector.Infrastructure.Services;
using Prism.Regions;
using Prism.Modularity;
using LightDirector.Modules.TimeLine;
using LightDirector.Modules.Menus;
using LightDirector.Modules.Viewer;
using LightDirector.Modules.Toolbar;

namespace LightDirector.Configuration
{
   class AutofacConfig : AutofacBootstrapper
   {
      protected override DependencyObject CreateShell()
      {
         return Container.Resolve<Shell>();
      }

      protected override void InitializeShell()
      {
         base.InitializeShell();

         App.Current.MainWindow = (Window)Shell;
         App.Current.MainWindow.DataContext = Container.Resolve<ShellViewModel>();
         App.Current.MainWindow.Show();
      }

      protected override IContainer CreateContainer(ContainerBuilder containerBuilder)
      {
         containerBuilder.RegisterModule<LightsModuleRegistry>();
         containerBuilder.RegisterModule<AboutModuleRegistry>();
         containerBuilder.RegisterModule<TimeLineModuleRegistry>();
         containerBuilder.RegisterModule<CuesModuleRegistry>();
         containerBuilder.RegisterModule<MenusModuleRegistry>();
         containerBuilder.RegisterModule<ViewerModuleRegistry>();
         containerBuilder.RegisterModule<ToolbarModuleRegistry>();

         containerBuilder.RegisterType<ShellViewModel>().SingleInstance();
         containerBuilder.RegisterType<StageViewModel>().SingleInstance();
         containerBuilder.RegisterType<DmxService>().As<IDmxService>().SingleInstance();
         containerBuilder.RegisterType<DmxGateway>().As<IDmxGateway>().SingleInstance();
         containerBuilder.RegisterType<AudioPlayer>().As<IAudioPlayer>().SingleInstance();
         containerBuilder.RegisterType<CueTimeService>().As<ICueTimeService>().SingleInstance();
         containerBuilder.RegisterType<ClientGateway>().As<IClientGateway>().SingleInstance();
         containerBuilder.RegisterType<ClientRepository>().As<IClientRepository>().SingleInstance();
         containerBuilder.RegisterType<ClientCueService>().As<IClientCueService>().SingleInstance();
         containerBuilder.RegisterType<LightStateService>().As<ILightStateService>().SingleInstance();
         containerBuilder.RegisterType<KeyFrameService>().As<IKeyFrameService>().SingleInstance();
         containerBuilder.RegisterType<LightSpecificationRepository>().SingleInstance();
         containerBuilder.RegisterType<TheatreModel>().SingleInstance();
         containerBuilder.RegisterType<LightingPlanService>().As<ILightingPlanService>().SingleInstance();
         containerBuilder.RegisterType<NewProjectDialogService>().SingleInstance();
         
         containerBuilder.RegisterType<LightingPlanViewModel>();
         containerBuilder.RegisterType<CueViewModel>();
         containerBuilder.RegisterType<KeyframeViewModel>();

         containerBuilder.RegisterType<ColorRepeatEffectViewModel>().As<EffectViewModelBase>().InstancePerDependency().Named<EffectViewModelBase>("ColorRepeatEffect");
         containerBuilder.RegisterType<ColorRepeatEffectView>();

         containerBuilder.RegisterType<RandomWiggleEffectViewModel>().As<EffectViewModelBase>().InstancePerDependency().Named<EffectViewModelBase>("RandomWiggleEffect");

         containerBuilder.RegisterType<MovingSpotEffectViewModel>().As<EffectViewModelBase>().InstancePerDependency().Named<EffectViewModelBase>("MovingSpotEffect");

         containerBuilder.RegisterType<SwingingLightEffectViewModel>().As<EffectViewModelBase>().InstancePerDependency().Named<EffectViewModelBase>("SwingingLightEffect");

         containerBuilder.RegisterType<FigureOfEightViewModel>().As<EffectViewModelBase>().InstancePerDependency().Named<EffectViewModelBase>("FigureOfEightEffect");


         containerBuilder.RegisterType<StagePositionService>().As<IStagePositionService>().SingleInstance();
         containerBuilder.RegisterType<Dispatcher>().As<IDispatcher>().SingleInstance();
         containerBuilder.RegisterType<ModeService>().As<IModeService>().SingleInstance();
         containerBuilder.RegisterType<ScreenModel>();
                  
         //var nancyHost = container.Resolve<LightingDirectorNancyHost>();

         

         return base.CreateContainer(containerBuilder);
      }

      protected override void InitializeModules()
      {
         base.InitializeModules();

         var clientGateway = Container.Resolve<IClientGateway>();
         clientGateway.Start();
      }

      protected override void ConfigureModuleCatalog()
      {
         base.ConfigureModuleCatalog();
         var moduleCatalog = (ModuleCatalog)ModuleCatalog;

         moduleCatalog.AddModule(typeof(AboutModule));
         moduleCatalog.AddModule(typeof(LightsModule));
         moduleCatalog.AddModule(typeof(TimeLineModule));
         moduleCatalog.AddModule(typeof(CuesModule));
         moduleCatalog.AddModule(typeof(MenusModule));
         moduleCatalog.AddModule(typeof(ViewerModule));
         moduleCatalog.AddModule(typeof(ToolbarModule));
      }
   }
}
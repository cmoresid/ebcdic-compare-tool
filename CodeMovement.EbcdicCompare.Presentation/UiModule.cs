using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using Wcb.Eco.EbcdicCompare.DataAccess.Service;
using Wcb.Eco.EbcdicCompare.Models.Constant;
using Wcb.Eco.EbcdicCompare.Services;
using Wcb.Eco.EbcdicCompare.Ui.Controller;
using Wcb.Eco.EbcdicCompare.Ui.Interaction;
using Wcb.Eco.EbcdicCompare.Ui.Views;

namespace Wcb.Eco.EbcdicCompare.Ui
{
    public class UiModule : IModule
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IRegionManager _regionManager;
        private EbcdicFileContentRegionController _ebcdicFileContentRegionController;

        public UiModule(IUnityContainer unityContainer, IRegionManager regionManager)
        {
            _unityContainer = unityContainer;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _unityContainer.RegisterType<IEbcdicReaderService, EbcdicReaderService>();
            _unityContainer.RegisterType<ICompareEbcdicFilesService, CompareEbcdicFilesService>();
            _unityContainer.RegisterType<IFileDialogInteraction, FileDialogInteraction>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<IExternalProgramService, ExternalProgramService>();
            _unityContainer.RegisterType<ICopybookRepository, CopybookRepository>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<IFileOperationsManager, FileOperationsManager>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<ICopybookManager, CopybookManager>(new ContainerControlledLifetimeManager());

            _unityContainer.RegisterType<HomeView>();
            _unityContainer.RegisterTypeForNavigation<HomeView>();

            _unityContainer.RegisterType<OpenEbcdicFileView>();
            _unityContainer.RegisterTypeForNavigation<OpenEbcdicFileView>();

            _unityContainer.RegisterType<CompareEbcdicFilesView>();
            _unityContainer.RegisterTypeForNavigation<CompareEbcdicFilesView>();

            _unityContainer.RegisterType<ManageCopybooksView>();
            _unityContainer.RegisterTypeForNavigation<ManageCopybooksView>();

            _unityContainer.RegisterType<EbcdicFileGridView>();

            _regionManager.RegisterViewWithRegion(RegionNames.ViewEbcdicFileContentRegion,
                () => _unityContainer.Resolve<EbcdicFileGridView>());

            _regionManager.RegisterViewWithRegion(RegionNames.FirstEbcdicFileContentRegion,
                () => _unityContainer.Resolve<EbcdicFileGridView>());

            _regionManager.RegisterViewWithRegion(RegionNames.SecondEbcdicFileContentRegion,
                () => _unityContainer.Resolve<EbcdicFileGridView>());

            // Initialize our controller -> It will be kept alive through out the program's life cycle
            // because the event aggregator will keep a strong reference to it.
            _ebcdicFileContentRegionController = _unityContainer.Resolve<EbcdicFileContentRegionController>();
        }
    }
}

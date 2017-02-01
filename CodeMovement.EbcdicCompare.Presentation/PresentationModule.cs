using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using CodeMovement.EbcdicCompare.DataAccess;
using CodeMovement.EbcdicCompare.Models.Constant;
using CodeMovement.EbcdicCompare.Services;
using CodeMovement.EbcdicCompare.Presentation.Controller;
using CodeMovement.EbcdicCompare.Presentation.Interaction;
using CodeMovement.EbcdicCompare.Presentation.View;
using System.Diagnostics.CodeAnalysis;
using CodeMovement.EbcdicCompare.Presentation.ViewModel;

namespace CodeMovement.EbcdicCompare.Presentation
{
    [ExcludeFromCodeCoverage]
    public class PresentationModule : IModule
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IRegionManager _regionManager;
        private EbcdicFileContentRegionController _ebcdicFileContentRegionController;

        public PresentationModule(IUnityContainer unityContainer, IRegionManager regionManager)
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
            _unityContainer.RegisterType<IFileOperation, FileOperation>(new ContainerControlledLifetimeManager());

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
                () => CreateGridViewWith(RegionNames.ViewEbcdicFileContentRegion));

            _regionManager.RegisterViewWithRegion(RegionNames.FirstEbcdicFileContentRegion,
                () => CreateGridViewWith(RegionNames.FirstEbcdicFileContentRegion));

            _regionManager.RegisterViewWithRegion(RegionNames.SecondEbcdicFileContentRegion,
                () => CreateGridViewWith(RegionNames.SecondEbcdicFileContentRegion));

            // Initialize our controller -> It will be kept alive through out the program's life cycle
            // because the event aggregator will keep a strong reference to it.
            _ebcdicFileContentRegionController = _unityContainer.Resolve<EbcdicFileContentRegionController>();
        }

        #region "Helper Methods"

        private EbcdicFileGridView CreateGridViewWith(string regionName)
        {
            var view = _unityContainer.Resolve<EbcdicFileGridView>();
            var viewModel = view.DataContext as EbcdicFileGridViewModel;
            viewModel.RegionName = regionName;

            return view;
        }

        #endregion
    }
}

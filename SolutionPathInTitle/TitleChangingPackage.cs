using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;

namespace SolutionPathInTitle
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(TitleChangingPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideAutoLoad(UIContextGuids80.NoSolution)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    public sealed class TitleChangingPackage : Package, IVsSolutionEvents, IVsSolutionLoadEvents
    {
        public const string PackageGuidString = "2ca96599-e000-42e4-b13d-e04a70511222";
        private uint _hSolutionEvents = uint.MaxValue;
        private IVsSolution _solution;
        private string _originalTitle;

        private string WindowTitle
        {
            get { return Application.Current.MainWindow.Title; }
            set { Application.Current.MainWindow.Title = value; }
        }

        protected override void Initialize()
        {
            _solution = GetService(typeof(SVsSolution)) as IVsSolution;
            _solution.AdviseSolutionEvents(this, out _hSolutionEvents);

            base.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            _solution.UnadviseSolutionEvents(_hSolutionEvents);

            base.Dispose(disposing);
        }

        public int OnBeforeOpenSolution(string solutionFilename)
        {
            _originalTitle = WindowTitle;
            WindowTitle = solutionFilename;

            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            WindowTitle = _originalTitle;

            return VSConstants.S_OK;
        }

        public int OnBeforeBackgroundSolutionLoadBegins() => VSConstants.S_OK;
        public int OnBeforeLoadProjectBatch(bool fIsBackgroundIdleBatch) => VSConstants.S_OK;
        public int OnAfterLoadProjectBatch(bool fIsBackgroundIdleBatch) => VSConstants.S_OK;
        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded) => VSConstants.S_OK;
        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel) => VSConstants.S_OK;
        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved) => VSConstants.S_OK;
        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy) => VSConstants.S_OK;
        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel) => VSConstants.S_OK;
        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy) => VSConstants.S_OK;
        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution) => VSConstants.S_OK;
        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel) => VSConstants.S_OK;
        public int OnBeforeCloseSolution(object pUnkReserved) => VSConstants.S_OK;
        public int OnAfterBackgroundSolutionLoadComplete() => VSConstants.S_OK;
        public int OnQueryBackgroundLoadProjectBatch(out bool pfShouldDelayLoadToNextIdle)
        {
            pfShouldDelayLoadToNextIdle = false;
            return VSConstants.S_OK;
        }
    }
}

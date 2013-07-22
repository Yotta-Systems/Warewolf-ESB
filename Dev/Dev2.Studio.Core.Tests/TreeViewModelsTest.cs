﻿#region

using System;
using System.Linq;
using System.Threading;
using Caliburn.Micro;
using Dev2.Activities;
using Dev2.Communication;
using Dev2.Composition;
using Dev2.Core.Tests.ProperMoqs;
using Dev2.Providers.Errors;
using Dev2.Providers.Events;
using Dev2.Services;
using Dev2.Studio.Core.AppResources.Enums;
using Dev2.Studio.Core.Interfaces;
using Dev2.Studio.Core.Messages;
using Dev2.Studio.Core.Models;
using Dev2.Studio.Core.Network;
using Dev2.Studio.Core.ViewModels.Navigation;
using Dev2.Studio.Core.Wizards.Interfaces;
using Dev2.Studio.Factory;
using Dev2.Studio.ViewModels.Navigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unlimited.Applications.BusinessDesignStudio.Activities;

#endregion

namespace Dev2.Core.Tests
{
    /// <summary>
    ///     Summary description for Base
    /// </summary>
    [TestClass]
    public class TreeViewModelsTest
    {
        TcpConnection _testConnection;

        #region Variables

        Mock<IEventAggregator> _eventAggregator;
        CategoryTreeViewModel _categoryVm;
        CategoryTreeViewModel _categoryVm2;
        EnvironmentTreeViewModel _environmentVm;
        Mock<IEnvironmentModel> _mockEnvironmentModel;
        Mock<IContextualResourceModel> _mockResourceModel;
        Mock<IContextualResourceModel> _mockResourceModel2;
        ResourceTreeViewModel _resourceVm;
        ResourceTreeViewModel _resourceVm2;
        RootTreeViewModel _rootVm;
        ServiceTypeTreeViewModel _serviceTypeVm;
        ServiceTypeTreeViewModel _serviceTypeVm2;

        private static readonly object TestGuard = new object();

        #endregion

        /// <summary>
        ///     Gets or sets the result context which provides
        ///     information about and functionality for the current result run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #region Initialize

        [TestInitialize]
        public void MyTestInitialize()
        {
            Monitor.Enter(TestGuard);

            _eventAggregator = new Mock<IEventAggregator>();
            _eventAggregator.Setup(e => e.Publish(It.IsAny<object>())).Verifiable();

            var securityContext = new Mock<IFrameworkSecurityContext>();
            _testConnection = new TcpConnection(securityContext.Object, new Uri("http://127.0.0.1:77/dsf"), 1234, _eventAggregator.Object);

            ImportService.CurrentContext =
                CompositionInitializer.InializeWithEventAggregator(_eventAggregator.Object);


            _mockEnvironmentModel = new Mock<IEnvironmentModel>();
            _mockEnvironmentModel.SetupGet(x => x.Connection.AppServerUri).Returns(new Uri("http://127.0.0.1/"));
            _mockEnvironmentModel.Setup(e => e.Connection).Returns(_testConnection);

            _mockResourceModel = new Mock<IContextualResourceModel>();
            _mockResourceModel.Setup(r => r.ResourceType).Returns(ResourceType.WorkflowService);
            _mockResourceModel.Setup(r => r.Category).Returns("Testing");
            _mockResourceModel.Setup(r => r.ResourceName).Returns("Mock");
            _mockResourceModel.Setup(r => r.Environment).Returns(_mockEnvironmentModel.Object);

            _mockResourceModel2 = new Mock<IContextualResourceModel>();
            _mockResourceModel2.Setup(r => r.ResourceType).Returns(ResourceType.Service);
            _mockResourceModel2.Setup(r => r.Category).Returns("Testing2");
            _mockResourceModel2.Setup(r => r.ResourceName).Returns("Mock2");
            _mockResourceModel2.Setup(r => r.Environment).Returns(_mockEnvironmentModel.Object);

            _rootVm = TreeViewModelFactory.Create() as RootTreeViewModel;
            _environmentVm = TreeViewModelFactory.Create(_mockEnvironmentModel.Object, _rootVm) as EnvironmentTreeViewModel;
            _serviceTypeVm = TreeViewModelFactory.Create(ResourceType.WorkflowService, _environmentVm) as ServiceTypeTreeViewModel;
            _serviceTypeVm2 = TreeViewModelFactory.Create(ResourceType.Service, _environmentVm) as ServiceTypeTreeViewModel;

            _categoryVm = TreeViewModelFactory.CreateCategory(_mockResourceModel.Object.Category,
                _mockResourceModel.Object.ResourceType, _serviceTypeVm) as CategoryTreeViewModel;

            _categoryVm2 = TreeViewModelFactory.CreateCategory(_mockResourceModel2.Object.Category,
                _mockResourceModel2.Object.ResourceType, _serviceTypeVm2) as CategoryTreeViewModel;

            var validationService = new Mock<IDesignValidationService>();
            _resourceVm = new ResourceTreeViewModel(validationService.Object, _categoryVm, _mockResourceModel.Object, typeof(DsfActivity).AssemblyQualifiedName);
            _resourceVm2 = new ResourceTreeViewModel(validationService.Object, _categoryVm2, _mockResourceModel2.Object, typeof(DsfActivity).AssemblyQualifiedName);
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            Monitor.Exit(TestGuard);
        }

        #endregion

        #region Root

        [TestMethod]
        public void CheckRoot_Expected_AllChildrenChecked()
        {
            _resourceVm.IsChecked = false;
            _resourceVm2.IsChecked = false;
            _rootVm.IsChecked = true;

            Assert.IsTrue(_rootVm.IsChecked == true);
            Assert.IsTrue(_environmentVm.IsChecked == true);
            Assert.IsTrue(_serviceTypeVm.IsChecked == true);
            Assert.IsTrue(_serviceTypeVm2.IsChecked == true);
            Assert.IsTrue(_categoryVm.IsChecked == true);
            Assert.IsTrue(_categoryVm2.IsChecked == true);
            Assert.IsTrue(_resourceVm.IsChecked == true);
            Assert.IsTrue(_resourceVm2.IsChecked == true);
        }

        [TestMethod]
        public void UncheckRoot_Expected_AllChildrenUnChecked()
        {
            _resourceVm.IsChecked = true;
            _resourceVm2.IsChecked = true;
            _rootVm.IsChecked = false;

            Assert.IsTrue(_rootVm.IsChecked == false);
            Assert.IsTrue(_environmentVm.IsChecked == false);
            Assert.IsTrue(_serviceTypeVm.IsChecked == false);
            Assert.IsTrue(_serviceTypeVm2.IsChecked == false);
            Assert.IsTrue(_categoryVm.IsChecked == false);
            Assert.IsTrue(_categoryVm2.IsChecked == false);
            Assert.IsTrue(_resourceVm.IsChecked == false);
            Assert.IsTrue(_resourceVm2.IsChecked == false);
        }

        [TestMethod]
        public void OneCheckedChildAndOneUnCheckedChild_Expected_PartiallyCheckedRoot()
        {
            _resourceVm.IsChecked = true;
            _resourceVm2.IsChecked = false;

            Assert.IsTrue(_rootVm.IsChecked == null);
            Assert.IsTrue(_environmentVm.IsChecked == null);
            Assert.IsTrue(_serviceTypeVm.IsChecked == true);
            Assert.IsTrue(_serviceTypeVm2.IsChecked == false);
            Assert.IsTrue(_categoryVm.IsChecked == true);
            Assert.IsTrue(_categoryVm2.IsChecked == false);
            Assert.IsTrue(_resourceVm.IsChecked == true);
            Assert.IsTrue(_resourceVm2.IsChecked == false);
        }

        [TestMethod]
        public void RootNodeFindChildByEnvironment_Expected_RightEnvironmentNode()
        {
            var environment = _mockEnvironmentModel.Object;
            var child = _rootVm.FindChild(environment);
            Assert.IsTrue(Equals(child, _environmentVm));
        }

        [TestMethod]
        public void RootNodeFindChildByResourceModel_Expected_RightChildNode()
        {
            var child = _rootVm.FindChild(_mockResourceModel.Object);
            Assert.IsTrue(ReferenceEquals(child, _resourceVm));
        }

        [TestMethod]
        public void TestGetChildCountFromRoot_Expected_RecursiveTotal()
        {
            var childCount = _rootVm.ChildrenCount;
            Assert.IsTrue(childCount == 2);
        }

        [TestMethod]
        public void TestGetChildCount_WherePredicateIsNull_Expected_AllChildren()
        {
            var childCount = _rootVm.GetChildren(null).ToList().Count;
            Assert.IsTrue(childCount == 7);
        }

        [TestMethod]
        public void TestGetChildren_Expected_FirstChildMatchingPredicate()
        {
            var child = _rootVm.GetChildren(c => c.DisplayName == "Mock").ToList();
            Assert.IsTrue(child.Count == 1);
            Assert.IsTrue(child.First().DisplayName == "Mock");
        }

        [TestMethod]
        public void TestFiler_Were_FiterIsSet_Expected_CheckedNodesAndParentCategoriesArentFiltered_()
        {
            ITreeNode parent =
                TreeViewModelFactory.CreateCategory("More", ResourceType.WorkflowService, null);

            var checkedNonMatchingNode = new ResourceTreeViewModel(
                new Mock<IDesignValidationService>().Object,
                parent,
                    Dev2MockFactory.SetupResourceModelMock(ResourceType.WorkflowService, "cake").Object,
                typeof(DsfActivity).AssemblyQualifiedName);

            checkedNonMatchingNode.IsChecked = true;

            var nonMatchingNode = new ResourceTreeViewModel(
                new Mock<IDesignValidationService>().Object,
                parent,
                    Dev2MockFactory.SetupResourceModelMock(ResourceType.WorkflowService, "cake1111").Object,
                typeof(DsfActivity).AssemblyQualifiedName);

            var matchingNode = new ResourceTreeViewModel(
                new Mock<IDesignValidationService>().Object,
                parent,
                    Dev2MockFactory.SetupResourceModelMock(ResourceType.WorkflowService, "Match").Object,
                typeof(DsfActivity).AssemblyQualifiedName);

            parent.FilterText = ("Match");

            Assert.IsTrue(nonMatchingNode.IsFiltered);
            Assert.IsFalse(checkedNonMatchingNode.IsFiltered);
            Assert.IsFalse(matchingNode.IsFiltered);
            Assert.IsFalse(parent.IsFiltered);
        }

        [TestMethod]
        public void TestFiler_Were_AllNodesFiltered_Expected_CheckedNodesAndParentCategoriestFiltered_()
        {
            var rootVM = TreeViewModelFactory.Create() as RootTreeViewModel;
            var environmentVM = TreeViewModelFactory.Create(_mockEnvironmentModel.Object, rootVM) as EnvironmentTreeViewModel;
            var serviceTypeVM = TreeViewModelFactory.Create(ResourceType.WorkflowService, environmentVM) as ServiceTypeTreeViewModel;
            var categoryVM = TreeViewModelFactory.CreateCategory(_mockResourceModel.Object.Category,
                _mockResourceModel.Object.ResourceType, serviceTypeVM);
            var resource1 = Dev2MockFactory.SetupResourceModelMock(ResourceType.WorkflowService, "cake1").Object;
            var resource2 = Dev2MockFactory.SetupResourceModelMock(ResourceType.WorkflowService, "cake2").Object;

            var resourceVM1 = new ResourceTreeViewModel(new Mock<IDesignValidationService>().Object, categoryVM, resource1, typeof(DsfActivity).AssemblyQualifiedName);
            var resourceVM2 = new ResourceTreeViewModel(new Mock<IDesignValidationService>().Object, categoryVM, resource2, typeof(DsfActivity).AssemblyQualifiedName);

            resourceVM1.IsChecked = false;
            resourceVM2.IsChecked = false;

            categoryVM.FilterText = ("Match");

            Assert.IsTrue(resourceVM1.IsFiltered);
            Assert.IsTrue(resourceVM2.IsFiltered);
            Assert.IsTrue(categoryVM.IsFiltered);
        }

        [TestMethod]
        public void TestFilter_Were_AllNodesCheckedAndFilter_Expected_CheckedNodesAndParentCategoriestNotFiltered()
        {
            var rootVM = TreeViewModelFactory.Create() as RootTreeViewModel;
            var environmentVM = TreeViewModelFactory.Create(_mockEnvironmentModel.Object, rootVM) as EnvironmentTreeViewModel;
            var serviceTypeVM = TreeViewModelFactory.Create(ResourceType.WorkflowService, environmentVM) as ServiceTypeTreeViewModel;
            var categoryVM = TreeViewModelFactory.CreateCategory(_mockResourceModel.Object.Category,
                _mockResourceModel.Object.ResourceType, serviceTypeVM);
            var resource1 = Dev2MockFactory.SetupResourceModelMock(ResourceType.WorkflowService, "cake1").Object;
            var resource2 = Dev2MockFactory.SetupResourceModelMock(ResourceType.WorkflowService, "cake2").Object;
            var resourceVM1 = new ResourceTreeViewModel(new Mock<IDesignValidationService>().Object, categoryVM, resource1, typeof(DsfActivity).AssemblyQualifiedName);
            var resourceVM2 = new ResourceTreeViewModel(new Mock<IDesignValidationService>().Object, categoryVM, resource2, typeof(DsfActivity).AssemblyQualifiedName);

            resourceVM1.IsChecked = false;
            resourceVM2.IsChecked = false;

            categoryVM.FilterText = "cake";

            Assert.IsFalse(resourceVM1.IsFiltered);
            Assert.IsFalse(resourceVM2.IsFiltered);
            Assert.IsFalse(categoryVM.IsFiltered);
        }

        #endregion Root

        #region Environment

        [TestMethod]
        public void EnvironmentNodeCanConnectWhenCanStudioExecuteFalseAndConnected()
        {
            _mockEnvironmentModel.Setup(e => e.CanStudioExecute).Returns(false);
            _mockEnvironmentModel.Setup(e => e.IsConnected).Returns(true);
            Assert.IsTrue(_environmentVm.CanConnect);
        }

        [TestMethod]
        public void EnvironmentNodeCanConnectWhenNotConnected()
        {
            _mockEnvironmentModel.Setup(e => e.IsConnected).Returns(false);
            Assert.IsTrue(_environmentVm.CanConnect);
        }

        [TestMethod]
        public void EnvironmentNodeConnectCommadsSetsCanStudioExecute()
        {
            //------Test Setup---------
            _mockEnvironmentModel.Setup(e => e.IsConnected).Returns(true);
            _mockEnvironmentModel.Setup(e => e.CanStudioExecute)
                .Returns(false)
                .Verifiable();
            _mockEnvironmentModel.Setup(e => e.ForceLoadResources()).Verifiable();
            _mockEnvironmentModel.Setup(e => e.Connect()).Verifiable();
            _mockEnvironmentModel.Setup(e => e.IsConnected).Returns(false);
            var navigationVM = new Mock<INavigationContext>();
            navigationVM.Setup(vm => vm.Update(It.IsAny<IEnvironmentModel>())).Verifiable();
            _rootVm.Parent = navigationVM.Object;

            //------Test Execution---------
            _environmentVm.ConnectCommand.Execute(null);

            //------Assertion--------
            _mockEnvironmentModel.Verify(e => e.ForceLoadResources(), Times.Once());
            _mockEnvironmentModel.Verify(e => e.Connect(), Times.Once());
            navigationVM.Verify(vm => vm.Update(It.IsAny<IEnvironmentModel>()), Times.Once());

        }

        [TestMethod]
        public void EnvironmentNodeExpectHasNewResourceCommand()
        {
            Assert.IsNotNull(_environmentVm.NewResourceCommand);
        }

        [TestMethod]
        public void EnvironmentNodeFindChildByResourceType_Expected_RightServiceTypeNode()
        {
            var child = _environmentVm.FindChild(ResourceType.WorkflowService);
            Assert.IsTrue(ReferenceEquals(child, _serviceTypeVm));
        }

        [TestMethod]
        public void EnvironmentNodeCantDisconnectLocalHost()
        {
            _mockEnvironmentModel.SetupGet(c => c.IsConnected).Returns(true);
            _mockEnvironmentModel.SetupGet(c => c.Connection).Returns(_testConnection);
            _mockEnvironmentModel.SetupGet(c => c.Name).Returns(StringResources.DefaultEnvironmentName);
            Assert.IsTrue(_environmentVm.CanDisconnect == false);
        }

        [TestMethod]
        public void EnvironmentNodeCanDisconnectOtherHosts()
        {
            _mockEnvironmentModel.SetupGet(c => c.IsConnected).Returns(true);
            _mockEnvironmentModel.SetupGet(c => c.Connection).Returns(_testConnection);
            _mockEnvironmentModel.SetupGet(c => c.Name).Returns("Mock");
            Assert.IsTrue(_environmentVm.CanDisconnect == true);
        }

        [TestMethod]
        public void EnvironmentNodeConnectCommand_Expected_EnvironmentModelConnectMethodExecuted()
        {
            _mockEnvironmentModel.Setup(c => c.Connect()).Verifiable();
            _mockEnvironmentModel.SetupGet(c => c.IsConnected).Returns(false);

            var navigationVM = new Mock<INavigationContext>();
            navigationVM.Setup(vm => vm.Update(It.IsAny<IEnvironmentModel>())).Verifiable();
            _rootVm.Parent = navigationVM.Object;

            var cmd = _environmentVm.ConnectCommand;
            cmd.Execute(null);

            _mockEnvironmentModel.Verify(c => c.Connect(), Times.Once());
        }

        [TestMethod]
        public void EnvironmentNodeDisconnectCommand_Expected_EnvironmentModelDisconnectMethodExecuted()
        {
            _mockEnvironmentModel.Setup(c => c.Disconnect()).Verifiable();
            _mockEnvironmentModel.SetupGet(c => c.Connection).Returns(_testConnection);
            _mockEnvironmentModel.SetupGet(c => c.IsConnected).Returns(true);
            var cmd = _environmentVm.DisconnectCommand;
            cmd.Execute(null);

            _mockEnvironmentModel.Verify(c => c.Disconnect(), Times.Once());
        }

        [TestMethod]
        public void EnvironmentNodeRemoveCommand_Expected_MediatorRemoveServerFromExplorerMessage()
        {
            _environmentVm.RemoveCommand.Execute(null);
            _eventAggregator.Verify(e => e.Publish(It.IsAny<RemoveEnvironmentMessage>()), Times.Once());
        }

        [TestMethod]
        public void EnvironmentNodeISNotIHandleUpdateActiveEnvironmentMessage()
        {           
            //Do not select the active environment in the tree,
            Assert.IsNotInstanceOfType(_environmentVm, typeof(IHandle<UpdateActiveEnvironmentMessage>));
        }



        #endregion Environment

        #region ServicType

        [TestMethod]
        public void ServiceNodeEnvironmentModel_Expect_ParentEnvironmentModel()
        {
            var model = _serviceTypeVm.EnvironmentModel;
            var model2 = _environmentVm.EnvironmentModel;
            Assert.IsTrue(ReferenceEquals(model, model2));
        }

        [TestMethod]
        public void ServiceTypeNodeFindChildByString_Expected_RightCategoryNode()
        {
            var child = _serviceTypeVm.FindChild("Testing");
            Assert.IsTrue(ReferenceEquals(child, _categoryVm));
        }

        [TestMethod]
        public void TestGetChildCountFromService_Expected_RecursiveTotal()
        {
            var childCount = _serviceTypeVm.ChildrenCount;
            Assert.IsTrue(childCount == 1);
        }

        #endregion ServiceType

        #region Category

        [TestMethod]
        public void AddChildExpectChildAdded()
        {
            var mockResource3 = new Mock<IContextualResourceModel>();
            mockResource3.Setup(r => r.ResourceType).Returns(ResourceType.Service);
            mockResource3.Setup(r => r.Category).Returns("Testing3");
            mockResource3.Setup(r => r.ResourceName).Returns("Mock3");

            var count = _categoryVm2.ChildrenCount;

            var toAdd = new ResourceTreeViewModel(new Mock<IDesignValidationService>().Object, null, mockResource3.Object, typeof(DsfActivity).AssemblyQualifiedName);

            _categoryVm2.Add(toAdd);

            Assert.IsTrue(_categoryVm2.ChildrenCount == count + 1);
            Assert.IsTrue(ReferenceEquals(toAdd.TreeParent, _categoryVm2));
        }

        [TestMethod]
        public void RemoveChildExpectChildRemoved()
        {
            var count = _categoryVm2.ChildrenCount;
            var toRemove = _categoryVm2.GetChildren(c => true).FirstOrDefault();
            _categoryVm2.Remove(toRemove);
            Assert.IsTrue(_categoryVm2.ChildrenCount == count - 1);
            Assert.IsTrue(toRemove.TreeParent == null);
        }

        [TestMethod]
        public void CategoryNodeEnvironmentModel_Expect_ParentEnvironmentModel()
        {
            var model = _categoryVm.EnvironmentModel;
            var model2 = _environmentVm.EnvironmentModel;
            Assert.IsTrue(ReferenceEquals(model, model2));
        }

        [TestMethod]
        public void CategoryNodeFindChildByName_Expected_RightChildNode()
        {
            var child = _categoryVm.FindChild("Mock");
            Assert.IsTrue(ReferenceEquals(child, _resourceVm));
        }

        [TestMethod]
        public void TestGetChildCountFromCategory_Expected_RecursiveTotal()
        {
            var childCount = _categoryVm.ChildrenCount;
            Assert.IsTrue(childCount == 1);
        }    
        #endregion Category

        #region Resource

        [TestMethod]
        public void TestGetChildCountFromResource_Expected_CountOfOne()
        {
            var childCount = _resourceVm.ChildrenCount;
            Assert.IsTrue(childCount == 1);
        }

        [TestMethod]
        public void ResourceNodeEnvironmentModel_Expect_ParentEnvironmentModel()
        {
            var model = _resourceVm.EnvironmentModel;
            var model2 = _environmentVm.EnvironmentModel;
            Assert.IsTrue(ReferenceEquals(model, model2));
        }

        [TestMethod]
        public void ResourceNodeDebugCommand_Expected_MediatorDebugResourceMessage()
        {
            //Mediator.RegisterToReceiveMessage(MediatorMessages.DebugResource, o => messageRecieved = true);
            _resourceVm.DebugCommand.Execute(null);
            _eventAggregator.Verify(e => e.Publish(It.Is<DebugResourceMessage>
              (t => t.Resource == _mockResourceModel.Object)), Times.Once());
        }

        [TestMethod]
        public void ResourceNodeDeleteCommand_With_Expected_EventAggregatorDeleteResourceMessage()
        {
            _resourceVm.DeleteCommand.Execute(null);

            _eventAggregator.Verify(e => e.Publish(It.Is<DeleteResourceMessage>
                (t => t.ResourceModel == _mockResourceModel.Object)), Times.Once());
        }

        [TestMethod]
        public void ResourceNodeCreateWizardCommand_Expected_WizardEngineCreateResourceMEthodExecuted()
        {
            var mockWizardEngine = new Mock<IWizardEngine>();
            mockWizardEngine.Setup(e => e.CreateResourceWizard(_mockResourceModel.Object))
                            .Callback<object>(o =>
                            {
                                var resource = (IContextualResourceModel)o;
                                Assert.IsTrue(ReferenceEquals(resource, _mockResourceModel.Object));
                            }).Verifiable();

            var newResourceVM = new ResourceTreeViewModel(new Mock<IDesignValidationService>().Object, null, _mockResourceModel.Object, typeof(DsfActivity).AssemblyQualifiedName);

            newResourceVM.WizardEngine = mockWizardEngine.Object;

            newResourceVM.CreateWizardCommand.Execute(null);
            mockWizardEngine.Verify(e => e.CreateResourceWizard(It.IsAny<IContextualResourceModel>()), Times.Once());
        }

        [TestMethod]
        public void ResourceNodeManualEditCommand_With_Source_Expected_MediatorShowEditResourceWizardMessage()
        {
            //Mediator.RegisterToReceiveMessage(MediatorMessages.ShowEditResourceWizard, o => messageRecieved = true);
            _mockResourceModel.SetupGet(m => m.ResourceType).Returns(ResourceType.Source);
            _resourceVm.ManualEditCommand.Execute(null);
            _eventAggregator.Verify(e => e.Publish(It.Is<ShowEditResourceWizardMessage>
             (t => t.ResourceModel == _mockResourceModel.Object)), Times.Once());
        }

        [TestMethod]
        public void ResourceNodeManualEditCommand_With_WorkflowService_Expected_MediatorAddWorkflowDesignerMessage()
        {
            //Mediator.RegisterToReceiveMessage(MediatorMessages.AddWorkflowDesigner, o => messageRecieved = true);
            _mockResourceModel.SetupGet(m => m.ResourceType).Returns(ResourceType.WorkflowService);
            _resourceVm.ManualEditCommand.Execute(null);
            _eventAggregator.Verify(e => e.Publish(It.Is<AddWorkSurfaceMessage>
            (t => t.WorkSurfaceObject == _mockResourceModel.Object)), Times.Once());
        }

        [TestMethod]
        public void ResourceNodeManualEditCommand_With_Service_Expected_MediatorShowEditResourceWizardMessage()
        {
            //Mediator.RegisterToReceiveMessage(MediatorMessages.ShowEditResourceWizard, o => messageRecieved = true);
            _mockResourceModel.SetupGet(m => m.ResourceType).Returns(ResourceType.Service);
            _resourceVm.ManualEditCommand.Execute(null);
            _eventAggregator.Verify(e => e.Publish(It.Is<ShowEditResourceWizardMessage>
                    (t => t.ResourceModel == _mockResourceModel.Object)), Times.Once());
        }

        [TestMethod]
        public void ResourceNodeEditCommand_With_ResourceWizard_Expected_WizardEngineEditWizardExecuted()
        {
            var mockWizardEngine = new Mock<IWizardEngine>();
            mockWizardEngine.Setup(c => c.IsResourceWizard(It.IsAny<IContextualResourceModel>()))
                            .Returns(true);
            mockWizardEngine.Setup(e => e.EditWizard(_mockResourceModel.Object))
                            .Callback<object>(o =>
                            {
                                var resource = (IContextualResourceModel)o;
                                Assert.IsTrue(ReferenceEquals(resource, _mockResourceModel.Object));
                            }).Verifiable();

            var newResourceVM = new WizardTreeViewModel(new Mock<IDesignValidationService>().Object, null, _mockResourceModel.Object, typeof(DsfActivity).AssemblyQualifiedName);

            newResourceVM.WizardEngine = mockWizardEngine.Object;

            newResourceVM.EditCommand.Execute(null);
            mockWizardEngine.Verify(e => e.EditWizard(It.IsAny<IContextualResourceModel>()), Times.Once());
        }

        [TestMethod]
        public void ResourceNodeEditCommand_With_Source_Expected_MediatorShowEditResourceWizardMessage()
        {
            //Mediator.RegisterToReceiveMessage(MediatorMessages.ShowEditResourceWizard, o => messageRecieved = true);
            _mockResourceModel.SetupGet(m => m.ResourceType).Returns(ResourceType.Source);
            _resourceVm.EditCommand.Execute(null);
            _eventAggregator.Verify(e => e.Publish(It.Is<ShowEditResourceWizardMessage>
                   (t => t.ResourceModel == _mockResourceModel.Object)), Times.Once());
        }

        [TestMethod]
        public void ResourceNodeEditCommand_With_WorkflowService_Expected_MediatorAddWorkflowDesignerMessage()
        {
            //Mediator.RegisterToReceiveMessage(MediatorMessages.AddWorkflowDesigner, o => messageRecieved = true);
            _mockResourceModel.SetupGet(m => m.ResourceType).Returns(ResourceType.WorkflowService);
            _resourceVm.EditCommand.Execute(null);
            _eventAggregator.Verify(e => e.Publish(It.Is<AddWorkSurfaceMessage>
                  (t => t.WorkSurfaceObject == _mockResourceModel.Object)), Times.Once());
        }

        [TestMethod]
        public void ResourceNodeEditCommand_With_Service_Expected_MediatorShowEditResourceWizardMessage()
        {
            //Mediator.RegisterToReceiveMessage(MediatorMessages.ShowEditResourceWizard, o => messageRecieved = true);
            _mockResourceModel.SetupGet(m => m.ResourceType).Returns(ResourceType.Service);
            _resourceVm.EditCommand.Execute(null);
            _eventAggregator.Verify(e => e.Publish(It.Is<ShowEditResourceWizardMessage>
                 (t => t.ResourceModel == _mockResourceModel.Object)), Times.Once());
        }

        [TestMethod]
        public void ResourceNodeShowPropertiesCommand_Expected_MediatorShowEditResourceWizardMessage()
        {
            //Mediator.RegisterToReceiveMessage(MediatorMessages.ShowEditResourceWizard, o => messageRecieved = true);
            _mockResourceModel.SetupGet(m => m.ResourceType).Returns(ResourceType.Service);
            _resourceVm.ShowPropertiesCommand.Execute(null);
            _eventAggregator.Verify(e => e.Publish(It.Is<ShowEditResourceWizardMessage>
                 (t => t.ResourceModel == _mockResourceModel.Object)), Times.Once());

            _mockResourceModel.SetupGet(m => m.ResourceType).Returns(ResourceType.WorkflowService);
            _resourceVm.ShowPropertiesCommand.Execute(null);

            _eventAggregator.Verify(e => e.Publish(It.Is<ShowEditResourceWizardMessage>
                 (t => t.ResourceModel == _mockResourceModel.Object)), Times.AtLeastOnce());
            _mockResourceModel.SetupGet(m => m.ResourceType).Returns(ResourceType.Source);
            _resourceVm.ShowPropertiesCommand.Execute(null);

            _eventAggregator.Verify(e => e.Publish(It.Is<ShowEditResourceWizardMessage>
                 (t => t.ResourceModel == _mockResourceModel.Object)), Times.AtLeastOnce());
        }

        [TestMethod]
        public void ResourceNodeShowDependenciesCommand_Expected_EventAggregatorShowDependencyGraphMessage()
        {
            _resourceVm.ShowDependenciesCommand.Execute(null);

            _eventAggregator.Verify(e => e.Publish(It.Is<ShowDependenciesMessage>
                (t => t.ResourceModel == _mockResourceModel.Object)), Times.Once());
        }

        [TestMethod]
        public void ResourceNodeBuildCommand_Expected_EditCommandExecuted_And_MediatorSaveResourceMessage()
        {
            _resourceVm.BuildCommand.Execute(null);
            _eventAggregator.Verify(e => e.Publish(It.Is<SaveResourceMessage>
                 (t => t.Resource == _mockResourceModel.Object)), Times.Once());
        }

        [TestMethod]
        public void EditCommand_GivenAResourceOfTypeWorkflowService_Expected_AddWorkflowDesignerMediatorMessage()
        {
            _mockResourceModel.Setup(r => r.ResourceType).Returns(ResourceType.WorkflowService);
            _resourceVm.EditCommand.Execute(_mockResourceModel);
            _eventAggregator.Verify(e => e.Publish(It.Is<AddWorkSurfaceMessage>
                (t => t.WorkSurfaceObject == _mockResourceModel.Object)), Times.Once());
        }


        [TestMethod]
        public void EditCommand_GivenAResourceOfTypeSource_Expected_ShowEditResourceWizardMediatorMessage()
        {
            _mockResourceModel.Setup(r => r.ResourceType).Returns(ResourceType.Source);
            _resourceVm.EditCommand.Execute(_mockResourceModel);
            _eventAggregator.Verify(e => e.Publish(It.Is<ShowEditResourceWizardMessage>
                (t => t.ResourceModel == _mockResourceModel.Object)), Times.Once());
        }

        [TestMethod]
        public void EditCommand_GivenAResourceOfTypeService_Expected_ShowEditResourceWizardMediatorMessage()
        {
            _mockResourceModel.Setup(r => r.ResourceType).Returns(ResourceType.Service);
            _resourceVm.EditCommand.Execute(_mockResourceModel);
            _eventAggregator.Verify(e => e.Publish(It.Is<ShowEditResourceWizardMessage>
                (t => t.ResourceModel == _mockResourceModel.Object)), Times.Once());
        }

        //
        //Juries - Bug 8427
        //
        [TestMethod]
        public void ResourceParentCheckedDoesNotCheckFilteredChildren()
        {
            var mockResource3 = new Mock<IContextualResourceModel>();
            mockResource3.Setup(r => r.ResourceType).Returns(ResourceType.Service);
            mockResource3.Setup(r => r.Category).Returns("Testing3");
            mockResource3.Setup(r => r.ResourceName).Returns("Mock3");
            var toAdd = new ResourceTreeViewModel(new Mock<IDesignValidationService>().Object, _categoryVm2, mockResource3.Object, typeof(DsfActivity).AssemblyQualifiedName);

            _categoryVm2.IsChecked = true;
            Assert.IsTrue(_categoryVm2.Children.Count(c => c.IsChecked == true) == 2);

            _categoryVm2.IsChecked = false;
            _rootVm.FilterText = "Mock3";
            _categoryVm2.IsChecked = true;

            Assert.IsTrue(_categoryVm2.Children.Count(c => c.IsChecked == true) == 1);
        }

        //
        //Juries - Bug 8427
        //
        [TestMethod]
        public void FilterChangedResultingInItemNotFilteredUpdatesParentState()
        {
            var mockResource3 = new Mock<IContextualResourceModel>();
            mockResource3.Setup(r => r.ResourceType).Returns(ResourceType.Service);
            mockResource3.Setup(r => r.Category).Returns("Testing2");
            mockResource3.Setup(r => r.ResourceName).Returns("Mock3");
            var toAdd = new ResourceTreeViewModel(new Mock<IDesignValidationService>().Object, _categoryVm2, mockResource3.Object, typeof(DsfActivity).AssemblyQualifiedName);

            Thread.Sleep(100);

            Assert.IsTrue(_categoryVm2.ChildrenCount == 2);

            toAdd.IsChecked = true;
            _rootVm.FilterText = "Mock3";
            _rootVm.NotifyOfFilterPropertyChanged(false);

            Thread.Sleep(100);

            Assert.IsTrue(_categoryVm2.IsChecked == true);

            _rootVm.FilterText = "";
            _rootVm.NotifyOfFilterPropertyChanged(false);

            Thread.Sleep(100);

            Assert.IsTrue(_categoryVm2.IsChecked == null);
            //Assert.Inconclusive("This test is flawed because it's asserts rely on work which is done on a seperate thread to the callin one. This needs to be addressed as part of 9128.");
        }

        [TestMethod]
        [TestCategory("ResourceTreeViewModelUnitTest")]
        [Description("Test for implementation of 'IDataErrorInfo': The IDataErrorInfo error message is set and then recovered from a resource tree view model")]
        [Owner("Ashley")]
        // ReSharper disable InconsistentNaming
        public void ResourceTreeViewModel_ResourceTreeViewModelUnitTest_IDataErrorInfoImplimentation_ReturnsItsErrorMessage()
        // ReSharper restore InconsistentNaming
        {
            //init
            var vm = new ResourceTreeViewModel(new Mock<IDesignValidationService>().Object, _categoryVm, new Mock<IContextualResourceModel>().Object, typeof(DsfActivity).AssemblyQualifiedName);
            var testError = new ErrorInfo { Message = "Test IDataErrorInfo Message" };
            vm.DataContext = new ResourceModel(ResourceModelTest.CreateMockEnvironment().Object);
            vm.DataContext.AddError(testError);

            var displayName = vm["DisplayName"];
            //assert on execute
            Assert.AreEqual("Test IDataErrorInfo Message", vm["DisplayName"], "ResourceTreeViewModel was not able to return its error message correctly");
        }

        [TestMethod]
        [TestCategory("ResourceTreeViewModelUnitTest")]
        [Description("Test for implementation of 'OnDesignValidationReceived': OnDesignValidationReceived is called and a new error message is expected to be added to data context")]
        [Owner("Ashley")]
        // ReSharper disable InconsistentNaming
        public void ResourceTreeViewModel_ResourceTreeViewModelUnitTest_OnDesignValidationReceived_ErrorNotAddedToDataContext()
        // ReSharper restore InconsistentNaming
        {
            //init
            var validator = new Mock<IDesignValidationService>();
            var vm = new MockResourceTreeViewModel(validator.Object, _categoryVm, new Mock<IContextualResourceModel>().Object, typeof(DsfActivity).AssemblyQualifiedName) { DataContext = new TestResourceModel() };
            var memo = new DesignValidationMemo();
            var testError = new ErrorInfo { Message = "Test Error Message" };
            memo.Errors.Add(testError);
            vm.TestDesignValidationReceived(memo);

            //assert on execute
            Assert.AreEqual(0, vm.DataContext.Errors.Count, "OnDesignValidationReceived added error to datacontext");
        }

        #endregion Resource

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Description("Constructor must throw exception if DataContext parameter is null.")]
        [TestCategory("UnitTest")]
        [Owner("Trevor Williams-Ros")]
        // ReSharper disable InconsistentNaming
        public void ResourceTreeViewModelConstructor_UnitTest_NullDataContext_ThrowsException()
        // ReSharper restore InconsistentNaming
        {
            var validationService = new Mock<IDesignValidationService>();

            var tvm = new ResourceTreeViewModel(validationService.Object, null, null);
        }
        [ExpectedException(typeof(ArgumentNullException))]
        [Description("Constructor must throw exception if ValidatonService parameter is null.")]
        [TestCategory("UnitTest")]
        [Owner("Trevor Williams-Ros")]
        // ReSharper disable InconsistentNaming
        public void ResourceTreeViewModelConstructor_UnitTest_NullValidatonService_ThrowsException()
        // ReSharper restore InconsistentNaming
        {
            var tvm = new ResourceTreeViewModel(null, null, new Mock<IContextualResourceModel>().Object);
        }

        [TestMethod]
        [TestCategory("TreeViewModelFactory_Create")]
        [Description("TreeViewModelFactory must assign a AssemblyQualifiedName based on the ServerResourceType to the ResourceTreeViewModel.")]
        [Owner("Trevor Williams-Ros")]
        public void TreeViewModelFactory_UnitTest_AssemblyQualifiedNameMatchesServerResourceType_DbService()
        {
            var mockEventPublisher = new Mock<IEventPublisher>();
            mockEventPublisher.Setup(publisher => publisher.GetEvent<DesignValidationMemo>()).Returns(new Mock<IObservable<DesignValidationMemo>>().Object);
            var mockEnvironmenConnection = new Mock<IEnvironmentConnection>();
            mockEnvironmenConnection.Setup(connection => connection.ServerEvents).Returns(mockEventPublisher.Object);
            var mockEnvironment = new Mock<IEnvironmentModel>();
            mockEnvironment.Setup(model => model.Connection).Returns(mockEnvironmenConnection.Object);
            var dbServiceModel = new Mock<IContextualResourceModel>();
            dbServiceModel.Setup(m => m.ResourceType).Returns(ResourceType.Service);
            dbServiceModel.Setup(m => m.ServerResourceType).Returns("DbService");
            dbServiceModel.Setup(m => m.Environment).Returns(mockEnvironment.Object);
            dbServiceModel.Setup(m => m.ID).Returns(Guid.NewGuid);

            var dbServiceTvm = (ResourceTreeViewModel)TreeViewModelFactory.Create(dbServiceModel.Object, null, false);
            Assert.AreEqual(typeof(DsfDatabaseActivity).AssemblyQualifiedName, dbServiceTvm.ActivityFullName, "TreeViewModelFactory.Create did not assign database activity type correctly");

            var otherModel = new Mock<IContextualResourceModel>();
            otherModel.Setup(m => m.ResourceType).Returns(ResourceType.WorkflowService);
            otherModel.Setup(m => m.ServerResourceType).Returns("xxx");
            otherModel.Setup(m => m.Environment).Returns(mockEnvironment.Object);
            otherModel.Setup(m => m.ID).Returns(Guid.NewGuid);

            var otherTvm = (ResourceTreeViewModel)TreeViewModelFactory.Create(otherModel.Object, null, false);
            Assert.AreEqual(typeof(DsfActivity).AssemblyQualifiedName, otherTvm.ActivityFullName, "TreeViewModelFactory.Create did not assign DSF activity type correctly");

        } 
        
        [TestMethod]
        [TestCategory("TreeViewModelFactory_Create")]
        [Description("TreeViewModelFactory must assign a AssemblyQualifiedName based on the ServerResourceType to the ResourceTreeViewModel.")]
        [Owner("Huggs")]
        public void TreeViewModelFactory_UnitTest_AssemblyQualifiedNameMatchesServerResourceType_PluginService()
        {
            var mockEventPublisher = new Mock<IEventPublisher>();
            mockEventPublisher.Setup(publisher => publisher.GetEvent<DesignValidationMemo>()).Returns(new Mock<IObservable<DesignValidationMemo>>().Object);
            var mockEnvironmenConnection = new Mock<IEnvironmentConnection>();
            mockEnvironmenConnection.Setup(connection => connection.ServerEvents).Returns(mockEventPublisher.Object);
            var mockEnvironment = new Mock<IEnvironmentModel>();
            mockEnvironment.Setup(model => model.Connection).Returns(mockEnvironmenConnection.Object);
            var dbServiceModel = new Mock<IContextualResourceModel>();
            dbServiceModel.Setup(m => m.ResourceType).Returns(ResourceType.Service);
            dbServiceModel.Setup(m => m.ServerResourceType).Returns("PluginService");
            dbServiceModel.Setup(m => m.Environment).Returns(mockEnvironment.Object);
            dbServiceModel.Setup(m => m.ID).Returns(Guid.NewGuid);

            var dbServiceTvm = (ResourceTreeViewModel)TreeViewModelFactory.Create(dbServiceModel.Object, null, false);
            Assert.AreEqual(typeof(DsfPluginActivity).AssemblyQualifiedName, dbServiceTvm.ActivityFullName, "TreeViewModelFactory.Create did not assign database activity type correctly");

            var otherModel = new Mock<IContextualResourceModel>();
            otherModel.Setup(m => m.ResourceType).Returns(ResourceType.WorkflowService);
            otherModel.Setup(m => m.ServerResourceType).Returns("xxx");
            otherModel.Setup(m => m.Environment).Returns(mockEnvironment.Object);
            otherModel.Setup(m => m.ID).Returns(Guid.NewGuid);

            var otherTvm = (ResourceTreeViewModel)TreeViewModelFactory.Create(otherModel.Object, null, false);
            Assert.AreEqual(typeof(DsfActivity).AssemblyQualifiedName, otherTvm.ActivityFullName, "TreeViewModelFactory.Create did not assign DSF activity type correctly");

        }
    }
}
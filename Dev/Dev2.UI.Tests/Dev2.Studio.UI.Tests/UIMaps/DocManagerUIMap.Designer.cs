﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by coded UI test builder.
//      Version: 11.0.0.0
//
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------

namespace Dev2.CodedUI.Tests.UIMaps.DocManagerUIMapClasses
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text.RegularExpressions;
    using System.Windows.Input;
    using Microsoft.VisualStudio.TestTools.UITest.Extension;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
    using Mouse = Microsoft.VisualStudio.TestTools.UITesting.Mouse;
    using MouseButtons = System.Windows.Forms.MouseButtons;
    
    
    [GeneratedCode("Coded UITest Builder", "11.0.50727.1")]
    public partial class DocManagerUIMap
    {
        
        /// <summary>
        /// ClickExplorer
        /// </summary>
        public static WpfTabPage FindTabPage(string tabName)
        {
            UIBusinessDesignStudioWindow theWindow = new UIBusinessDesignStudioWindow();
            UIPART_UnpinnedTabAreaTabList theTabList = new UIPART_UnpinnedTabAreaTabList(theWindow);
            //UIPART_UnpinnedTabAreaTabList 
            #region Variable Declarations
            WpfTabPage uIGenericTabPage = new WpfTabPage(theTabList);
            uIGenericTabPage.SearchProperties["Name"] = tabName;
            return uIGenericTabPage;
            #endregion
        }
        
        #region Properties
        public UIBusinessDesignStudioWindow UIBusinessDesignStudioWindow
        {
            get
            {
                if ((this.mUIBusinessDesignStudioWindow == null))
                {
                    this.mUIBusinessDesignStudioWindow = new UIBusinessDesignStudioWindow();
                }
                return this.mUIBusinessDesignStudioWindow;
            }
        }
        #endregion
        
        #region Fields
        private UIBusinessDesignStudioWindow mUIBusinessDesignStudioWindow;
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "11.0.50727.1")]
    public class UIBusinessDesignStudioWindow : WpfWindow
    {
        
        public UIBusinessDesignStudioWindow()
        {
            #region Search Criteria

            this.SearchProperties.Add(new PropertyExpression(WpfWindow.PropertyNames.ClassName, "HwndWrapper", PropertyExpressionOperator.Contains));
            this.SearchProperties.Add(new PropertyExpression(WpfWindow.PropertyNames.Name, "Warewolf", PropertyExpressionOperator.Contains));

            #endregion
        }
        
        #region Properties
        public UIDockManagerCustom UIDockManagerCustom
        {
            get
            {
                if ((this.mUIDockManagerCustom == null))
                {
                    this.mUIDockManagerCustom = new UIDockManagerCustom(this);
                }
                return this.mUIDockManagerCustom;
            }
        }
        #endregion
        
        #region Fields
        private UIDockManagerCustom mUIDockManagerCustom;
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "11.0.50727.1")]
    public class UIDockManagerCustom : WpfCustom
    {
        
        public UIDockManagerCustom(UITestControl searchLimitContainer) : 
                base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.XamDockManager";
            this.SearchProperties["AutomationId"] = "dockManager";
            this.SearchProperties.Add(new PropertyExpression(WpfWindow.PropertyNames.Name, "Warewolf", PropertyExpressionOperator.Contains));
            #endregion
        }
        
        #region Properties
        public UIPART_UnpinnedTabAreaTabList UIPART_UnpinnedTabAreaTabList
        {
            get
            {
                if ((this.mUIPART_UnpinnedTabAreaTabList == null))
                {
                    this.mUIPART_UnpinnedTabAreaTabList = new UIPART_UnpinnedTabAreaTabList(this);
                }
                return this.mUIPART_UnpinnedTabAreaTabList;
            }
        }
        #endregion
        
        #region Fields
        private UIPART_UnpinnedTabAreaTabList mUIPART_UnpinnedTabAreaTabList;
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "11.0.50727.1")]
    public class UIPART_UnpinnedTabAreaTabList : WpfTabList
    {
        
        public UIPART_UnpinnedTabAreaTabList(UITestControl searchLimitContainer) : 
                base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties[WpfTabList.PropertyNames.AutomationId] = "PART_UnpinnedTabAreaLeft";
            this.SearchProperties.Add(new PropertyExpression(WpfWindow.PropertyNames.Name, "Warewolf", PropertyExpressionOperator.Contains));
            #endregion
        }
        
        #region Properties
        public WpfTabPage UIGenericTabPage(string tabName)
        {
            
            {
                //if ((this.mUIExplorerTabPage == null))
                {
                    this.mUIExplorerTabPage = new WpfTabPage(this);
                    #region Search Criteria
                    this.mUIExplorerTabPage.SearchProperties[WpfTabPage.PropertyNames.Name] = tabName;
                    this.mUIExplorerTabPage.WindowTitles.Add("Business Design Studio (DEV2\\" + Environment.UserName + ")");
                    #endregion
                }
                return this.mUIExplorerTabPage;
            }
        }
        #endregion
        
        #region Fields
        private WpfTabPage mUIExplorerTabPage;
        #endregion
    }
}

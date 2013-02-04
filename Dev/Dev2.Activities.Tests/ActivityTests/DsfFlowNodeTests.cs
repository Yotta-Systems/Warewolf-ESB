﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ActivityUnitTests.ActivityTests
{
    /// <summary>
    /// Summary description for DateTimeDifferenceTests
    /// </summary>
    [TestClass]
    public class DsfFlowNodeTests : BaseActivityUnitTest
    {
        public DsfFlowNodeTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        #region GetDebugInputs/Outputs

        #region Decision tests
        [TestMethod]
        // ReSharper disable InconsistentNaming
        //Bug 8104
        public void FileRead_Get_Debug_Input_Output_With_Scalar_Expected_Pass()
        // ReSharper restore InconsistentNaming
        {
            //DsfFlowDecisionActivity act = new DsfFlowDecisionActivity();
            //Dev2DecisionStack dds = new Dev2DecisionStack() { TheStack = new List<Dev2Decision>(), Mode = Dev2DecisionMode.OR };

            //dds.AddModelItem(new Dev2Decision() { Col1 = "[[A]]", Col2 = string.Empty, Col3 = string.Empty, EvaluationFn = enDecisionType.IsNumeric });
            //dds.AddModelItem(new Dev2Decision() { Col1 = "[[B]]", Col2 = string.Empty, Col3 = string.Empty, EvaluationFn = enDecisionType.IsNotNumeric });


            //act.ExpressionText = "";


            //IList<IDebugItem> inRes;
            //IList<IDebugItem> outRes;

            //CheckPathOperationActivityDebugInputOutput(act, ActivityStrings.DebugDataListShape,
            //                                                    ActivityStrings.DebugDataListWithData, out inRes, out outRes);

            //Assert.AreEqual(4, inRes.Count);
            //Assert.AreEqual(4, inRes[0].Count);
            //Assert.AreEqual(1, inRes[1].Count);
            //Assert.AreEqual(1, inRes[2].Count);

            //Assert.AreEqual(1, outRes.Count);
            //Assert.AreEqual(3, outRes[0].Count);

            Assert.Inconclusive();
        }

        #endregion

        #region Switch Tests

        [TestMethod]
        // ReSharper disable InconsistentNaming
        //Bug 8104
        public void FileRead_Get_Debug_Input_Output_With_Recordset_Using_Star_Notation_Expected_Pass()
        // ReSharper restore InconsistentNaming
        {
            //DsfFileRead act = new DsfFileRead { InputPath = "[[Numeric(*).num]]", Result = "[[CompanyName]]" };
            //IList<IDebugItem> inRes;
            //IList<IDebugItem> outRes;

            //CheckPathOperationActivityDebugInputOutput(act, ActivityStrings.DebugDataListShape,
            //                                                    ActivityStrings.DebugDataListWithData, out inRes, out outRes);


            //Assert.AreEqual(4, inRes.Count);
            //Assert.AreEqual(31, inRes[0].Count);
            //Assert.AreEqual(1, inRes[1].Count);
            //Assert.AreEqual(1, inRes[2].Count);

            //Assert.AreEqual(1, outRes.Count);
            //Assert.AreEqual(3, outRes[0].Count);
            Assert.Inconclusive();
        }

        #endregion

        #endregion
    }
}

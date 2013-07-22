﻿using System;
using Dev2.DataList.Contract;
using Dev2.DynamicServices;
using Dev2.Runtime.Hosting;
using Dev2.Runtime.ServiceModel.Data;
using Dev2.Services.Execution;
using Dev2.Workspaces;

namespace Dev2.Runtime.ESB.Execution
{
    /// <summary>
    /// Database Execution Container
    /// </summary>
    public class DatabaseServiceContainer : EsbExecutionContainer
    {
        readonly IServiceExecution _databaseServiceExecution;

        #region Constuctors
        public DatabaseServiceContainer(ServiceAction sa, IDSFDataObject dataObj, IWorkspace workspace, IEsbChannel esbChannel)
            : base(sa, dataObj, workspace, esbChannel)
        {
           _databaseServiceExecution = new DatabaseServiceExecution(dataObj);           
        }
        public DatabaseServiceContainer(IServiceExecution databaseServiceExecution)
            : base(databaseServiceExecution)
        {
            _databaseServiceExecution = databaseServiceExecution;
        }
        #endregion
        #region Execute
        public override Guid Execute(out ErrorResultTO errors)
        {
            var result =_databaseServiceExecution.Execute(out errors);
            return result;
        }
        #endregion
    }
}

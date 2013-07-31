﻿using System;
using Caliburn.Micro;
using Dev2.Common;

namespace Dev2.Studio.Core.Network
{
    public class TcpConnection : TcpConnectionBase
    {
        public TcpConnection(IFrameworkSecurityContext securityContext, Uri appServerUri, int webServerPort, IEventAggregator eventAggregator, bool isAuxiliary = false, int networkTimeout = GlobalConstants.NetworkTimeOut)
            : base(securityContext, appServerUri, webServerPort, eventAggregator, networkTimeout, isAuxiliary)
        {
        }

        // The authenticated user's AccountID is used as the WorkspaceID
        public override Guid WorkspaceID { get { return TCPHost == null ? Guid.Empty : TCPHost.AccountID; } }

        public override Guid ServerID { get { return TCPHost == null ? Guid.Empty : TCPHost.ServerID; } }

        public override bool IsConnected { get { return TCPHost != null && TCPHost.IsConnected; } }

        public override void Connect(bool isAuxiliary = false)
        {
            // This will set TCPHost if connect succeeds
            ConnectImpl(isAuxiliary);
        }

        protected override ITcpClientHost CreateHost(bool isAuxiliary)
        {
            var host = new TcpClientHost(ServerEvents, IsAuxiliary) { EventAggregator = EventAggregator };
            return host;
        }
    }
}
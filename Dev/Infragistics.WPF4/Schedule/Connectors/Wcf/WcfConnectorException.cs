using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Infragistics.Controls.Schedules
{
	/// <summary>
	/// Represents an exception that occurs during a remote service call.
	/// </summary>

	[Serializable]

	public class WcfServiceException : SystemException
	{
		private string _serviceExceptionStackTrace;
		private string _serviceExceptionTypeName;

		/// <summary>
		/// Creates a new <see cref="WcfServiceException"/> instance.
		/// </summary>
		public WcfServiceException()
			: base() { }

		/// <summary>
		/// Creates a new <see cref="WcfServiceException"/> instance.
		/// </summary>
		/// <param name="message">The message describing the error which occurred on the server.</param>
		public WcfServiceException(string message)
			: base(message) { }

		/// <summary>
		/// Creates a new <see cref="WcfServiceException"/> instance.
		/// </summary>
		/// <param name="message">The message describing the error which occurred on the server.</param>
		/// <param name="innerException">The error which caused this error or null.</param>
		public WcfServiceException(string message, Exception innerException)
			: base(message, innerException) { }

		/// <summary>
		/// Creates a new <see cref="WcfServiceException"/> instance.
		/// </summary>
		/// <param name="message">The message describing the error which occurred on the server.</param>
		/// <param name="serviceExceptionTypeName">The full type name of the exception thrown by the service.</param>
		/// <param name="serviceExceptionStackTrace">The stack trace of the exception thrown by the service.</param>
		public WcfServiceException(string message, string serviceExceptionTypeName, string serviceExceptionStackTrace)
			: this(message)
		{
			_serviceExceptionTypeName = serviceExceptionTypeName;
			_serviceExceptionStackTrace = serviceExceptionStackTrace;
		}

		/// <summary>
		/// Gets the stack trace of the exception thrown by the service.
		/// </summary>
		public string ServiceExceptionStackTrace
		{
			get { return _serviceExceptionStackTrace; }
		}

		/// <summary>
		/// Gets the full type name of the exception thrown by the service.
		/// </summary>
		public string ServiceExceptionTypeName
		{
			get { return _serviceExceptionTypeName; }
		}
	}
}

#region Copyright (c) 2001-2012 Infragistics, Inc. All Rights Reserved
/* ---------------------------------------------------------------------*
*                           Infragistics, Inc.                          *
*              Copyright (c) 2001-2012 All Rights reserved               *
*                                                                       *
*                                                                       *
* This file and its contents are protected by United States and         *
* International copyright laws.  Unauthorized reproduction and/or       *
* distribution of all or any portion of the code contained herein       *
* is strictly prohibited and will result in severe civil and criminal   *
* penalties.  Any violations of this copyright will be prosecuted       *
* to the fullest extent possible under law.                             *
*                                                                       *
* THE SOURCE CODE CONTAINED HEREIN AND IN RELATED FILES IS PROVIDED     *
* TO THE REGISTERED DEVELOPER FOR THE PURPOSES OF EDUCATION AND         *
* TROUBLESHOOTING. UNDER NO CIRCUMSTANCES MAY ANY PORTION OF THE SOURCE *
* CODE BE DISTRIBUTED, DISCLOSED OR OTHERWISE MADE AVAILABLE TO ANY     *
* THIRD PARTY WITHOUT THE EXPRESS WRITTEN CONSENT OF INFRAGISTICS, INC. *
*                                                                       *
* UNDER NO CIRCUMSTANCES MAY THE SOURCE CODE BE USED IN WHOLE OR IN     *
* PART, AS THE BASIS FOR CREATING A PRODUCT THAT PROVIDES THE SAME, OR  *
* SUBSTANTIALLY THE SAME, FUNCTIONALITY AS ANY INFRAGISTICS PRODUCT.    *
*                                                                       *
* THE REGISTERED DEVELOPER ACKNOWLEDGES THAT THIS SOURCE CODE           *
* CONTAINS VALUABLE AND PROPRIETARY TRADE SECRETS OF INFRAGISTICS,      *
* INC.  THE REGISTERED DEVELOPER AGREES TO EXPEND EVERY EFFORT TO       *
* INSURE ITS CONFIDENTIALITY.                                           *
*                                                                       *
* THE END USER LICENSE AGREEMENT (EULA) ACCOMPANYING THE PRODUCT        *
* PERMITS THE REGISTERED DEVELOPER TO REDISTRIBUTE THE PRODUCT IN       *
* EXECUTABLE FORM ONLY IN SUPPORT OF APPLICATIONS WRITTEN USING         *
* THE PRODUCT.  IT DOES NOT PROVIDE ANY RIGHTS REGARDING THE            *
* SOURCE CODE CONTAINED HEREIN.                                         *
*                                                                       *
* THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE.              *
* --------------------------------------------------------------------- *
*/
#endregion Copyright (c) 2001-2012 Infragistics, Inc. All Rights Reserved
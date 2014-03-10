using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;




using Infragistics.Documents.Excel.Serialization.Excel2007.SharedContentTypes;

namespace Infragistics.Documents.Excel.Serialization.Excel2007.XLSX.Elements

{
    class UnderlineElement : XLSXElementBase
    {
        #region Constants

        public const string LocalName = "u";

        public const string QualifiedName =
            XLSXElementBase.DefaultXmlNamespace +
            XmlElementBase.NamespaceSeparator +
            UnderlineElement.LocalName;

        public const string ValAttributeName = "val";

        #endregion Constants

        #region Base Class Overrides

        #region Type

        /// <summary>
		/// Returns the <see cref="XLSXElementType">XLSXElementType</see> constant which identifies this element.
        /// </summary>
        public override XLSXElementType Type
        {
            get { return XLSXElementType.u; }
        }
        #endregion Type

        #region Load



#region Infragistics Source Cleanup (Region)




#endregion // Infragistics Source Cleanup (Region)

        protected override void Load(Excel2007WorkbookSerializationManager manager, ExcelXmlElement element, string value, ref bool isReaderOnNextNode)
        {
            IWorkbookFont fontData = (IWorkbookFont)manager.ContextStack[typeof(IWorkbookFont)];
            if (fontData == null)
            {
                Utilities.DebugFail("Failed to find the IWorkbookFont object on the stack.");
                return;
            }
            foreach (ExcelXmlAttribute attribute in element.Attributes)
            {
                string attributeName = XLSXElementBase.GetQualifiedAttributeName(attribute);

                switch (attributeName)
                {
                    case UnderlineElement.ValAttributeName:
                        FontUnderlineStyle val = (FontUnderlineStyle)XLSXElementBase.GetAttributeValue(attribute, DataType.ST_UnderlineValues, ST_UnderlineValues._single);
                        fontData.UnderlineStyle = val;
                        break;

                    default:
                        Utilities.DebugFail("Unknown attribute type in the u (underline) element: " + attributeName);
                        break;
                }
            }

            // If the attribute still hasn't been set, do so now since if the 'val' attribute isn't specified,
            // we should assume that the presence of the tag means 'single'
            if (fontData.UnderlineStyle == FontUnderlineStyle.Default)
                fontData.UnderlineStyle = FontUnderlineStyle.Single;
        }

        #endregion Load

        #region Save



#region Infragistics Source Cleanup (Region)



#endregion // Infragistics Source Cleanup (Region)

        protected override void Save(Excel2007WorkbookSerializationManager manager, ExcelXmlElement element, ref string value)
        {
            IWorkbookFont fontData = (IWorkbookFont)manager.ContextStack[typeof(IWorkbookFont)];
            if (fontData != null)
            {
                // If the underline style is set to 'Single' then we don't need to add this attribute since it is assumed by Excel
                if(fontData.UnderlineStyle != FontUnderlineStyle.Single)
                    XLSXElementBase.AddAttribute(element, UnderlineElement.ValAttributeName, XLSXElementBase.GetXmlString(fontData.UnderlineStyle, DataType.ST_UnderlineValues));
            }
        }

        #endregion Save

        #endregion Base Class Overrides

		// MD 11/4/10 - TFS49093
		#region SaveDirectHelper

		public static void SaveDirectHelper(
			XmlWriter writer,
			FontUnderlineStyle underlineStyle)
		{
			writer.WriteStartElement(UnderlineElement.LocalName);
			writer.WriteAttributeString(UnderlineElement.ValAttributeName, XLSXElementBase.GetXmlString(underlineStyle, DataType.ST_UnderlineValues));
			writer.WriteEndElement();
		} 

		#endregion // SaveDirectHelper
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
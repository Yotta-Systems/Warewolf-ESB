﻿using Dev2.DataList.Contract.Binary_Objects.Structs;
using System;

namespace Dev2.DataList.Contract.Binary_Objects
{
    [Serializable]
    public class BinaryDataListItem : IBinaryDataListItem
    {
        #region Internal Struct

        private SBinaryDataListItem _internalObj = new SBinaryDataListItem();

        #endregion

        #region Properties

        public string TheValue { get { return _internalObj.TheValue; } private set { _internalObj.TheValue = value; } }

        public int ItemCollectionIndex { get { return _internalObj.ItemCollectionIndex; } private set { _internalObj.ItemCollectionIndex = value; } }

        public string Namespace { get { return _internalObj.Namespace; } private set { _internalObj.Namespace = value; } }

        public string FieldName { get { return _internalObj.FieldName; } private set { _internalObj.FieldName = value; } }

        //public string DisplayValue { get { return _internalObj.DisplayValue; } private set { _internalObj.DisplayValue = value; } }

        public string DisplayValue { 
            get
            {
                if (_internalObj.ItemCollectionIndex >= 0 && _internalObj.DisplayValue == null)
                {
                    _internalObj.DisplayValue = DataListUtil.ComposeIntoUserVisibleRecordset(_internalObj.Namespace, _internalObj.ItemCollectionIndex, _internalObj.FieldName);
                }
                
                if (_internalObj.DisplayValue == null)
                {
                    return "null";
                }

                return _internalObj.DisplayValue;
            }
        }

        public bool IsDeferredRead { get { return false; } }

        #endregion Properties

        #region Internal Methods

        internal BinaryDataListItem(string val, string ns, string field, string idx) 
        {
            _internalObj.TheValue = val;
            _internalObj.Namespace = ns;
            _internalObj.FieldName = field;
            int tmp;
            if (Int32.TryParse(idx, out tmp))
            {
                _internalObj.ItemCollectionIndex = tmp;
            }
            else
            {
                _internalObj.ItemCollectionIndex = Int32.MinValue;
            }
        }

        internal BinaryDataListItem(string val, string ns, string field, int idx)
        {
            _internalObj.TheValue = val;
            _internalObj.Namespace = ns;
            _internalObj.FieldName = field;
            _internalObj.ItemCollectionIndex = idx;
            // Travis.Frisinger
            // This is a performance killer, removed for bug 8579
            // And should have been amended above in the property
            //_internalObj.DisplayValue = DataListUtil.ComposeIntoUserVisibleRecordset(_internalObj.Namespace, _internalObj.ItemCollectionIndex, _internalObj.FieldName);//Bug 7891 - null display value
        }

        internal BinaryDataListItem(string val, string fieldName)
        {
            _internalObj.TheValue = val;
            _internalObj.Namespace = string.Empty;
            _internalObj.FieldName = fieldName;
            _internalObj.ItemCollectionIndex = -1;
            _internalObj.DisplayValue = fieldName;
        }

        public IBinaryDataListItem Clone()
        {
            IBinaryDataListItem result;

            if (_internalObj.ItemCollectionIndex > 0)
            {
                result = new BinaryDataListItem(_internalObj.TheValue, _internalObj.Namespace, _internalObj.FieldName, _internalObj.ItemCollectionIndex);
            }
            else
            {
                result = new BinaryDataListItem(_internalObj.TheValue, _internalObj.FieldName);
            }

            return result;
        }

        public void HtmlEncodeRegionBrackets()
        {
            _internalObj.TheValue = DataListUtil.HtmlEncodeRegionBrackets(_internalObj.TheValue);
        }

        public void UpdateValue(string val)
        {
            TheValue = val;
        }

        public void UpdateField(string val)
        {
            FieldName = val;
        }

        public void UpdateRecordset(string val)
        {
            Namespace = val;
        }

        public void UpdateIndex(int idx)
        {
            ItemCollectionIndex = idx;
        }

        public string FetchDeferredLocation()
        {
            throw new NotImplementedException("Standard BinaryDataList Item is does not support this feature");
        }

        #endregion Internal Methods
    }
}

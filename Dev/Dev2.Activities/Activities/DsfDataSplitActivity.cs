﻿using Dev2;
using Dev2.Activities;
using Dev2.Common;
using Dev2.DataList.Contract;
using Dev2.DataList.Contract.Binary_Objects;
using Dev2.DataList.Contract.Builders;
using Dev2.DataList.Contract.Value_Objects;
using Dev2.Diagnostics;
using Dev2.Enums;
using Dev2.Interfaces;
using System;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Unlimited.Applications.BusinessDesignStudio.Activities
{
    public class DsfDataSplitActivity : DsfActivityAbstract<string>, ICollectionActivity
    {
        //private readonly static int _EndOfData = int.MaxValue;
        //IDev2DataLanguageParser parser = DataListFactory.CreateLanguageParser(); 
        //IDataListCompiler dlcomp = DataListFactory.CreateDataListCompiler();
        //IRecordsetScopingObject scopingObj;
        //StringTokenizerOld stok = new StringTokenizerOld();
        string _sourceString;
        //bool endOfFile = false;

        private IList<DataSplitDTO> _resultsCollection;
        public IList<DataSplitDTO> ResultsCollection
        {
            get
            {
                return _resultsCollection;
            }
            set
            {
                _resultsCollection = value;
                OnPropertyChanged("ResultsCollection");
            }
        }

        public bool ReverseOrder { get; set; }

        public string SourceString
        {
            get
            {
                return _sourceString;
            }
            set
            {
                _sourceString = value;
                OnPropertyChanged("SourceString");
            }
        }

        protected override bool CanInduceIdle
        {
            get
            {
                return true;
            }
        }

        public DsfDataSplitActivity()
            : base("Data Split")
        {
            ResultsCollection = new List<DataSplitDTO>();
        }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
        }

        protected override void OnExecute(NativeActivityContext context)
        {

            IDSFDataObject dataObject = context.GetExtension<IDSFDataObject>();
            IDataListCompiler compiler = context.GetExtension<IDataListCompiler>();

            Guid dlID = dataObject.DataListID;
            ErrorResultTO allErrors = new ErrorResultTO();
            ErrorResultTO errors = new ErrorResultTO();
            //Guid executionId = DataListExecutionID.Get(context);

            try
            {

                CleanArguments(ResultsCollection);

                if (ResultsCollection.Count > 0)
                {
                    IBinaryDataListEntry expressionsEntry = compiler.Evaluate(dlID, enActionType.User, SourceString, false, out errors);
                    allErrors.MergeErrors(errors);
                    IDev2DataListEvaluateIterator itr = Dev2ValueObjectFactory.CreateEvaluateIterator(expressionsEntry);
                    IDev2DataListUpsertPayloadBuilder<string> toUpsert = Dev2DataListBuilderFactory.CreateStringDataListUpsertBuilder(true);

                    // SourceString
                    int iterNumber = Int32.MaxValue;

                    // TODO : Properly fetch the total number of executions....?

                    while (itr.HasMoreRecords())
                    {
                        IList<IBinaryDataListItem> cols = itr.FetchNextRowData();
                        foreach (IBinaryDataListItem c in cols)
                        {
                            if (!string.IsNullOrEmpty(c.TheValue))
                            {
                                IDev2Tokenizer tokenizer = CreateSplitPattern(c.TheValue, ResultsCollection, compiler, dlID);
                                int opCnt = 0;
                                int pos = 0;
                                int end = (ResultsCollection.Count - 1);

                                bool added = false;
                                while (tokenizer.HasMoreOps() && opCnt < iterNumber)
                                {

                                    string tmp = tokenizer.NextToken();

                                    if (!string.IsNullOrEmpty(ResultsCollection[pos].OutputVariable))
                                    {
                                        toUpsert.Add(ResultsCollection[pos].OutputVariable, tmp);
                                    }

                                    // Per pass
                                    if (pos == end)
                                    {
                                        pos = 0;
                                        opCnt++;
                                        toUpsert.FlushIterationFrame();
                                    }
                                    else
                                    {
                                        pos++;
                                    }

                                }

                                compiler.Upsert(dlID, toUpsert, out errors);
                                allErrors.MergeErrors(errors);

                                //compiler.Shape(executionId, enDev2ArgumentType.Output, OutputMapping, out errors);
                                //allErrors.MergeErrors(errors);
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                allErrors.AddError(e.Message);
            }
            finally
            {

                // Handle Errors
                if (allErrors.HasErrors())
                {
                    string err = DisplayAndWriteError("DsfDataSplitActivity", allErrors);
                    compiler.UpsertSystemTag(dlID, enSystemTag.Error, err, out errors);
                }
            }
        }

        #region Private Methods


        private IDev2Tokenizer CreateSplitPattern(string StringToSplit, IList<DataSplitDTO> Args, IDataListCompiler compiler, Guid DlID)
        {
            Dev2TokenizerBuilder dtb = new Dev2TokenizerBuilder();
            ErrorResultTO errors = new ErrorResultTO();
            string error = string.Empty;
            dtb.ToTokenize = StringToSplit;

            for (int i = 0; i < Args.Count; i++)
            {
                switch (Args[i].SplitType)
                {
                    case "Index":
                        try
                        {
                            IBinaryDataListEntry entry = compiler.Evaluate(DlID, enActionType.User, Args[i].At, true, out errors);
                            string index = DataListUtil.GetValueAtIndex(entry, 1, out error);
                            int indexNum = Convert.ToInt32(index);
                            if (indexNum > 0)
                            {
                                dtb.AddIndexOp(indexNum);
                            }
                        }
                        catch (Exception) { }
                        break;

                    case "End":
                        dtb.AddEoFOp();
                        break;

                    case "Space":
                        dtb.AddTokenOp(" ", Args[i].Include);
                        break;

                    case "Tab":
                        dtb.AddTokenOp("\t", Args[i].Include);
                        break;

                    case "New Line":
                        if (StringToSplit.Contains("\r\n"))
                        {
                            dtb.AddTokenOp("\r\n", Args[i].Include);
                        }
                        else if (StringToSplit.Contains("\n"))
                        {
                            dtb.AddTokenOp("\n", Args[i].Include);
                        }
                        else if (StringToSplit.Contains("\r"))
                        {
                            dtb.AddTokenOp("\r", Args[i].Include);
                        }
                        break;

                    case "Chars":
                        if (!string.IsNullOrEmpty(Args[i].At))
                        {
                            IBinaryDataListEntry entry = compiler.Evaluate(DlID, enActionType.User, Args[i].At, true, out errors);
                            string val = DataListUtil.GetValueAtIndex(entry, 1, out error);
                            dtb.AddTokenOp(val, Args[i].Include);
                        }
                        break;
                }
            }

            return dtb.Generate();
        }

        private void CleanArguments(IList<DataSplitDTO> Args)
        {
            int count = 0;
            while (count < Args.Count)
            {
                if (Args[count].SplitType == "Index" && string.IsNullOrEmpty(Args[count].At) || Args[count].SplitType == "Chars" && string.IsNullOrEmpty(Args[count].At))
                {
                    Args.RemoveAt(count);
                }
                else
                {
                    count++;
                }
            }
        }

        #endregion Private Methods

        #region Overridden ActivityAbstact Methods

        public override IBinaryDataList GetWizardData()
        {
            string error = string.Empty;
            IBinaryDataList result = Dev2BinaryDataListFactory.CreateDataList();
            string recordsetName = "ResultsCollection";
            result.TryCreateScalarTemplate(string.Empty, "SourceString", string.Empty, true, out error);
            result.TryCreateScalarValue(SourceString, "SourceString", out error);
            result.TryCreateScalarTemplate(string.Empty, "ReverseOrder", string.Empty, true, out error);
            result.TryCreateScalarValue(ReverseOrder.ToString(), "ReverseOrder", out error);
            result.TryCreateRecordsetTemplate(recordsetName, string.Empty, new List<Dev2Column>() { DataListFactory.CreateDev2Column("SplitType", string.Empty), DataListFactory.CreateDev2Column("At", string.Empty), DataListFactory.CreateDev2Column("Include", string.Empty), DataListFactory.CreateDev2Column("Result", string.Empty) }, true, out error);
            foreach (DataSplitDTO item in ResultsCollection)
            {
                result.TryCreateRecordsetValue(item.SplitType, "SplitType", recordsetName, item.IndexNumber, out error);
                result.TryCreateRecordsetValue(item.At, "At", recordsetName, item.IndexNumber, out error);
                result.TryCreateRecordsetValue(item.Include.ToString(), "Include", recordsetName, item.IndexNumber, out error);
                result.TryCreateRecordsetValue(item.OutputVariable, "Result", recordsetName, item.IndexNumber, out error);
            }
            return result;
        }

        #endregion Overridden ActivityAbstact Methods

        private void InsertToCollection(IList<string> listToAdd, ModelItem modelItem)
        {
            ModelItemCollection mic = modelItem.Properties["ResultsCollection"].Collection;

            if (mic != null)
            {
                int startIndex = ResultsCollection.Last(c => !c.CanRemove()).IndexNumber;
                foreach (string s in listToAdd)
                {
                    mic.Insert(startIndex, new DataSplitDTO(s, ResultsCollection[startIndex - 1].SplitType, ResultsCollection[startIndex - 1].At, startIndex + 1));
                    startIndex++;
                }
                CleanUpCollection(mic, modelItem, startIndex);
            }
        }

        private void AddToCollection(IList<string> listToAdd, ModelItem modelItem)
        {
            ModelItemCollection mic = modelItem.Properties["ResultsCollection"].Collection;

            if (mic != null)
            {
                int startIndex = 0;
                mic.Clear();
                foreach (string s in listToAdd)
                {
                    mic.Add(new DataSplitDTO(s, "Chars", string.Empty, startIndex + 1));
                    startIndex++;
                }
                CleanUpCollection(mic, modelItem, startIndex);
            }
        }

        private void CleanUpCollection(ModelItemCollection mic, ModelItem modelItem, int startIndex)
        {
            if (startIndex < mic.Count)
            {
                mic.RemoveAt(startIndex);
            }
            mic.Add(new DataSplitDTO(string.Empty, "Chars", string.Empty, startIndex + 1));
            modelItem.Properties["DisplayName"].SetValue(CreateDisplayName(modelItem, startIndex + 1));
        }

        private string CreateDisplayName(ModelItem modelItem, int count)
        {
            string currentName = modelItem.Properties["DisplayName"].ComputedValue as string;
            if (currentName.Contains("(") && currentName.Contains(")"))
            {
                if (currentName.Contains(" ("))
                {
                    currentName = currentName.Remove(currentName.IndexOf(" ("));
                }
                else
                {
                    currentName = currentName.Remove(currentName.IndexOf("("));
                }
            }
            currentName = currentName + " (" + (count - 1) + ")";
            return currentName;
        }

        #region Get Debug Inputs/Outputs

        #region GetDebugInputs

        public override IList<IDebugItem> GetDebugInputs(IBinaryDataList dataList)
        {
            var result = new List<IDebugItem>();
            DebugItem itemToAdd = new DebugItem();
            itemToAdd.Add(new DebugItemResult { Type = DebugItemResultType.Label, Value = "String To Split" });
            foreach (IDebugItemResult debugItemResult in CreateDebugItems(SourceString, dataList))
            {
                itemToAdd.Add(debugItemResult);
            }
            result.Add(itemToAdd);
            int indexToShow = 1;
            foreach (DataSplitDTO dataSplitDto in ResultsCollection)
            {
                if (dataSplitDto.SplitType == "Index" && dataSplitDto.IndexNumber == ResultsCollection.Count &&
                    dataSplitDto.At == string.Empty)
                {
                    continue;
                }
                itemToAdd = new DebugItem();
                itemToAdd.Add(new DebugItemResult { Type = DebugItemResultType.Label, Value = indexToShow.ToString(CultureInfo.InvariantCulture) });
                itemToAdd.Add(new DebugItemResult { Type = DebugItemResultType.Label, Value = "Split Using" });
                itemToAdd.Add(new DebugItemResult { Type = DebugItemResultType.Value, Value = dataSplitDto.SplitType });
                if (!string.IsNullOrEmpty(dataSplitDto.At))
                {
                    foreach (IDebugItemResult debugItemResult in CreateDebugItems(dataSplitDto.At, dataList))
                    {
                        itemToAdd.Add(debugItemResult);
                    }
                }
                result.Add(itemToAdd);
                indexToShow++;
            }
            return result;
        }

        #endregion

        #region GetDebugOutputs

        public override IList<IDebugItem> GetDebugOutputs(IBinaryDataList dataList)
        {
            var result = new List<IDebugItem>();

            int indexToShow = 1;
            foreach (DataSplitDTO dataSplitDto in ResultsCollection)
            {
                DebugItem itemToAdd = new DebugItem();
                itemToAdd.Add(new DebugItemResult { Type = DebugItemResultType.Label, Value = indexToShow.ToString(CultureInfo.InvariantCulture) });
                if (!string.IsNullOrEmpty(dataSplitDto.OutputVariable))
                {
                    foreach (IDebugItemResult debugItemResult in CreateDebugItems(dataSplitDto.OutputVariable, dataList))
                    {
                        itemToAdd.Add(debugItemResult);
                    }
                }
                result.Add(itemToAdd);
                indexToShow++;
            }
            return result;
        }

        #endregion

        #endregion

        #region Get ForEach Inputs/Ouputs

        public override void UpdateForEachInputs(IList<Tuple<string, string>> updates, NativeActivityContext context)
        {
            foreach (Tuple<string, string> t in updates)
            {
                // locate all updates for this tuple
                var items = ResultsCollection.Where(c => !string.IsNullOrEmpty(c.At) && c.At.Equals(t.Item1));

                // issues updates
                foreach (var a in items)
                {
                    a.At = t.Item2;
                }

                if (SourceString == t.Item1)
                {
                    SourceString = t.Item2;
                }
            }
        }

        public override void UpdateForEachOutputs(IList<Tuple<string, string>> updates, NativeActivityContext context)
        {
            foreach (Tuple<string, string> t in updates)
            {
                // locate all updates for this tuple
                var items = ResultsCollection.Where(c => !string.IsNullOrEmpty(c.OutputVariable) && c.OutputVariable.Equals(t.Item1));

                // issues updates
                foreach (var a in items)
                {
                    a.OutputVariable = t.Item2;
                }
            }
        }

        #endregion

        #region GetForEachInputs/Outputs

        public override IList<DsfForEachItem> GetForEachInputs(NativeActivityContext context)
        {
            var items = (new[] { SourceString }).Union(ResultsCollection.Where(c => !string.IsNullOrEmpty(c.At)).Select(c => c.At)).ToArray();
            return GetForEachItems(context, StateType.Before, items);
        }

        public override IList<DsfForEachItem> GetForEachOutputs(NativeActivityContext context)
        {
            var items = ResultsCollection.Where(c => !string.IsNullOrEmpty(c.OutputVariable)).Select(c => c.OutputVariable).ToArray();
            return GetForEachItems(context, StateType.After, items);
        }

        #endregion

        #region Implementation of ICollectionActivity

        public int GetCollectionCount()
        {
            return ResultsCollection.Count(caseConvertTO => !caseConvertTO.CanRemove());
        }

        public void AddListToCollection(IList<string> listToAdd, bool overwrite, ModelItem modelItem)
        {
            if (!overwrite)
            {
                InsertToCollection(listToAdd, modelItem);
            }
            else
            {
                AddToCollection(listToAdd, modelItem);
            }
        }

        #endregion
    }
}

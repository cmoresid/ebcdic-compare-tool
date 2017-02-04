using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CodeMovement.EbcdicCompare.Models;
using CodeMovement.EbcdicCompare.Models.Exception;
using CodeMovement.EbcdicCompare.Models.Request;
using CodeMovement.EbcdicCompare.Models.Result;
using CodeMovement.EbcdicCompare.Models.ViewModel;
using CodeMovement.EbcdicCompare.DataAccess;
using System.Text;

namespace CodeMovement.EbcdicCompare.Services
{
    public class CompareEbcdicFilesService : ICompareEbcdicFilesService
    {
        private readonly IEbcdicReaderService _ebcdicReaderService;
        private readonly IFileOperation _fileOperation;

        public CompareEbcdicFilesService(IEbcdicReaderService ebcdicReaderService, IFileOperation fileOperation)
        {
            _ebcdicReaderService = ebcdicReaderService;
            _fileOperation = fileOperation;
        }

        public OperationResult<CompareEbcdicFileResult> Compare(CompareEbcdicFilesRequest request)
        {
            var firstFileRecordsResult = ReadEbcdicFile(request.FirstEbcdicFilePath, request.CopybookFilePath);
            if (!firstFileRecordsResult.Successful || string.IsNullOrWhiteSpace(request.SecondEbcdicFilePath))
                return CreateResult(firstFileRecordsResult.Result, new ObservableCollection<EbcdicFileRecordModel>(),
                    firstFileRecordsResult.Messages);

            var secondFileRecordsResult = ReadEbcdicFile(request.SecondEbcdicFilePath, request.CopybookFilePath);
            if (!secondFileRecordsResult.Successful)
                return CreateResult(firstFileRecordsResult.Result, secondFileRecordsResult.Result,
                    secondFileRecordsResult.Messages);

            SetRecordFlags(firstFileRecordsResult, secondFileRecordsResult);
            SetRemainingRecordFlags(firstFileRecordsResult, secondFileRecordsResult);

            return CalculateRecordStatistics(firstFileRecordsResult, secondFileRecordsResult);
        }

        public OperationResult<CompareEbcdicFileResult> CompareEbcdicByteContents(string ebcdicFilePath1, string ebcdicFilePath2)
        {
            var result = new OperationResult<CompareEbcdicFileResult>
            {
                Result = new CompareEbcdicFileResult()
            };

            try
            {
                var file1Size = _fileOperation.GetFileSize(ebcdicFilePath1);
                var file2Size = _fileOperation.GetFileSize(ebcdicFilePath2);
    
                result.Result.FirstEbcdicFile = GetEbcdicFileSize(file1Size);
                result.Result.SecondEbcdicFile = GetEbcdicFileSize(file2Size);

                if (file1Size != file2Size)
                {
                    result.Result.AreIdentical = false;
                    return result;
                }

                var file1Contents = _fileOperation.ReadAllBytes(ebcdicFilePath1);
                var file2Contents = _fileOperation.ReadAllBytes(ebcdicFilePath2);

                result.Result.AreIdentical =
                    file1Contents.Zip(file2Contents, (file1Byte, file2Byte) => file1Byte == file2Byte).All(r => r);
            }
            catch (Exception ex)
            {
                result.AddMessage(ex.Message);
            }

            return result;
        }

        #region "Private Helpers"

        private static OperationResult<CompareEbcdicFileResult> CalculateRecordStatistics(OperationResult<ObservableCollection<EbcdicFileRecordModel>> firstFileRecordsResult,
            OperationResult<ObservableCollection<EbcdicFileRecordModel>> secondFileRecordsResult)
        {
            var firstEbcdicFileAnalysis = AnalyzeEbcdicFileRecords(firstFileRecordsResult.Result);
            var secondEbcdicFileAnalysis = AnalyzeEbcdicFileRecords(secondFileRecordsResult.Result);

            return new OperationResult<CompareEbcdicFileResult>
            {
                Result = new CompareEbcdicFileResult
                {
                    FirstEbcdicFile = firstEbcdicFileAnalysis,
                    SecondEbcdicFile = secondEbcdicFileAnalysis
                }
            };
        }

        private static void SetRemainingRecordFlags(OperationResult<ObservableCollection<EbcdicFileRecordModel>> firstFileRecordsResult,
            OperationResult<ObservableCollection<EbcdicFileRecordModel>> secondFileRecordsResult)
        {
            // Extra records were found in one of the data sets, flag them as such.
            var difference = Math.Abs(firstFileRecordsResult.Result.Count - secondFileRecordsResult.Result.Count);
            var maxValue = Math.Max(firstFileRecordsResult.Result.Count, secondFileRecordsResult.Result.Count);
            var pos = 1;

            for (var i = difference; i > 0; i--)
            {
                if (firstFileRecordsResult.Result.Count > secondFileRecordsResult.Result.Count)
                    firstFileRecordsResult.Result[maxValue - pos].Flag = RecordFlag.Extra;
                else
                    secondFileRecordsResult.Result[maxValue - pos].Flag = RecordFlag.Extra;

                pos += 1;
            }
        }

        private static void SetRecordFlags(OperationResult<ObservableCollection<EbcdicFileRecordModel>> firstFileRecordsResult,
            OperationResult<ObservableCollection<EbcdicFileRecordModel>> secondFileRecordsResult)
        {
            var minCount = Math.Min(firstFileRecordsResult.Result.Count, secondFileRecordsResult.Result.Count);

            for (var i = 0; i < minCount; i++)
            {
                var firstCurrentFileRecord = firstFileRecordsResult.Result[i];
                var secondCurrentFileRecord = secondFileRecordsResult.Result[i];

                if (firstCurrentFileRecord.Equals(secondCurrentFileRecord))
                {
                    firstCurrentFileRecord.Flag = RecordFlag.Identical;
                    firstCurrentFileRecord.Differences = null;

                    secondCurrentFileRecord.Flag = RecordFlag.Identical;
                    secondCurrentFileRecord.Differences = null;
                }
                else
                {
                    firstCurrentFileRecord.Flag = RecordFlag.Different;
                    secondCurrentFileRecord.Flag = RecordFlag.Different;

                    MarkDifferences(firstCurrentFileRecord, secondCurrentFileRecord);
                }
            }
        }

        private static void MarkDifferences(EbcdicFileRecordModel firstFileRecord, EbcdicFileRecordModel secondFileRecord)
        {
            var firstRecordStr = firstFileRecord.RowValue;
            var secondRecordStr = secondFileRecord.RowValue;
            var differenceString = new StringBuilder();

            int i = 0;
            for (; i < Math.Min(firstRecordStr.Length, secondRecordStr.Length); i++)
                differenceString.Append(firstRecordStr[i] != secondRecordStr[i] ? "^" : " ");

            while (i < Math.Max(firstRecordStr.Length, secondRecordStr.Length))
            {
                differenceString.Append("^");
                i++;
            }

            firstFileRecord.Differences = differenceString.ToString();
            secondFileRecord.Differences = differenceString.ToString();
        }

        private static EbcdicFileAnalysis AnalyzeEbcdicFileRecords(ObservableCollection<EbcdicFileRecordModel> records)
        {
            return new EbcdicFileAnalysis
            {
                EbcdicFileRecords = records,
                Total = records.Count,
                Differences = records.Count(record => record.Flag == RecordFlag.Different),
                Matches = records.Count(record => record.Flag == RecordFlag.Identical),
                Extras = records.Count(record => record.Flag == RecordFlag.Extra)
            };
        }

        private static EbcdicFileAnalysis GetEbcdicFileSize(long fileSize)
        {
            return new EbcdicFileAnalysis
            {
                FileSize = fileSize
            };
        }

        private OperationResult<CompareEbcdicFileResult> CreateResult(
            ObservableCollection<EbcdicFileRecordModel> firstEbcdicFileRecordModels,
            ObservableCollection<EbcdicFileRecordModel> secondEbcdicFileRecordModels,
            List<string> messages)
        {
            return OperationResult<CompareEbcdicFileResult>.CreateResult(new CompareEbcdicFileResult
            {
                FirstEbcdicFile = new EbcdicFileAnalysis { EbcdicFileRecords = firstEbcdicFileRecordModels },
                SecondEbcdicFile = new EbcdicFileAnalysis { EbcdicFileRecords = secondEbcdicFileRecordModels }
            }, messages);
        }

        private OperationResult<ObservableCollection<EbcdicFileRecordModel>> ReadEbcdicFile(string ebcdicFilePath,
            string copybookFilePath)
        {
            var result = new OperationResult<ObservableCollection<EbcdicFileRecordModel>>();

            try
            {
                var ebcdicFileRows = _ebcdicReaderService.ReadEbcdicFile(ebcdicFilePath,
                    copybookFilePath);

                result.Result = EbcdicFileRecordModel.Map(ebcdicFileRows);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.InnerException is System.Xml.XmlException)
                {
                    result.AddMessage(
                        "It looks like the selected copybook XML file you selected is not a valid XML file.\n\n" +
                        "Are you sure you selected the right file?\n\n" +
                        copybookFilePath);
                }
            }
            catch (EbcdicParseException ex)
            {
                if (ex.InnerException is FieldParsingException)
                {
                    result.AddMessage(
                        "There has been an error parsing a field in EBCDIC file.\n\n" +
                        "Message: " + ex.InnerException.Message);
                }
                else if (ex.InnerException is UnexpectedFieldTypeException)
                {
                    result.AddMessage(
                        "There as been an error parsing the EBCDIC file. It looks the \n" +
                        "contents of the EBCDIC file don't match up to the fields specified \n" +
                        "in the copybook XML file.\n\n" +
                        "Are you sure you have selected the correct copybook XML file?\n\n" +
                        "Message: " + ex.InnerException.Message);
                }
                else
                {
                    result.AddMessage(
                        "There has been an error parsing the selected EBCDIC file.\n\n" +
                        "Message: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                result.AddMessage(ex.Message);
            }

            return result;
        }

        #endregion
    }
}
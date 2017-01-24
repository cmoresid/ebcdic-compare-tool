using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NLog;
using CodeMovement.EbcdicCompare.Models.Copybook;
using CodeMovement.EbcdicCompare.Models.Ebcdic;
using CodeMovement.EbcdicCompare.Models.Exception;
using CodeMovement.EbcdicCompare.Services.Extension;

namespace CodeMovement.EbcdicCompare.Services
{
    /// <summary>
    /// An EbcdicReader reads bytes from an input stream and returns
    /// records, according to a copybook. Each call to NextRecord
    /// returns a list of objects, containing the decoded values of the fields. When
    /// there are no more records to read, it returns null.
    /// Copybooks with multiple records are supported. The reader relies on the
    /// position get/set methods of the input stream to detect
    /// the format of the current record.
    /// </summary>
    public class EbcdicReader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #region Attributes
        private static readonly int RdwSize = 4;
        private readonly FileFormat _fileFormat;
        private readonly EbcdicDecoder _decoder;
        private readonly RecordFormatMap _recordFormatMap;
        private readonly bool _hasRdw;
        private readonly BufferedStream _stream;
        private int _readRecords;
        #endregion

        /// <summary>
        ///  Constructs an EbcdicReader.
        /// </summary>
        /// <param name="inputStream">the stream to read the data from</param>
        /// <param name="fileFormat">the copybook to use for decoding records</param>
        /// <param name="hasRdw">true if records have a record descriptor word (RDW)</param>
        public EbcdicReader(BufferedStream inputStream, FileFormat fileFormat, bool hasRdw)
        {
            _stream = inputStream;
            _fileFormat = fileFormat;
            _decoder = new EbcdicDecoder(fileFormat.Charset);
            _recordFormatMap = new RecordFormatMap(fileFormat);
            _hasRdw = hasRdw;
        }

        /// <summary>
        ///  Returns the next record as a list of objects.
        /// </summary>
        /// <returns>a list containing the decoded fields of the record. If there are
        /// multiple record formats, the first item in the list is the
        /// discriminator value. Returns null if there are no
        /// more records.</returns>
        /// <exception cref="IOException">&nbsp;</exception>
        /// <exception cref="EbcdicException">&nbsp;</exception>
        public List<FieldValuePair> NextRecord()
        {
            List<FieldValuePair> result = new List<FieldValuePair>();
            if (_readRecords == 0 && _fileFormat.HeaderSize > 0)
            {
                _stream.Seek(_fileFormat.HeaderSize + _fileFormat.NewLineSize, SeekOrigin.Begin);
            }
            if (_hasRdw)
            {
                _stream.Seek(RdwSize, SeekOrigin.Current);
            }
            _readRecords++;
            try
            {
                var recordFormat = RetrieveRecordFormat(result);
                result.AddRange(ReadFields(recordFormat));
            }
            catch (EndOfFileException)
            {
                return null;
            }

            if (_fileFormat.NewLineSize > 0)
            {
                _stream.Seek(_fileFormat.NewLineSize, SeekOrigin.Current);
            }

            return result;
        }

        #region private methods
        /// <summary>
        /// read a fields list
        /// </summary>
        /// <param name="fieldsList"></param>
        /// <returns></returns>
        /// <exception cref="IOException">&nbsp;</exception>
        /// <exception cref="EbcdicException">&nbsp;</exception>
        private List<FieldValuePair> ReadFields(IFieldsList fieldsList)
        {
            List<FieldValuePair> values = new List<FieldValuePair>();
            IDictionary<string, decimal> readNumericValues = new Dictionary<string, decimal>();
            foreach (CopybookElement fieldFormat in fieldsList.Elements)
            {
                var format = fieldFormat as FieldFormat;
                values.Add(ReadField(format, readNumericValues));
            }
            return values;
        }

        /// <summary>
        /// read a field
        /// </summary>
        /// <param name="fieldFormat"></param>
        /// <param name="readNumericValues"></param>
        /// <returns></returns>
        private FieldValuePair ReadField(FieldFormat fieldFormat, IDictionary<string, decimal> readNumericValues)
        {
            List<FieldValuePair> values = new List<FieldValuePair>();
            int occurs;

            if (fieldFormat.HasDependencies())
            {
                if (readNumericValues.ContainsKey(fieldFormat.DependingOn))
                {
                    occurs = Decimal.ToInt32(readNumericValues[fieldFormat.DependingOn]);
                }
                else
                {
                    throw new System.Exception(
                    string.Format("Check your copybook :[{0}] is not present, but field format says it has dependencies ...",
                        fieldFormat.DependingOn));
                }
            }
            else
            {
                occurs = fieldFormat.Occurs;
            }

            for (int i = 0; i < occurs; i++)
            {
                byte[] bytes = Read(fieldFormat.ByteSize, fieldFormat);
                object value = _decoder.Decode(bytes, fieldFormat);
                values.Add(new FieldValuePair(fieldFormat, value));
                if (value is decimal)
                {
                    readNumericValues[fieldFormat.Name] = (decimal)value;
                }
            }

            return values[0];
        }

        /// <summary>
        /// read the discriminator value
        /// </summary>
        /// <returns></returns>
        /// <exception cref="IOException">&nbsp;</exception>
        /// <exception cref="EbcdicException">&nbsp;</exception>
        private string ReadDiscriminatorValue()
        {
            var position = _stream.Position;//mark
            byte[] bytes = Read(_fileFormat.DiscriminatorSize, null);//read
            _stream.Position = position; //reset
            return Encoding.GetEncoding(_fileFormat.Charset).GetString(bytes);
        }

        /// <summary>
        /// Technical read byte array, given length and field format
        /// </summary>
        /// <param name="length"></param>
        /// <param name="fieldFormat"></param>
        /// <returns></returns>
        /// <exception cref="IOException">&nbsp;</exception>
        /// <exception cref="EbcdicException">&nbsp;</exception>
        private byte[] Read(int length, FieldFormat fieldFormat)
        {
            byte[] bytes = new byte[length];
            int readBytes = _stream.Read(bytes);
            if (readBytes == 0) // note : -1 in java
            {
                throw new EndOfFileException();
            }
            if (readBytes < length)
            {
                throw new FieldParsingException(fieldFormat, bytes);
            }
            return bytes;
        }

        /// <summary>
        /// retrieve record format
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        /// <exception cref="IOException">&nbsp;</exception>
        /// <exception cref="EbcdicException">&nbsp;</exception>
        private RecordFormat RetrieveRecordFormat(List<FieldValuePair> result)
        {
            RecordFormat recordFormat;
            if (_fileFormat.DiscriminatorSize > 0)
            {
                string discriminatorValue = ReadDiscriminatorValue();
                recordFormat = _recordFormatMap.GetFromDiscriminator(discriminatorValue);
                result.Add(new FieldValuePair(null, discriminatorValue));
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Record format detected:" + recordFormat.CobolRecordName);
                }
            }
            else
            {
                recordFormat = _recordFormatMap.Default;
                result.Add(new FieldValuePair(null, recordFormat.CobolRecordName));
            }

            return recordFormat;
        }
        #endregion


    }
}

//
//   Copyright 2015 Blu Age Corporation - Plano, Texas
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeMovement.EbcdicCompare.Models.Copybook;
using CodeMovement.EbcdicCompare.Models.Ebcdic;
using CodeMovement.EbcdicCompare.Models.Exception;
using System.Diagnostics.CodeAnalysis;

namespace CodeMovement.EbcdicCompare.Services
{
    /// <summary>
    /// A Summer.Batch reader for EBCDIC files.Given a file and a copybook it
    /// returns the records in the file. An IEbcdicReaderMapper is also
    /// required to match records to actual business objects
    /// </summary>
    /// <typeparam name="T">&nbsp;the type of the business objects to read</typeparam>
    [ExcludeFromCodeCoverage]
    public class EbcdicFileReader
    {
        private readonly string _ebcdicFilePath;
        private readonly string _copybookFilePath;

        private BufferedStream _inputStream;
        private EbcdicReader _reader;
        private int _nbRead;

        public EbcdicFileReader(string ebcdicFilePath, string copybookFilePath)
        {
            _ebcdicFilePath = ebcdicFilePath;
            _copybookFilePath = copybookFilePath;
        }

        /// <summary>
        /// @see AbstractItemCountingItemStreamItemReader#DoRead
        /// </summary>
        /// <returns></returns>
        public EbcdicFileRow Read()
        {
            EbcdicFileRow record = null;
            List<FieldValuePair> fields;
            try
            {
                fields = _reader.NextRecord();
            }
            catch (EbcdicException e)
            {
                throw new EbcdicParseException("Error while parsing item number " + _nbRead, e);
            }

            if (fields == null)
                return null;

            var distinguishedPattern = fields[0].Value;

            record = new EbcdicFileRow()
            {
                RecordTypeName = (string)distinguishedPattern,
                FieldValues = fields.Skip(1).ToList()
            };

            _nbRead++;

            return record;
        }

        /// <summary>
        /// @see AbstractItemCountingItemStreamItemReader#DoOpen
        /// </summary>
        public void Open()
        {
            _inputStream = new BufferedStream(File.Open(_ebcdicFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

            FileFormat fileFormat = CopybookLoader.LoadCopybook(File.Open(_copybookFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            _reader = new EbcdicReader(_inputStream, fileFormat, false);

            _nbRead = 0;
        }

        /// <summary>
        /// @see AbstractItemCountingItemStreamItemReader#DoClose
        /// </summary>
        public void Close()
        {
            if (_inputStream != Stream.Null)
            {
                _inputStream.Close();
            }
        }
    }
}

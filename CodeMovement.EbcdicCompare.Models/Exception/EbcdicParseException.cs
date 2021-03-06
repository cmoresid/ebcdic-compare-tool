﻿//
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

using System;
using System.Diagnostics.CodeAnalysis;

namespace CodeMovement.EbcdicCompare.Models.Exception
{
    /// <summary>
    /// Exception throw by the EbcdicFileReader when there is an error reading an
    /// EBCDIC file.
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class EbcdicParseException : System.Exception
    {
        /// <summary>
        /// Custom constructor with a message and a inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public EbcdicParseException(string message, System.Exception exception)
            : base(message, exception)
        {
        }

        /// <summary>
        /// Custom constructor with a message
        /// </summary>
        /// <param name="message"></param>
        public EbcdicParseException(string message)
            : base(message)
        {
        }
    }
}

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

using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace CodeMovement.EbcdicCompare.Models.Copybook
{
    /// <summary>
    /// Xml reui of a Copybook Element
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class CopybookElement
    {
        /// <summary>
        /// Name attribute.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Occurs attribute.
        /// </summary>
        [XmlAttribute]
        public int Occurs { get; set; }

        /// <summary>
        /// DependingOn attribute.
        /// </summary>
        [XmlAttribute]
        public string DependingOn { get; set; }

        /// <summary>
        /// Redefined attribute.
        /// </summary>
        [XmlAttribute]
        public bool Redefined { get; set; }

        /// <summary>
        /// Redefines attribute.
        /// </summary>
        [XmlAttribute]
        public string Redefines { get; set; }

        /// <summary>
        /// Whether this element has dependencies or not.
        /// </summary>
        /// <returns></returns>
        public abstract bool HasDependencies();
    }
}

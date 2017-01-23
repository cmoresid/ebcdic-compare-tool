using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CodeMovement.EbcdicCompare.Models.Copybook
{
    /// <summary>
    /// Xml reui of FileFormat.
    /// </summary>
    [XmlRoot]
    public class FileFormat
    {
        /// <summary>
        /// List of record formats.
        /// </summary>
        [XmlElement("RecordFormat")]
        public List<RecordFormat> RecordFormats { get; set; }

        /// <summary>
        /// ConversionTable attribute.
        /// </summary>
        [XmlAttribute("ConversionTable")]
        public string Charset { get; set; }

        /// <summary>
        /// distinguishFieldSize attribute.
        /// </summary>
        [XmlAttribute("distinguishFieldSize")]
        public int DiscriminatorSize { get; set; }

        /// <summary>
        /// newLineSize attribute.
        /// </summary>
        [XmlAttribute("newLineSize")]
        public int NewLineSize { get; set; }

        /// <summary>
        /// headerSize attribute.
        /// </summary>
        [XmlAttribute("headerSize")]
        public int HeaderSize { get; set; }

        /// <summary>
        /// dataFileImplementation attribute.
        /// </summary>
        [XmlAttribute("dataFileImplementation")]
        public string DataFileImplementation { get; set; }
    }
}

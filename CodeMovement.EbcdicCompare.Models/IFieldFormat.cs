using CodeMovement.EbcdicCompare.Models.Ebcdic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMovement.EbcdicCompare.Models
{
    public interface IFieldFormat
    {
        string FormatField(FieldValuePair field);
    }
}

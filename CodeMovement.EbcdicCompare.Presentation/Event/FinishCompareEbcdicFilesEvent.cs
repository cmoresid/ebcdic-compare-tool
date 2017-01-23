using Prism.Events;
using CodeMovement.EbcdicCompare.Models.Result;

namespace CodeMovement.EbcdicCompare.Presentation.Event
{
    public class FinishCompareEbcdicFilesEvent : PubSubEvent<CompareEbcdicFileResult>
    {
    }
}

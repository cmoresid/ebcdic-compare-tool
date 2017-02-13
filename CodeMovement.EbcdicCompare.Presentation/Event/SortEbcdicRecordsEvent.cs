using CodeMovement.EbcdicCompare.Models.Request;
using Prism.Events;

namespace CodeMovement.EbcdicCompare.Presentation.Event
{
    public class SortEbcdicRecordsEvent : PubSubEvent<SortEbcdicRecordsRequest>
    {
    }
}

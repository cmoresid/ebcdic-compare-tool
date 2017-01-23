using Prism.Events;
using CodeMovement.EbcdicCompare.Models.Request;

namespace CodeMovement.EbcdicCompare.Presentation.Event
{
    public class FilterEbcdicRecordsEvent : PubSubEvent<FilterEbcdicRecordsRequest>
    {
    }
}

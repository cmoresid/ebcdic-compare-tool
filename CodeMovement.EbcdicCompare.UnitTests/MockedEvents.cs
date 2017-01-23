using System;
using Prism.Events;
using CodeMovement.EbcdicCompare.Models.Request;
using CodeMovement.EbcdicCompare.Models.Result;
using CodeMovement.EbcdicCompare.Presentation.Event;

namespace CodeMovement.EbcdicCompare.Tests
{
    public class ViewEbcdicFileRequestEventMock : ViewEbcdicFileRequestEvent
    {
        public override void Publish(ViewEbcdicFileRequest payload)
        {
            ReceivedPayload = payload;
        }

        public ViewEbcdicFileRequest ReceivedPayload { get; private set; }
    }

    public class FinishReadEbcdicFileEventMock : FinishReadEbcdicFileEvent
    {
        public override void Publish(FinishReadEbcdicFile payload)
        {
            ReceivedPayload = payload;
            Callback(ReceivedPayload);
        }

        public override SubscriptionToken Subscribe(Action<FinishReadEbcdicFile> action, ThreadOption threadOption, bool keepSubscriberReferenceAlive,
            Predicate<FinishReadEbcdicFile> filter)
        {
            Callback = action;
            return new SubscriptionToken(e => { });
        }

        public FinishReadEbcdicFile ReceivedPayload { get; private set; }
        public Action<FinishReadEbcdicFile> Callback { get; private set; }
    }

    public class ClearEbcdicFileGridEventMock : ClearEbcdicFileGridEvent
    {
        public override void Publish(ClearEbcdicFileGridRequest payload)
        {
            ReceivedPayload = payload;
        }

        public ClearEbcdicFileGridRequest ReceivedPayload { get; private set; }
    }
}

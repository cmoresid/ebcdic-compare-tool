using System;
using Prism.Events;
using CodeMovement.EbcdicCompare.Models.Request;
using CodeMovement.EbcdicCompare.Models.Result;
using CodeMovement.EbcdicCompare.Presentation.Event;
using System.Collections.Generic;

namespace CodeMovement.EbcdicCompare.Tests
{
    public class ViewEbcdicFileRequestEventMock : ViewEbcdicFileRequestEvent
    {
        public override void Publish(ViewEbcdicFileRequest payload)
        {
            ReceivedPayload = payload;
            Callback(payload);
        }

        public override SubscriptionToken Subscribe(Action<ViewEbcdicFileRequest> action, ThreadOption threadOption, bool keepSubscriberReferenceAlive, Predicate<ViewEbcdicFileRequest> filter)
        {
            Callback = action;
            return new SubscriptionToken(e => { });
        }

        public ViewEbcdicFileRequest ReceivedPayload { get; private set; }

        public Action<ViewEbcdicFileRequest> Callback { get; private set; }
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

    public class CompareEbcdicFilesRequestEventMock : CompareEbcdicFilesRequestEvent
    {
        public override void Publish(CompareEbcdicFilesRequest payload)
        {
            ReceivedPayload = payload;
            Callback(payload);
        }

        public override SubscriptionToken Subscribe(Action<CompareEbcdicFilesRequest> action, ThreadOption threadOption, bool keepSubscriberReferenceAlive, Predicate<CompareEbcdicFilesRequest> filter)
        {
            Callback = action;
            return new SubscriptionToken(e => { });
        }

        public CompareEbcdicFilesRequest ReceivedPayload { get; private set; }

        public Action<CompareEbcdicFilesRequest> Callback { get; private set; }
    }

    public class FilterEbcdicRecordsEventMock : FilterEbcdicRecordsEvent
    {
        public override void Publish(FilterEbcdicRecordsRequest payload)
        {
            ReceivedPayload = payload;
            Callback(payload);
        }

        public override SubscriptionToken Subscribe(Action<FilterEbcdicRecordsRequest> action, ThreadOption threadOption, bool keepSubscriberReferenceAlive, Predicate<FilterEbcdicRecordsRequest> filter)
        {
            Callback = action;
            return new SubscriptionToken(e => { });
        }

        public FilterEbcdicRecordsRequest ReceivedPayload { get; private set; }

        public Action<FilterEbcdicRecordsRequest> Callback { get; private set; }
    }

    public class UpdateEbcdicFileGridEventMock : UpdateEbcdicFileGridEvent
    {
        public override void Publish(UpdateEbcdicFileGridResult payload)
        {
            foreach (var kv in Mapping)
            {
                if (kv.Key(payload))
                    kv.Value(payload);
            }
        }

        public override SubscriptionToken Subscribe(Action<UpdateEbcdicFileGridResult> action, ThreadOption threadOption, bool keepSubscriberReferenceAlive, Predicate<UpdateEbcdicFileGridResult> filter)
        {
            Mapping = Mapping ?? new Dictionary<Predicate<UpdateEbcdicFileGridResult>, Action<UpdateEbcdicFileGridResult>>();
            Mapping[filter] = action;

            return new SubscriptionToken((e) => { });
        }

        public Dictionary<Predicate<UpdateEbcdicFileGridResult>, Action<UpdateEbcdicFileGridResult>> Mapping { get; private set; }
    }
}

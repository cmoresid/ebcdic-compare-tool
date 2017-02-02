using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using CodeMovement.EbcdicCompare.Tests;
using CodeMovement.EbcdicCompare.Presentation.Event;
using CodeMovement.EbcdicCompare.Models.Request;
using Prism.Events;
using CodeMovement.EbcdicCompare.Models.Result;
using CodeMovement.EbcdicCompare.Presentation.ViewModel;
using System.Collections.ObjectModel;
using CodeMovement.EbcdicCompare.Models.ViewModel;
using System.Collections.Generic;
using CodeMovement.EbcdicCompare.Models;

namespace CodeMovement.EbcdicCompare.UnitTests.ViewModels
{
    [TestClass]
    public class EbcdicFileGridViewModelTests
    {
        [TestMethod]
        public void EbcdicFileGridViewModel_Initialize()
        {
            var filterEvent = MockRepository.GenerateMock<FilterEbcdicRecordsEventMock>();
            filterEvent.Expect(m => m.Subscribe(Arg<Action<FilterEbcdicRecordsRequest>>.Is.Anything, 
                Arg<ThreadOption>.Is.Anything, Arg<bool>.Is.Anything, 
                Arg<Predicate<FilterEbcdicRecordsRequest>>.Is.Anything)).Return(new SubscriptionToken(e => { }));

            var updateEvent = MockRepository.GenerateMock<UpdateEbcdicFileGridEventMock>();
            updateEvent.Expect(m => m.Subscribe(Arg<Action<UpdateEbcdicFileGridResult>>.Is.Anything,
                Arg<ThreadOption>.Is.Anything, Arg<bool>.Is.Anything,
                Arg<Predicate<UpdateEbcdicFileGridResult>>.Is.Anything)).Return(new SubscriptionToken(e => { }));

            var eventAggregator = TestHelper.CreateEventAggregator(filterEvent: filterEvent, updateEvent: updateEvent);

            var viewModel = new EbcdicFileGridViewModel(eventAggregator);

            filterEvent.VerifyAllExpectations();
        }

        [TestMethod]
        public void EbcdicFileGridViewModel_Filter_Results()
        {
            var filterEvent = MockRepository.GenerateMock<FilterEbcdicRecordsEventMock>();
            filterEvent.Expect(m => m.Subscribe(Arg<Action<FilterEbcdicRecordsRequest>>.Is.Anything,
                Arg<ThreadOption>.Is.Anything, Arg<bool>.Is.Anything,
                Arg<Predicate<FilterEbcdicRecordsRequest>>.Is.Anything)).Return(new SubscriptionToken(e => { }));

            var eventAggregator = TestHelper.CreateEventAggregator(filterEvent: filterEvent);

            var viewModel = new EbcdicFileGridViewModel(eventAggregator);
            viewModel.AllEbcdicFileRecordModels = new ObservableCollection<EbcdicFileRecordModel>();
            viewModel.VisibleEbcdicFileRecords = new ObservableCollection<EbcdicFileRecordModel>();

            filterEvent.Publish(new FilterEbcdicRecordsRequest
            {
                FilterBy = new List<RecordFlag>() {  RecordFlag.Different, RecordFlag.Extra }
            });

            Assert.AreEqual(0, viewModel.AllEbcdicFileRecordModels.Count);
            Assert.AreEqual(0, viewModel.VisibleEbcdicFileRecords.Count);
        }
    }
}

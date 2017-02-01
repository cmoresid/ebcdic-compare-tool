﻿using Prism.Events;
using Prism.Mvvm;
using CodeMovement.EbcdicCompare.Models.ViewModel;
using CodeMovement.EbcdicCompare.Presentation.Event;
using CodeMovement.EbcdicCompare.Models.Request;
using System.Linq;
using System.Collections.ObjectModel;
using CodeMovement.EbcdicCompare.Models.Result;

namespace CodeMovement.EbcdicCompare.Presentation.ViewModel
{
    public class EbcdicFileGridViewModel : BindableBase
    {
        private ObservableCollection<EbcdicFileRecordModel> _visibleEbcdicFileRecords;
        private readonly IEventAggregator _eventAggregator;

        public EbcdicFileGridViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<FilterEbcdicRecordsEvent>().Subscribe(OnFilterByEbcdicFileGrid, 
                ThreadOption.UIThread, false, (e) => e.RegionName == RegionName);

            _eventAggregator.GetEvent<UpdateEbcdicFileGridEvent>().Subscribe(OnUpdateEbcdicFileGrid,
                ThreadOption.UIThread, false, (e) => e.Region == RegionName);
        }

        public string RegionName { get; set; }

        public ObservableCollection<EbcdicFileRecordModel> AllEbcdicFileRecordModels { get; set; }

        public ObservableCollection<EbcdicFileRecordModel> VisibleEbcdicFileRecords
        {
            get { return _visibleEbcdicFileRecords; }
            set
            {
                _visibleEbcdicFileRecords = value;
                OnPropertyChanged("VisibleEbcdicFileRecords");
            }
        }

        private void OnFilterByEbcdicFileGrid(FilterEbcdicRecordsRequest request)
        {
            VisibleEbcdicFileRecords = new ObservableCollection<EbcdicFileRecordModel>(
                AllEbcdicFileRecordModels.Where(record => request.FilterBy.Contains(record.Flag)));
        }

        private void OnUpdateEbcdicFileGrid(UpdateEbcdicFileGridResult result)
        {
            AllEbcdicFileRecordModels = result.AllEbcdicFileRecordModels;
            VisibleEbcdicFileRecords = result.VisibleEbcdicFileRecords;
        }
    }
}

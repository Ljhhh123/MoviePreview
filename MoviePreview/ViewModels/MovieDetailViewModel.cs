﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MoviePreview.Helpers;
using MoviePreview.Models;
using MoviePreview.Services;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace MoviePreview.ViewModels
{
    public class MovieDetailViewModel : ViewModelBase
    {
        public MovieDetailViewModel()
        {
            MovieDetail = new MovieItemDetail();
            Source = new ObservableCollection<PostItem>();
        }
        public MovieItemDetail movieDetail;
        public MovieItemDetail MovieDetail {
            get => movieDetail;
            set => Set(ref movieDetail, value);
        }

        public Visibility GetVisibility {
            get {
                return MovieDetail.ID == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public void AddData()
        {
            MovieDetail = TimeAPIService.CurrentDetail;
            Source = new ObservableCollection<PostItem>(movieDetail.Images);
            RaisePropertyChanged("GetVisibility");
        }

        public const string ImageGallerySelectedIdKey = "ImageGallerySelectedIdKey";
        public const string ImageGalleryAnimationOpen = "ImageGallery_AnimationOpen";
        public const string ImageGalleryAnimationClose = "ImageGallery_AnimationClose";

        private ObservableCollection<PostItem> _source;
        private ICommand _itemSelectedCommand;
        private GridView _imagesGridView;

        public ObservableCollection<PostItem> Source {
            get => _source;
            set => Set(ref _source, value);
        }

        public ICommand ItemSelectedCommand => _itemSelectedCommand ?? (_itemSelectedCommand = new RelayCommand<ItemClickEventArgs>(OnsItemSelected));
        

        public void Initialize(GridView imagesGridView)
        {
            _imagesGridView = imagesGridView;
        }

        public async Task LoadAnimationAsync()
        {
            var selectedImageId = await ApplicationData.Current.LocalSettings.ReadAsync<string>(ImageGallerySelectedIdKey);
            if (!string.IsNullOrEmpty(selectedImageId))
            {
                var item = _imagesGridView.Items.FirstOrDefault(i => ((PostItem)i).ID == selectedImageId);
                if (item != null) _imagesGridView.ScrollIntoView(item);
                //var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation(ImageGalleryAnimationClose);
                //if (animation != null)
                //{
                //    var item = _imagesGridView.Items.FirstOrDefault(i => ((PostItem)i).ID == selectedImageId);
                //    _imagesGridView.ScrollIntoView(item);
                //    await _imagesGridView.TryStartConnectedAnimationAsync(animation, item, "galleryImage");
                //}

                ApplicationData.Current.LocalSettings.SaveString(ImageGallerySelectedIdKey, string.Empty);
            }
        }

        public NavigationServiceEx NavigationService {
            get {
                return CommonServiceLocator.ServiceLocator.Current.GetInstance<NavigationServiceEx>();
            }
        }

        private void OnsItemSelected(ItemClickEventArgs args)
        {
            var selected = args.ClickedItem as PostItem;
            _imagesGridView.PrepareConnectedAnimation(ImageGalleryAnimationOpen, selected, "galleryImage");
            NavigationService.Navigate(typeof(ImageGalleryDetailViewModel).FullName, args.ClickedItem);
        }
    }
}

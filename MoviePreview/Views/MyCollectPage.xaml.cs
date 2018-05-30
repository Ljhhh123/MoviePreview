﻿using System;
using System.Diagnostics;
using GalaSoft.MvvmLight.Views;
using MoviePreview.Models;
using MoviePreview.Services;
using MoviePreview.ViewModels;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace MoviePreview.Views
{
    public sealed partial class MyCollectPage : Page
    {
        private MyCollectViewModel ViewModel {
            get { return DataContext as MyCollectViewModel; }
        }

        public MyCollectPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ViewModel.SaveData();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            CollectionsView.IsEnabled = true;
            LoadToOther.Visibility = Visibility.Collapsed;
            ViewModel.SyncData();
        }

        private void ChangeNoteButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // FlyoutBase.GetAttachedFlyout(((e.OriginalSource as Button).Parent as StackPanel).Parent as FrameworkElement).Hide();
            var a = (((e.OriginalSource as Button).Parent as StackPanel).Children[0] as TextBox).Text;
            var item = ((e.OriginalSource as Button).DataContext as MovieItem);
            ViewModel.OnsItemChange(item.ID, a);
        }

        private async void DisplayDeleteFileDialog()
        {
            ContentDialog deleteFileDialog = new ContentDialog
            {
                Title = ResourceLoader.GetForCurrentView().GetString("MyCollectPage_CS_Delete/Title"),
                Content = string.Format(ResourceLoader.GetForCurrentView().GetString("MyCollectPage_CS_Delete/Content"), ToDelItem.TitleCn),
                PrimaryButtonText = ResourceLoader.GetForCurrentView().GetString("MyCollectPage_CS_Delete/PrimaryButtonText"),
                CloseButtonText = ResourceLoader.GetForCurrentView().GetString("MyCollectPage_CS_Delete/CloseButtonText")
            };

            ContentDialogResult result = await deleteFileDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ViewModel.OnsItemDelete(ToDelItem);
            }
        }

        private MovieItem ToDelItem;
        private void DeleteButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ToDelItem = (e.OriginalSource as Button).DataContext as MovieItem;
            DisplayDeleteFileDialog();
        }

        private async void CollectionsView_ItemClick(object sender, ItemClickEventArgs e)
        {
            CollectionsView.IsEnabled = false;
            LoadToOther.Visibility = Visibility.Visible;
            MovieItemDetail data;
            string movieId = (e.ClickedItem as MovieItem).ID;
            if (TimeAPIService.GetedDetail != null && TimeAPIService.GetedDetail.ContainsKey(movieId))
            {
                data = TimeAPIService.GetedDetail[movieId];
            }
            else
            {
                data = await TimeAPIService.GetMovieDetail(movieId);
            }
            CollectionsView.PrepareConnectedAnimation("Image", e.ClickedItem as MovieItem, "ImageMovie");
            NavigationService.Navigate(typeof(MovieDetailViewModel).FullName, data, new SuppressNavigationTransitionInfo());

        }

        public NavigationServiceEx NavigationService {
            get {
                return CommonServiceLocator.ServiceLocator.Current.GetInstance<NavigationServiceEx>();
            }
        }
    }
}

﻿using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MoviePreview.Helpers;
using MoviePreview.Models;
using MoviePreview.Services;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace MoviePreview.ViewModels
{
    public class MyCollectViewModel : ViewModelBase
    {
        private ObservableCollection<MovieItem> _collections;

        public ObservableCollection<MovieItem> Collections {
            get {
                if (_collections == null) _collections = new ObservableCollection<MovieItem>();
                return _collections;
            }
            set {
                Set(ref _collections, value);
            }
        }

        public MyCollectViewModel()
        {
            SyncData();
            // TODO 加入磁贴 我的收藏
            // 还有N天就上映/已经上映N天了
            if(Collections.Count != 0)
            {
                var theDate = DateTime.ParseExact(Collections[0].Date, "yyyy-M-d",
                                  CultureInfo.InvariantCulture);
                string tips;
                string date;
                int day = (int)(theDate - DateTime.Now).TotalDays;
                
                if (day <= 0)
                {
                    tips = "正在上映";
                    date = $"已上映{-day}天";
                }
                else
                {
                    tips = "即将上映";
                    date = $"还剩{day}天";
                }
                Singleton<LiveTileService>.Instance.AddTileToQueue("我的收藏", Collections[0].TitleCn, "", tips, date, Collections[0].Image);
                Singleton<ToastNotificationsService>.Instance.ShowToastNotificationSample();
            }
        }

        public void SaveData()
        {
            Singleton<MyCollectService>.Instance.SaveToStorage(Collections.ToList());
        }

        public void SyncData()
        {
            Collections = new ObservableCollection<MovieItem>(Singleton<MyCollectService>.Instance.Collections);
        }

        
        
        // 删除收藏
        public void OnsItemDelete(MovieItem item)
        {
            Collections.Remove(item);
            SaveData();
        }

        // 修改备注
        public void OnsItemChange(string id, string newNote)
        {
            // Nothing
        }
    }
}

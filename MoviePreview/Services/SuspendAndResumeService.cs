﻿using System;
using System.Threading.Tasks;

using CommonServiceLocator;

using MoviePreview.Activation;
using MoviePreview.Helpers;

using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace MoviePreview.Services
{
    // More details regarding the application lifecycle and how to handle suspend and resume at https://docs.microsoft.com/windows/uwp/launch-resume/app-lifecycle
    internal class SuspendAndResumeService : ActivationHandler<LaunchActivatedEventArgs>
    {
        private const string StateFilename = "SuspendAndResumeState";

        public event EventHandler<OnBackgroundEnteringEventArgs> OnBackgroundEntering;

        public async Task SaveStateAsync()
        {
            var suspensionState = new SuspensionState()
            {
                SuspensionDate = DateTime.Now
            };

            var target = OnBackgroundEntering?.Target.GetType();
            var onBackgroundEnteringArgs = new OnBackgroundEnteringEventArgs(suspensionState, target);

            OnBackgroundEntering?.Invoke(this, onBackgroundEnteringArgs);

            await ApplicationData.Current.LocalFolder.SaveAsync(StateFilename, onBackgroundEnteringArgs);
        }

        protected override async Task HandleInternalAsync(LaunchActivatedEventArgs args)
        {
            await RestoreStateAsync();
        }

        protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
        {
            return args.PreviousExecutionState == ApplicationExecutionState.Terminated;
        }

        private async Task RestoreStateAsync()
        {
            var saveState = await ApplicationData.Current.LocalFolder.ReadAsync<OnBackgroundEnteringEventArgs>(StateFilename);
            if (saveState?.Target != null)
            {
                var navigationService = ServiceLocator.Current.GetInstance<NavigationServiceEx>();
                navigationService.Navigate(saveState.Target.FullName, saveState.SuspensionState.Data);
            }
        }
    }
}

﻿//{**
//This code block adds the logic to handle SettingsItem in NavigationView control from ViewModel.
//**}
namespace Param_ItemNamespace.ViewModels
{
    public class ShellViewModel : Screen
    {
        public void OnItemInvoked(WinUI.NavigationViewItemInvokedEventArgs args)
        {
            //{[{
            if (args.IsSettingsInvoked)
            {
                _navigationService.Navigate(typeof(wts.ItemNamePage));
                return;
            }

            //}]}
        }

        private void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {
            IsBackEnabled = _navigationService.CanGoBack;
            //{[{
            if (e.SourcePageType == typeof(wts.ItemNamePage))
            {
                Selected = _navigationView.SettingsItem as WinUI.NavigationViewItem;
                return;
            }

            //}]}
        }
    }
}

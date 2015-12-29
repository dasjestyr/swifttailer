﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SwiftTailer.Wpf.Behaviors
{
    public class MenuItemExtensions : DependencyObject
    {
        public static Dictionary<MenuItem, string> ElementToGroupNames = new Dictionary<MenuItem, string>();

        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.RegisterAttached("GroupName",
                                         typeof(string),
                                         typeof(MenuItemExtensions),
                                         new PropertyMetadata(String.Empty, OnGroupNameChanged));

        public static void SetGroupName(MenuItem element, string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            element.SetValue(GroupNameProperty, value);
        }

        public static string GetGroupName(MenuItem element)
        {
            return element.GetValue(GroupNameProperty).ToString();
        }

        private static void OnGroupNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Add an entry to the group name collection
            var menuItem = d as MenuItem;

            if (menuItem == null) return;

            var newGroupName = e.NewValue.ToString();
            var oldGroupName = e.OldValue.ToString();
            if (String.IsNullOrEmpty(newGroupName))
            {
                //Removing the toggle button from grouping
                RemoveCheckboxFromGrouping(menuItem);
            }
            else
            {
                //Switching to a new group
                if (newGroupName == oldGroupName) return;

                if (!String.IsNullOrEmpty(oldGroupName))
                {
                    //Remove the old group mapping
                    RemoveCheckboxFromGrouping(menuItem);
                }
                ElementToGroupNames.Add(menuItem, e.NewValue.ToString());
                menuItem.Checked += MenuItemChecked;
            }
        }

        private static void RemoveCheckboxFromGrouping(MenuItem checkBox)
        {
            ElementToGroupNames.Remove(checkBox);
            checkBox.Checked -= MenuItemChecked;
        }


        static void MenuItemChecked(object sender, RoutedEventArgs e)
        {
            var menuItem = e.OriginalSource as MenuItem;
            foreach (var item in ElementToGroupNames)
            {
                if (!Equals(item.Key, menuItem) && item.Value == GetGroupName(menuItem))
                {
                    item.Key.IsChecked = false;
                }
            }
        }
    }
}

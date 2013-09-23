﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Yanitta.Properties;

namespace Yanitta
{
    /// <summary>
    /// Логика взаимодействия для WindowProfileEditor.xaml
    /// </summary>
    public partial class WinProfileEditor : Window
    {
        private Key[] Keys = new Key[] { Key.X, Key.Z, Key.C, Key.V, Key.B, Key.N };

        public WinProfileEditor()
        {
            InitializeComponent();
            InitialiseEmptyProfiles();
        }

        private Profile CurrentProfile
        {
            get { return (profiLeList != null && profiLeList.SelectedValue is Profile) ? (Profile)profiLeList.SelectedValue : null; }
        }

        private Ability CurrentAbility
        {
            get { return (abilityList != null && abilityList.SelectedValue is Ability) ? (Ability)abilityList.SelectedValue : null; }
        }

        private Rotation CurrentRotation
        {
            get { return (rotationList != null && rotationList.SelectedValue is Rotation) ? (Rotation)rotationList.SelectedValue : null; }
        }

        private void SetAlavilableAbilityFilter()
        {
            if (listBoxAlavilableAbilitys == null)
                return;
            var abilityView = CollectionViewSource.GetDefaultView(listBoxAlavilableAbilitys.ItemsSource);
            if (abilityView != null && CurrentRotation != null)
            {
                abilityView.Filter = new Predicate<object>((raw_ability) =>
                {
                    var ability = raw_ability as Ability;
                    if (ability == null)
                        return false;
                    return !CurrentRotation.AbilityQueue.Contains(ability.Name);
                });
            }
        }

        private void MoveRotationAbility(int shift)
        {
            var index = rotationListAbilitys.SelectedIndex;
            if (CurrentRotation != null && index > -1
                && !(shift == -1 && index == 0)
                && !(shift == 1  && index == CurrentRotation.AbilityQueue.Count - 1))
            {
                CurrentRotation.AbilityQueue.Move(index, index + shift);
            }
        }

        private void MoveAbility(int shift)
        {
            var index = abilityList.SelectedIndex;
            if (CurrentProfile != null && index > -1
                && !(shift == -1 && index == 0)
                && !(shift == 1  && index == CurrentProfile.AbilityList.Count - 1))
            {
                CurrentProfile.AbilityList.Move(index, index + shift);
                abilityList.ScrollIntoView(this.abilityList.SelectedItem);
            }
        }

        private void listBoxAlavilableAbilitys_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            bMoveTo_Click_1(null, null);
        }

        private void rotationListAbilitys_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            bMoveFrom_Click_1(null, null);
        }

        private void bMoveTo_Click_1(object sender, RoutedEventArgs e)
        {
            if (CurrentRotation != null && listBoxAlavilableAbilitys.SelectedValue != null)
            {
                var index   = listBoxAlavilableAbilitys.SelectedIndex;
                var ability = listBoxAlavilableAbilitys.SelectedValue as Ability;

                CurrentRotation.AbilityQueue.Add(ability.Name);
                SetAlavilableAbilityFilter();

                var count = listBoxAlavilableAbilitys.Items.Count;

                if (count > index)
                    listBoxAlavilableAbilitys.SelectedIndex = index;
                else if (count <= index && count > 0)
                    listBoxAlavilableAbilitys.SelectedIndex = count - 1;
            }
        }

        private void bMoveFrom_Click_1(object sender, RoutedEventArgs e)
        {
            var index = rotationListAbilitys.SelectedIndex;
            if (CurrentRotation != null && index > -1)
            {
                CurrentRotation.AbilityQueue.RemoveAt(index);
                SetAlavilableAbilityFilter();

                var count = rotationListAbilitys.Items.Count;

                if (count > index)
                    rotationListAbilitys.SelectedIndex = index;
                else if (count <= index && count > 0)
                    rotationListAbilitys.SelectedIndex = count - 1;
            }
        }

        private void bMoveUp_Click_1(object sender, RoutedEventArgs e)
        {
            MoveRotationAbility(-1);
        }

        private void bMoveDown_Click_1(object sender, RoutedEventArgs e)
        {
            MoveRotationAbility(1);
        }

        private void rotationList_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            SetAlavilableAbilityFilter();
        }

        private void InitialiseEmptyProfiles()
        {
            foreach (WowClass wowClass in Enum.GetValues(typeof(WowClass)))
            {
                if (!ProfileDb.Instance.ProfileList.Any(n => n.Class == wowClass))
                    ProfileDb.Instance.ProfileList.Add(new Profile() { Class = wowClass });
            }
        }

        private void bAbMoveUp_Click_1(object sender, RoutedEventArgs e)
        {
            MoveAbility(-1);
        }

        private void bAbMoveDown_Click_1(object sender, RoutedEventArgs e)
        {
            MoveAbility(1);
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ProfileDb.Instance.Save(Settings.Default.ProfilesFileName);
        }

        #region Commands

        // ability
        private void CommandBinding_Executed_AddAbility(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.CurrentProfile != null)
            {
                this.CurrentProfile.AbilityList.Add(new Ability());
                this.abilityList.SelectedIndex = this.CurrentProfile.AbilityList.Count - 1;
                this.tbAbilityName.Focus();
                this.tbAbilityName.SelectAll();
                abilityList.ScrollIntoView(this.abilityList.SelectedItem);
            }
            SetAlavilableAbilityFilter();
        }

        private void CommandBinding_Executed_CopyAbility(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.CurrentAbility != null)
            {
                this.CurrentProfile.AbilityList.Add((Ability)this.CurrentAbility.Clone());
                this.abilityList.SelectedIndex = this.CurrentProfile.AbilityList.Count - 1;
                this.tbAbilityName.Focus();
                this.tbAbilityName.SelectAll();
                this.abilityList.ScrollIntoView(this.abilityList.SelectedItem);
            }
            SetAlavilableAbilityFilter();
        }

        private void CommandBinding_Executed_DeleteAbility(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.CurrentAbility != null)
            {
                this.CurrentProfile.AbilityList.Remove(this.CurrentAbility);
            }
            SetAlavilableAbilityFilter();
        }

        // rotations
        private void CommandBinding_Executed_AddRotation(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.CurrentProfile != null)
            {
                var rotation = new Rotation();
                if (CurrentProfile.RotationList.Count < Keys.Length)
                    rotation.HotKey = new HotKey(Keys[CurrentProfile.RotationList.Count], ModifierKeys.Alt);
                this.CurrentProfile.RotationList.Add(rotation);
                this.rotationList.SelectedIndex = this.CurrentProfile.RotationList.Count - 1;
                this.tbRotationName.Focus();
                this.tbRotationName.SelectAll();
                this.rotationList.ScrollIntoView(this.rotationList.SelectedItem);
            }
            SetAlavilableAbilityFilter();
        }

        private void CommandBinding_Executed_CopyRotation(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.CurrentRotation != null)
            {
                this.CurrentProfile.RotationList.Add((Rotation)this.CurrentRotation.Clone());
                this.rotationList.SelectedIndex = this.CurrentProfile.RotationList.Count - 1;
                this.tbRotationName.Focus();
                this.tbRotationName.SelectAll();
                this.rotationList.ScrollIntoView(this.rotationList.SelectedItem);
            }
            SetAlavilableAbilityFilter();
        }

        private void CommandBinding_Executed_DeleteRotation(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.CurrentRotation != null)
                this.CurrentProfile.RotationList.Remove(this.CurrentRotation);
            SetAlavilableAbilityFilter();
        }

        private void CommandBinding_Executed_RefreshRotation(object sender, ExecutedRoutedEventArgs e)
        {
            if (CurrentProfile != null && this.CurrentRotation != null)
                for (int i = this.CurrentRotation.AbilityQueue.Count - 1; i >= 0; --i)
                    if (!this.CurrentProfile.AbilityList.Any(a => a.Name == this.CurrentRotation.AbilityQueue[i]))
                        this.CurrentRotation.AbilityQueue.RemoveAt(i);
        }

        // other
        private void CommandBinding_Executed_Import(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.CurrentProfile != null)
            {
                var dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.Filter = "Profile file (*.xmlp)|*.xmlp|All Files (*.*)|*.*";
                if (dialog.ShowDialog() == true)
                {
                    try
                    {
                        var profile = new XmlManager(dialog.FileName).Load<Profile>();
                        for (int i = 0; i < ProfileDb.Instance.ProfileList.Count; ++i)
                        {
                            if (ProfileDb.Instance.ProfileList[i].Class == profile.Class)
                            {
                                ProfileDb.Instance.ProfileList[i] = profile;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void CommandBinding_Executed_Export(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.CurrentProfile != null)
            {
                var dialog = new Microsoft.Win32.SaveFileDialog();
                dialog.Filter = "Profile file (*.xmlp)|*.xmlp|All Files (*.*)|*.*";
                dialog.FileName = string.Format("{0}_profile.xmlp", this.CurrentProfile.Class);
                if (dialog.ShowDialog() == true)
                {
                    try
                    {
                        new XmlManager(dialog.FileName).Save<Profile>(this.CurrentProfile);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void CommandBinding_Executed_Update(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ProfileDb.UpdateProfiles();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CommandBinding_Executed_Reload(object sender, ExecutedRoutedEventArgs e)
        {
            ProfileDb.Instance.Load(Settings.Default.ProfilesFileName);
        }

        private void CommandBinding_Executed_Save(object sender, ExecutedRoutedEventArgs e)
        {
            ProfileDb.Instance.Save(Settings.Default.ProfilesFileName, true);
            if (App.MainWindow is MainWindow)
                App.MainWindow.TaskbarIcon.ShowBalloonTip("Yanitta", "Profiles saved!", Microsoft.Windows.Controls.BalloonIcon.Info);
        }

        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            this.Close();
        }

        #endregion Commands
    }
}
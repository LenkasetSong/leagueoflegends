﻿using System;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using Leagueoflegends.Data.Main;
using Leagueoflegends.Foundation.Mvvm;
using Leagueoflegends.Foundation.Riotcore;
using Leagueoflegends.Main.Local.Collection;
using Leagueoflegends.Main.Local.Work;
using Leagueoflegends.Main.UI.Units;
using Leagueoflegends.MyShop.UI.Views;
using Leagueoflegends.MyShop.Local.ViewModel;
using Leagueoflegends.TeamFight.UI.Views;
using Leagueoflegends.TeamFight.Local.ViewModel;
using Leagueoflegends.DBEntity.Local.Api;
using Leagueoflegends.Home.UI.Views;
using Leagueoflegends.Home.Local.ViewModels;
using Leagueoflegends.Store.UI.Views;
using Leagueoflegends.Store.Local.ViewModels;
using Leagueoflegends.Clash.UI.Views;
using Leagueoflegends.Clash.Local.ViewModel;

namespace Leagueoflegends.Main.Local.ViewModel
{
	public class MainViewModel : ObservableObject
    {
        #region Variables

        private readonly WindowWork _winWork;
        private readonly ModalWork _modalWork;

        private object _modalContent;
        private IRiotUI _currentUI;
        private List<SubMenuModel> _subMenus;
        private SubMenuModel _currentSubMenu;
        private MainMenuModel _mainMenu;
        private FriendsCollection _friends;
        private int _currentSeq;

        private Dictionary<int, IRiotUI> UIs { get; set; }
        #endregion

        #region ViewModels

        public MenuWork MainMenu { get; }
        public FriendsSortWork Options { get; }
        #endregion

        #region ICommands

        public ICommand ModalCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand MinimizeCommand { get; }
        #endregion

        #region ModalContent

        public object ModalContent
        {
            get => _modalContent;
            set { _modalContent = value; OnPropertyChanged(); }
        }
        #endregion

        #region CurrentUI

        public IRiotUI CurrentUI
        {
            get => _currentUI;
            set { _currentUI = value; OnPropertyChanged(); }
        }
        #endregion

        #region CurrentSeq

        public int CurrentSeq
		{
            get => _currentSeq;
			set { _currentSeq = value; OnPropertyChanged(); }
		}
		#endregion

		#region SubMenus

		public SubMenuModel CurrentSubMenu
        {
            get => _currentSubMenu;
            set { _currentSubMenu = value; OnPropertyChanged(); SubMenuChanged(value); }
        }

        public List<SubMenuModel> SubMenus
        {
            get => _subMenus;
            set { _subMenus = value; OnPropertyChanged(); }
        }
        #endregion

        #region Friends

        public FriendsCollection Friends
        {
            get => _friends;
            set { _friends = value; OnPropertyChanged(); }
        }
        #endregion

        #region Constructor

        public MainViewModel()
        {
            _winWork = new(this);
            _modalWork = new(this);

            UIs = new();

            CloseCommand = new RelayCommand<object>(_winWork.DoClosing);
            MinimizeCommand = new RelayCommand<object>(_winWork.DoMinizing);
            ModalCommand = new RelayCommand<Type>(_modalWork.SwitchModal);

            MainMenu = new(MenuSelected);
            Options = new();

            var friends = new GetFriends().Run(0);
            Friends = new(friends);
        }
        #endregion

        // Private

        #region MenuSelected

        private void MenuSelected(MainMenuModel menu, List<SubMenuModel> subMenus)
        {
            _mainMenu = menu;
            SubMenus = subMenus;
            CurrentSubMenu = SubMenus.FirstOrDefault();
        }
        #endregion

        #region SubMenuChanged

        private void SubMenuChanged(SubMenuModel value)
        {
            IRiotUI content;
            int key;

            if (value != null)
            {
                key = value.Seq;
                content = value.Seq switch
                {
                    8 => new Overview().SetVM(new OverviewModel()),
                    21 => new Champions().SetVM(new ChampionsViewModel()),
                    //추후에 HubView로 변경 필요
                    26 => new ClashView().SetVM(new ClashViewModel()),
                    _ => new EmptyContent()
                };
            }
            else
            {
                key = _mainMenu.Seq;
                content = _mainMenu.Seq switch
                {
                    1 => new TeamFightView().SetVM(new TeamFightViewModel()),
                    6 => new MyShopView().SetVM(new MyShopViewModel()),
                    _ => new EmptyContent()
                };
            }

            if (!UIs.ContainsKey(key))
            {
                UIs.Add(key, content);
            }

            CurrentUI = UIs[key];
            CurrentSeq = key;
        }
        #endregion
    }
}
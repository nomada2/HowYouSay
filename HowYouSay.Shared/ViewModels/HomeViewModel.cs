﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CodeMill.VMFirstNav;
using HowYouSay.Models;
using HowYouSay.Pages;
using MvvmHelpers;
using Realms;
using Xamarin.Forms;

namespace HowYouSay.ViewModels
{
	public class HomeViewModel : BaseViewModel, IViewModel
	{
		private Realm _realm;

		public Action Changed { get; set; }

		public IQueryable<VocabEntry> Entries { get; private set; }

		public ICommand NavToAddCommand { get; private set; }

		public ICommand NavToMenuCommand { get; private set; }

		public ICommand ToggleCommand { get; private set; }

		public ICommand DeleteEntryCommand { get; private set; }

		public ICommand SearchCommand { get; private set; }

		public INavigation Navigation { get; set; }

		public INavigationService _navService { get; set; }

		private bool _isFullTabSelected = true;
		public bool IsFullTabSelected
		{
			get
			{
				return _isFullTabSelected;
			}
			set
			{
				SetProperty(ref _isFullTabSelected, value, onChanged: Changed);
				OnPropertyChanged(nameof(IsBookmarkedTabSelected));
			}
		}

		public bool IsBookmarkedTabSelected
		{
			get
			{
				return !_isFullTabSelected;
			}
		}

		public HomeViewModel()
		{
			IsBusy = false;

			_navService = NavigationService.Instance;


			NavToAddCommand = new Command(AddEntry);
			DeleteEntryCommand = new Command<VocabEntry>(DeleteEntry);
			NavToMenuCommand = new Command(GoToMenu);
			ToggleCommand = new Command<string>(Toggle);
			SearchCommand = new Command(OpenSearch);


		}

		async Task ConnectToRealm()
		{
			IsBusy = true;

			_realm = Realm.GetInstance();

			Entries = _realm.All<VocabEntry>();

			OnPropertyChanged(nameof(Entries));
		}

		private async void AddEntry()
		{
			//var entry = new VocabEntry
			//{
			//	Metadata = new EntryMetadata
			//	{
			//		Date = DateTimeOffset.Now
			//	}
			//};

			//var id = entry.Id;

			//_realm.Write(() =>
			//{
			//	_realm.Add<VocabEntry>(entry);
			//});

			try
			{
				
				await _navService.PushAsync<VocabEntryDetailsViewModel>();
				//var page = new VocabEntryDetailsPage{
				//	EntryId = entry.Id
				//};
				//await Navigation.PushAsync(page);
			}
			catch (Exception ex)
			{
				//App.ShowMessageBox("An error occred navigating to the Job List page", "Navigation Failed!");
				System.Diagnostics.Debug.WriteLine("Navigation failed " + ex.Message);
			};


		}

		private void OpenSearch()
		{

		}

		private void GoToMenu()
		{
			//var page = new VocabEntryDetailsPage(new VocabEntryDetailsViewModel(entry));

			//Navigation.PushAsync(page);
		}

		private void Toggle(string destination)
		{

			IsFullTabSelected = (destination == ListTabs.FULL);
		}

		internal async void EditEntry(VocabEntry entry)
		{
			await _navService.PushAsync<VocabEntryDetailsViewModel>((vm) => vm.SetEntry(entry.Id));
			//var page = new VocabEntryDetailsPage{
			//	EntryId = entry.Id
			//};

			//Navigation.PushAsync(page);
		}

		private void DeleteEntry(VocabEntry entry)
		{
			_realm.Write(() => _realm.Remove(entry));
		}

		public void OnAppearing()
		{
			ConnectToRealm().ContinueWith(task =>
			{
				IsBusy = false;
				if (task.Exception != null)
				{/* error */}
			});
		}
	}

	static class ListTabs
	{
		public const string FULL = "full";
		public const string BOOKMARKED = "bookmarked";
	}
}
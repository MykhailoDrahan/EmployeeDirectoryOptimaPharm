using EmployeeDirectoryOptimaPharm.Data;
using EmployeeDirectoryOptimaPharm.Models;
using EmployeeDirectoryOptimaPharm.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EmployeeDirectoryOptimaPharm.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IEmployeeService _employeeService;
        private readonly IJsonDataService _jsonDataService;
        private readonly IDBFulfillService _dBFulfillService;
        private BaseViewModel _currentViewModel;
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }
        public MainWindowViewModel(IUserService userService, IEmployeeService employeeService, IJsonDataService jsonDataService, IDBFulfillService dBFulfillService)
        {
            _userService = userService;
            _employeeService = employeeService;
            _jsonDataService = jsonDataService;
            _dBFulfillService = dBFulfillService;
            _ = InitAsync();

        }

        public async Task InitAsync()
        {
            bool userExists = await _userService.IsAnyAsync();
            if (!userExists)
            {
                ShowRegisterView();
                var result = MessageBox.Show(
                "Do you want to fullfil DB with random employees?",
                "Fullfil DB",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _dBFulfillService.FullfilDB();
                }
            }
            else
            {
                ShowLoginView();
            }
        }

        public void ShowRegisterView()
        {
            CurrentViewModel = new RegisterViewModel(this.ShowLoginView, _userService);
        }

        public void ShowLoginView()
        {
            CurrentViewModel = new LoginViewModel(this.ShowEmployeeListView, _userService);
        }

        public void ShowEmployeeListView()
        {
            CurrentViewModel = new EmployeeListViewModel(this.ShowAddUserView, _employeeService, _jsonDataService);
        }

        public void ShowAddUserView()
        {
            CurrentViewModel = new AddEmployeeViewModel(this.ShowEmployeeListView, _employeeService);
        }
    }
}

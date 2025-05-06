using EmployeeDirectoryOptimaPharm.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using EmployeeDirectoryOptimaPharm.Data;
using EmployeeDirectoryOptimaPharm.Models;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeDirectoryOptimaPharm.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private Action _navigateAction;

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value.ToUpper();
                OnPropertyChanged();
            }
        }
        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand LoginCommand => new RelayCommand(execute => ToLogin(), canExecute => CanLogin());
        public LoginViewModel(Action navigateAction, IUserService userService)
        {
            _navigateAction = navigateAction;
            _userService = userService;
        }

        private async Task ToLogin()
        {
            bool isLogined = await _userService.LoginAsync(Username, Password);
            if(isLogined)
            {
                _navigateAction?.Invoke();
            }
        }

        private bool CanLogin()
        {
            return !Username.IsNullOrEmpty() && !Password.IsNullOrEmpty();
        }
    }
}

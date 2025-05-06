using EmployeeDirectoryOptimaPharm.Data;
using EmployeeDirectoryOptimaPharm.MVVM;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDirectoryOptimaPharm.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private Action _navigateAction;
        private IUserService _userService;

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
        public RelayCommand RegistrationCommand => new RelayCommand(execute => ToRegister(), canExecute => CanRegister());
        public RegisterViewModel(Action navigateAction, IUserService userService)
        {
            _navigateAction = navigateAction;
            _userService = userService;
        }
        private async Task ToRegister()
        {
            bool isRegistred = await _userService.RegisterAsync(Username,Password);
            if(isRegistred)
            {
                _navigateAction?.Invoke();
            }
        }
        private bool CanRegister()
        {
            return !Username.IsNullOrEmpty()&&!Password.IsNullOrEmpty();
        }

    }
}

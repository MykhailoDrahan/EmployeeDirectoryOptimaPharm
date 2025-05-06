using EmployeeDirectoryOptimaPharm.Data;
using EmployeeDirectoryOptimaPharm.Models;
using EmployeeDirectoryOptimaPharm.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace EmployeeDirectoryOptimaPharm.ViewModels
{
    public class EmployeeListViewModel : BaseViewModel
    {
        private readonly IEmployeeService _employeeService;
        private readonly IJsonDataService _jsonDataService;
        public ObservableCollection<Employee> FilteredEmployees { get; private set; } = new ObservableCollection<Employee>();
        private List<Employee> _allEmployees;
        private Action _navigateAddAction;
        public EmployeeListViewModel(Action navigateAddAction, IEmployeeService employeeService, IJsonDataService jsonDataService)
        {
            _navigateAddAction = navigateAddAction;
            _employeeService = employeeService;
            _jsonDataService = jsonDataService;
            FilteredEmployees.CollectionChanged += (s, e) => RaiseStatistics();
            _ = LoadEmployeesAsync();

        }
        private async Task LoadEmployeesAsync()
        {
            _allEmployees = await _employeeService.GetEmployeesAsync();
            FilteredEmployees.Clear();
            foreach (var employee in _allEmployees)
            {
                FilteredEmployees.Add(employee);
            }
        }

        private Employee? _selectedEmployee;
        public Employee? SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                if (_selectedEmployee != value)
                {
                    _selectedEmployee = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanTerminate));
                    OnPropertyChanged(nameof(MinTemrinateDate));
                    IsEnabledCheck = false;
                }
            }
        }
        private string _searchField;
        public string SearchField
        {
            get => _searchField;
            set
            {
                if (_searchField != value)
                {
                    _searchField = value;
                    OnPropertyChanged();
                    FilterEmployeesSuggestions();
                }
            }
        }

        private int _employeesCount;
        public int EmployeesCount
        {
            get => _employeesCount;
            private set
            {
                if (_employeesCount != value)
                {
                    _employeesCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _employeesWorkingCount;
        public int EmployeesWorkingCount
        {
            get => _employeesWorkingCount;
            private set
            {
                if (_employeesWorkingCount != value)
                {
                    _employeesWorkingCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _employeesFinishedCount;
        public int EmployeesFinishedCount
        {
            get => _employeesFinishedCount;
            private set
            {
                if (_employeesFinishedCount != value)
                {
                    _employeesFinishedCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _peopleCount;
        public int PeopleCount
        {
            get => _peopleCount;
            private set
            {
                if (_peopleCount != value)
                {
                    _peopleCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _peopleWorkingCount;
        public int PeopleWorkingCount
        {
            get => _peopleWorkingCount;
            private set
            {
                if (_peopleWorkingCount != value)
                {
                    _peopleWorkingCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _peopleFinishedCount;
        public int PeopleFinishedCount
        {
            get => _peopleFinishedCount;
            private set
            {
                if (_peopleFinishedCount != value)
                {
                    _peopleFinishedCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal _medianSalary;
        public decimal MedianSalary
        {
            get => _medianSalary;
            private set
            {
                if (_medianSalary != value)
                {
                    _medianSalary = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal _minSalary;
        public decimal MinSalary
        {
            get => _minSalary;
            private set
            {
                if (_minSalary != value)
                {
                    _minSalary = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal _maxSalary;
        public decimal MaxSalary
        {
            get => _maxSalary;
            private set
            {
                if (_maxSalary != value)
                {
                    _maxSalary = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal _averageSalary;
        public decimal AverageSalary
        {
            get => _averageSalary;
            private set
            {
                if (_averageSalary != value)
                {
                    _averageSalary = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isEnabledChaeck;
        public bool IsEnabledCheck
        {
            get => _isEnabledChaeck;
            set
            {
                if (_isEnabledChaeck != value)
                {
                    _isEnabledChaeck = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged();
                    SelectedEmployee.EndDate = EndDate;
                }
            }
        }

        public DateTime MinTemrinateDate
        {
            get
            {
                if (SelectedEmployee == null)
                    return DateTime.Today;

                return SelectedEmployee.StartDate > DateTime.Today ? SelectedEmployee.StartDate : DateTime.Today;
            }
        }

        public bool CanTerminate => SelectedEmployee != null;

        private void RaiseStatistics()
        {
            EmployeesCount = FilteredEmployees.Count();
            EmployeesWorkingCount = FilteredEmployees.Where(e => !e.IsFinished).Count();
            EmployeesFinishedCount = FilteredEmployees.Where(e => e.IsFinished).Count();
            PeopleCount = FilteredEmployees.Select(e => e.Person).Distinct().Count();
            PeopleWorkingCount = FilteredEmployees.GroupBy(e => e.PersonId).Where(group => group.Any(e => !e.IsFinished)).Count();
            PeopleFinishedCount = FilteredEmployees.GroupBy(e => e.PersonId).Where(group => group.All(e => e.IsFinished)).Count();
            MedianSalary = GetMedianSalary();
            MinSalary = FilteredEmployees.Any() ? FilteredEmployees.Min(e => e.Salary) : 0.00m;
            MaxSalary = FilteredEmployees.Any() ? FilteredEmployees.Max(e => e.Salary) : 0.00m;
            AverageSalary = FilteredEmployees.Any() ? Math.Round(FilteredEmployees.Average(e => e.Salary), 2) : 0.00m;
        }

        private void FilterEmployeesSuggestions()
        {
            var filtered = _allEmployees
                .Where(e => string.IsNullOrWhiteSpace(SearchField)
                        || e.Position.Name.Contains(SearchField, StringComparison.InvariantCultureIgnoreCase)
                        || e.Person.LastName.Contains(SearchField, StringComparison.InvariantCultureIgnoreCase)
                        || e.Person.FirstName.Contains(SearchField, StringComparison.InvariantCultureIgnoreCase)
                        || e.Person.MiddleName.Contains(SearchField, StringComparison.InvariantCultureIgnoreCase)
                        || e.Person.PersonIdentityDocument.DocumentNumber.Contains(SearchField, StringComparison.InvariantCultureIgnoreCase)
                        || e.Person.TaxIdentifier.Number.Contains(SearchField, StringComparison.InvariantCultureIgnoreCase)
                        ).ToList();
            FilteredEmployees.Clear();
            foreach (var item in filtered)
                FilteredEmployees.Add(item);
        }

        private decimal GetMedianSalary()
        {
            var salaries = FilteredEmployees
                .Select(e => e.Salary)
                .OrderBy(s => s)
                .ToList();
            if (salaries.Count == 0)
                return 0.00m;
            int middle = salaries.Count / 2;
            if (salaries.Count % 2 == 0)
            {
                return (salaries[middle - 1] + salaries[middle]) / 2;
            }
            else
            {
                return salaries[middle];
            }
        }

        private async Task ImportFilteredEmployees()
        {
            var result = await _jsonDataService.ImportEmployeesFromJsonAsync();

            if (result.Success)
            {
                await _employeeService.AddEmployeesAsync(result.Data);
                await LoadEmployeesAsync();
                MessageBox.Show($"Imported {result.Data.Count} employees from:\n{result.FilePath}", "Import Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Import failed:\n{result.ErrorMessage}", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async Task ExportFilteredEmployees()
        {
            var result = await _jsonDataService.ExportEmployeesToJsonAsync(FilteredEmployees.ToList());
            if (result.Success)
            {
                MessageBox.Show($"File saved at:\n{result.FilePath}", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Export was cancelled or failed.", "Export Cancelled", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private async Task TerminateEmployeeAsync()
        {
            await _employeeService.TerminateEmployeeAsync(SelectedEmployee);
            await LoadEmployeesAsync();
        }

        private async Task TerminatePersonAsync()
        {
            await _employeeService.TerminatePersonAsync(SelectedEmployee.Person, EndDate.Value);
            await LoadEmployeesAsync();
        }
        public RelayCommand AddUserCommand => new RelayCommand(execute => _navigateAddAction?.Invoke());

        public RelayCommand ImportFilteredEmployeesComand => new RelayCommand(execute => ImportFilteredEmployees());
        public RelayCommand ExportFilteredEmployeesComand => new RelayCommand(execute => _ = ExportFilteredEmployees(), canExecute => FilteredEmployees.Any());
        public RelayCommand TerminateEmployeeCommand => new RelayCommand(execute => _ = TerminateEmployeeAsync(), canExecute => IsEnabledCheck && EndDate != null);
        public RelayCommand TerminatePersonCommand => new RelayCommand(execute => _ = TerminatePersonAsync(), canExecute => IsEnabledCheck && EndDate != null);
                
    }
}

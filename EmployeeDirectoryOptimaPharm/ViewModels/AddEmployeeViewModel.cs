using EmployeeDirectoryOptimaPharm.Data;
using EmployeeDirectoryOptimaPharm.Models;
using EmployeeDirectoryOptimaPharm.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace EmployeeDirectoryOptimaPharm.ViewModels
{
    public class AddEmployeeViewModel : BaseViewModel, IDataErrorInfo
    {
        private Action _backAction;
        private readonly IEmployeeService _employeeService;
        private bool _flagPosition = true;
        private bool _flagPerson = true;
        private bool _autofilledPersonTaxIdentifier;
        private bool _autofilledPersonIdentityDocument;

        private Position _selectedPosition;
        public Position SelectedPosition
        {
            get => _selectedPosition;
            set
            {
                if (_selectedPosition != value && _flagPosition)
                {
                    _selectedPosition = value;
                    OnPropertyChanged();
                    PositionName = _selectedPosition.Name;
                }
            }
        }

        private string _positionName = string.Empty;
        public string PositionName
        {
            get => _positionName.Trim();
            set
            {
                if (_positionName != value && _flagPosition)
                {
                    _positionName = value;
                    OnPropertyChanged();
                    SelectedPosition = _allPositions.Find(p => p.Name == _positionName) ?? new Position { Name = _positionName };
                    FilterPositionsSuggestions();
                    IsDropDownOpenPosition = FilteredPositions.Any();
                }
            }
        }

        private string _employmentRate = string.Empty;
        public string EmploymentRate
        {
            get => _employmentRate.Trim();
            set
            {
                if(value==string.Empty)
                {
                    _employmentRate = value;
                }
                if (double.TryParse(value, out var result) && result >= 0 && result <= 99.99)
                {
                    int dotIndex = value.IndexOf('.');
                    if (dotIndex >= 0 && value.Length > dotIndex + 3)
                    {
                        value = value.Substring(0, dotIndex + 3);
                    }
                    _employmentRate = value;
                }
                OnPropertyChanged();
            }
        }

        private string _salary = string.Empty;
        public string Salary
        {
            get => _salary.Trim();
            set
            {
                if (value == string.Empty)
                {
                    _salary = value;
                }
                if (double.TryParse(value, out var result) && result >= 0)
                {
                    int dotIndex = value.IndexOf('.');
                    if (dotIndex >= 0 && value.Length > dotIndex + 3)
                    {
                        value = value.Substring(0, dotIndex + 3);
                    }
                    _salary = value;
                }
                OnPropertyChanged();
            }
        }

        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(EndDate));
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
                }
            }
        }

        private TaxIdentifier _selectedTaxIdentifier;
        public TaxIdentifier SelectedTaxIdentifier
        {
            get => _selectedTaxIdentifier;
            set
            {
                if (_selectedTaxIdentifier != value && _flagPerson)
                {
                    _selectedTaxIdentifier = value;
                    OnPropertyChanged();
                    TaxIdentifierNumber = _selectedTaxIdentifier?.Number ?? TaxIdentifierNumber;
                }
            }
        }

        private string _taxIdentifierNumber = string.Empty;
        public string TaxIdentifierNumber
        {
            get => _taxIdentifierNumber.Trim();
            set
            {
                if (_taxIdentifierNumber != value && _flagPerson)
                {
                    _taxIdentifierNumber = value;
                    OnPropertyChanged();
                    AutofillPersonTaxIdentifier();
                    FilterPeopleSuggestions();
                    IsDropDownOpenTaxIdentifier = FilteredPeople.Any();
                }
            }
        }

        private PersonIdentityDocument _selectedIdentityDocument;
        public PersonIdentityDocument SelectedIdentityDocument
        {
            get => _selectedIdentityDocument;
            set
            {
                if (_selectedIdentityDocument != value && _flagPerson)
                {
                    _selectedIdentityDocument = value;
                    OnPropertyChanged();
                    IdentityDocumentNumber = _selectedIdentityDocument?.DocumentNumber ?? IdentityDocumentNumber;
                }
            }
        }

        private string _identityDocumentNumber = string.Empty;
        public string IdentityDocumentNumber
        {
            get => _identityDocumentNumber.Trim();
            set
            {
                if (_identityDocumentNumber != value && _flagPerson)
                {
                    _identityDocumentNumber = value.ToUpper();
                    OnPropertyChanged();
                    AutofillPersonIdentityDocument();
                    FilterPeopleSuggestions();
                    IsDropDownOpenIdentityDocument = FilteredPeople.Any();
                }
            }
        }

        private string _lastName = string.Empty;
        public string LastName
        {
            get => _lastName.Trim();
            set
            {
                if (_lastName != value && _flagPerson)
                {
                    _lastName = value != string.Empty ? char.ToUpper(value[0]) + value.Substring(1).ToLower() : string.Empty;
                    OnPropertyChanged();
                    FilterPeopleSuggestions();
                    IsDropDownOpenLastName = FilteredPeople.Any();
                }
            }
        }

        private string _firstName = string.Empty;
        public string FirstName
        {
            get => _firstName.Trim();
            set
            {
                if (_firstName != value && _flagPerson)
                {
                    _firstName = value != string.Empty ? char.ToUpper(value[0]) + value.Substring(1).ToLower() : string.Empty;
                    OnPropertyChanged();
                    FilterPeopleSuggestions();
                    IsDropDownOpenFirstName = FilteredPeople.Any();
                }
            }
        }

        private string _middleName = string.Empty;
        public string MiddleName
        {
            get => _middleName.Trim();
            set
            {
                if (_middleName != value && _flagPerson)
                {
                    _middleName = value != string.Empty ? char.ToUpper(value[0]) + value.Substring(1).ToLower() : string.Empty;
                    OnPropertyChanged();
                    FilterPeopleSuggestions();
                    IsDropDownOpenMiddleName = FilteredPeople.Any();
                }
            }
        }


        public ObservableCollection<Position> FilteredPositions { get; set; } = new ObservableCollection<Position>();
        public ObservableCollection<Person> FilteredPeople { get; set; } = new ObservableCollection<Person>();
        public ObservableCollection<string> FilteredLastName { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> FilteredFirstName { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> FilteredMiddleName { get; set; } = new ObservableCollection<string>();

        private List<Position> _allPositions;

        private List<Person> _allPeople;

        public AddEmployeeViewModel(Action backAction, IEmployeeService employeeService)
        {
            _backAction = backAction;
            _employeeService = employeeService;
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            _allPositions = await _employeeService.GetPositionsAsync();
            foreach (var position in _allPositions)
            {
                FilteredPositions.Add(position);
            }
            _allPeople = await _employeeService.GetPeopleAsync();
            foreach(var person in _allPeople)
            {
                FilteredPeople.Add(person);
            }
            foreach (var item in _allPeople.Select(person => person.LastName).Distinct().OrderBy(name => name))
            {
                FilteredLastName.Add(item);
            }
            foreach (var item in _allPeople.Select(person => person.FirstName).Distinct().OrderBy(name => name))
            {
                FilteredFirstName.Add(item);
            }
            foreach (var item in _allPeople.Select(person => person.MiddleName).Distinct().OrderBy(name => name))
            {
                FilteredMiddleName.Add(item);
            }
        }

        private bool _isDropDownOpenPosition;
        public bool IsDropDownOpenPosition
        {
            get => _isDropDownOpenPosition;
            set
            {
                _isDropDownOpenPosition = FilteredPositions.Any() ? value : false;
                OnPropertyChanged();
            }
        }

        private bool _isDropDownOpenTaxIdentifier;
        public bool IsDropDownOpenTaxIdentifier
        {
            get => _isDropDownOpenTaxIdentifier;
            set
            {
                _isDropDownOpenTaxIdentifier = FilteredPeople.Any() ? value && IsEnableTaxIdentifier : false;
                OnPropertyChanged();
            }
        }

        private bool _isDropDownOpenIdentityDocument;
        public bool IsDropDownOpenIdentityDocument
        {
            get => _isDropDownOpenIdentityDocument;
            set
            {
                _isDropDownOpenIdentityDocument = FilteredPeople.Any() ? value && IsEnableIdentityDocument : false;
                OnPropertyChanged();
            }
        }

        private bool _isDropDownOpenLastName;
        public bool IsDropDownOpenLastName
        {
            get => _isDropDownOpenLastName;
            set
            {
                _isDropDownOpenLastName = FilteredPeople.Any() ? value && IsEnablePerson : false;
                OnPropertyChanged();
            }
        }

        private bool _isDropDownOpenFirstName;
        public bool IsDropDownOpenFirstName
        {
            get => _isDropDownOpenFirstName;
            set
            {
                _isDropDownOpenFirstName = FilteredPeople.Any() ? value && IsEnablePerson : false;
                OnPropertyChanged();
            }
        }

        private bool _isDropDownOpenMiddleName;
        public bool IsDropDownOpenMiddleName
        {
            get => _isDropDownOpenMiddleName;
            set
            {
                _isDropDownOpenMiddleName = FilteredPeople.Any() ? value && IsEnablePerson : false;
                OnPropertyChanged();
            }
        }

        private bool _isEnableIdentityDocument = true;
        public bool IsEnableIdentityDocument
        {
            get => _isEnableIdentityDocument;
            private set
            {
                if (_isEnableIdentityDocument != value)
                {
                    _isEnableIdentityDocument = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isEnableTaxIdentifier = true;
        public bool IsEnableTaxIdentifier
        {
            get => _isEnableTaxIdentifier;
            private set
            {
                if (_isEnableTaxIdentifier != value)
                {
                    _isEnableTaxIdentifier = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isEnablePerson = true;
        public bool IsEnablePerson
        {
            get => _isEnablePerson;
            private set
            {
                if (_isEnablePerson != value)
                {
                    _isEnablePerson = value;
                    OnPropertyChanged();
                }
            }
        }
        private void FilterPeopleSuggestions()
        {
            var filtered = _allPeople
                .Where(person => person.TaxIdentifier.Number.Contains(TaxIdentifierNumber, StringComparison.InvariantCultureIgnoreCase)
                && person.PersonIdentityDocument.DocumentNumber.Contains(IdentityDocumentNumber, StringComparison.InvariantCultureIgnoreCase)
                && person.LastName.Contains(LastName, StringComparison.InvariantCultureIgnoreCase)
                && person.FirstName.Contains(FirstName, StringComparison.InvariantCultureIgnoreCase)
                && person.MiddleName.Contains(MiddleName, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            _flagPerson = false;
            FilteredPeople.Clear();
            foreach (var item in filtered)
                FilteredPeople.Add(item);
            FilteredLastName.Clear();
            foreach (var item in filtered.Select(person => person.LastName).Distinct().OrderBy(name => name))
                FilteredLastName.Add(item);
            FilteredFirstName.Clear();
            foreach (var item in filtered.Select(person => person.FirstName).Distinct().OrderBy(name => name))
                FilteredFirstName.Add(item);
            FilteredMiddleName.Clear();
            foreach (var item in filtered.Select(person => person.MiddleName).Distinct().OrderBy(name => name))
                FilteredMiddleName.Add(item);
            _flagPerson = true;
        }

        private void FilterPositionsSuggestions()
        {
            var filtered = _allPositions
                .Where(p => string.IsNullOrWhiteSpace(PositionName)
                         || p.Name.Contains(PositionName, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            _flagPosition = false;
            FilteredPositions.Clear();
            foreach (var item in filtered)
                FilteredPositions.Add(item);
            _flagPosition = true;
        }

        private void AutofillPersonTaxIdentifier()
        {
            Person? person = _allPeople.Find(p => p.TaxIdentifier.Number == TaxIdentifierNumber);
            if(person != null)
            {
                if (IsEnableTaxIdentifier)
                {
                    IsEnableIdentityDocument = false;
                    IsEnablePerson = false;
                }
                SelectedIdentityDocument = person.PersonIdentityDocument;
                SelectedTaxIdentifier = person.TaxIdentifier;
                LastName = person.LastName;
                FirstName = person.FirstName;
                MiddleName = person.MiddleName;
                _autofilledPersonTaxIdentifier = true;
            }
            else
            {
                IsEnableIdentityDocument = true;
                IsEnablePerson = true;
                if (Regex.IsMatch(TaxIdentifierNumber, @"^\d{10}$"))
                {
                    SelectedTaxIdentifier = new TaxIdentifier { Number = _taxIdentifierNumber };
                }
                else
                {
                    if(_autofilledPersonTaxIdentifier)
                    {
                        _autofilledPersonTaxIdentifier = false;
                        IdentityDocumentNumber = string.Empty;
                    }
                    SelectedTaxIdentifier = null;
                }
            }                
        }

        private void AutofillPersonIdentityDocument()
        {
            Person? person = _allPeople.Find(p => p.PersonIdentityDocument.DocumentNumber == IdentityDocumentNumber);
            if (person != null)
            {
                if (IsEnableIdentityDocument)
                {
                    IsEnableTaxIdentifier = false;
                    IsEnablePerson = false;
                }
                SelectedTaxIdentifier = person.TaxIdentifier;
                SelectedIdentityDocument = person.PersonIdentityDocument;
                LastName = person.LastName;
                FirstName = person.FirstName;
                MiddleName = person.MiddleName;
                _autofilledPersonIdentityDocument = true;
            }
            else
            {
                IsEnableTaxIdentifier = true;
                IsEnablePerson = true;
                bool isPassport = Regex.IsMatch(IdentityDocumentNumber, @"^[A-Z]{2} \d{6}$");
                bool isIdCard = Regex.IsMatch(IdentityDocumentNumber, @"^\d{9}$");

                if (isPassport || isIdCard)
                {
                    _selectedIdentityDocument = new PersonIdentityDocument() ;
                    if (isPassport)
                    {
                        SelectedIdentityDocument.Type = IdentityDocumentType.Passport;
                        SelectedIdentityDocument.Passport = new Passport() { Series = IdentityDocumentNumber.Substring(0, 2), Number = IdentityDocumentNumber.Substring(3, 6) };
                    }
                    else
                    {
                        SelectedIdentityDocument.Type = IdentityDocumentType.IDCard;
                        SelectedIdentityDocument.IDCard = new IDCard() { Number = IdentityDocumentNumber };
                    }
                }
                else
                {
                    if (_autofilledPersonIdentityDocument)
                    {
                        _autofilledPersonIdentityDocument = false;
                        TaxIdentifierNumber = string.Empty;
                    }
                    SelectedIdentityDocument = null;
                }
            }
        }

        public string Error => null;
        public string this[string columnName]
        {
            get
            {
                if (columnName == nameof(TaxIdentifierNumber))
                {
                    if (string.IsNullOrWhiteSpace(TaxIdentifierNumber))
                        return "Field is required";

                    if (!Regex.IsMatch(TaxIdentifierNumber, @"^\d{10}$"))
                        return "Must be exactly 10 digits";
                }

                if (columnName == nameof(IdentityDocumentNumber))
                {
                    if (string.IsNullOrWhiteSpace(IdentityDocumentNumber))
                        return "Field is required";

                    bool isPassport = Regex.IsMatch(IdentityDocumentNumber, @"^[A-Z]{2} \d{6}$");
                    bool isIdCard = Regex.IsMatch(IdentityDocumentNumber, @"^\d{9}$");

                    if (!isPassport && !isIdCard)
                        return "Enter either 'XX 000000' or '000000000'";
                }

                if (columnName == nameof(LastName))
                {
                    if (string.IsNullOrWhiteSpace(LastName))
                        return "Field is required";

                    if (!Regex.IsMatch(LastName, @"^[A-Za-z]+$"))
                        return "Must be only letter";
                }

                if (columnName == nameof(FirstName))
                {
                    if (string.IsNullOrWhiteSpace(FirstName))
                        return "Field is required";

                    if (!Regex.IsMatch(FirstName, @"^[A-Za-z]+$"))
                        return "Must be only letter";
                }

                if (columnName == nameof(MiddleName))
                {
                    if (MiddleName != string.Empty && !Regex.IsMatch(MiddleName, @"^[A-Za-z]+$")) 
                        return "Must be only letter";
                }

                if (columnName == nameof(PositionName))
                {
                    if (string.IsNullOrWhiteSpace(PositionName))
                        return "Field is required";
                }

                if (columnName == nameof(EmploymentRate))
                {
                    if (string.IsNullOrWhiteSpace(EmploymentRate))
                        return "Field is required";
                    if (!Regex.IsMatch(EmploymentRate, @"^\d+(\.\d{1,2})?$"))
                        return "Must be only letter";
                }

                if (columnName == nameof(Salary))
                {
                    if (string.IsNullOrWhiteSpace(Salary))
                        return "Field is required";
                    if (!Regex.IsMatch(Salary, @"^\d+(\.\d{1,2})?$"))
                        return "Must be only letter";
                }

                if (columnName == nameof(StartDate))
                {
                    if (StartDate == null)
                        return "Date is required";

                    if (StartDate?.Date < DateTime.Today)
                        return "Date must be today or later";
                }

                if (columnName == nameof(EndDate))
                {
                    if (EndDate != null && EndDate < StartDate)
                        return "End date must be after or equal to start date";
                }

                return null;
            }
        }

        private bool CanAddEmployee()
        {
            var propertiesToValidate = new[]
            {
                nameof(TaxIdentifierNumber),
                nameof(IdentityDocumentNumber),
                nameof(LastName),
                nameof(FirstName),
                nameof(MiddleName),
                nameof(PositionName),
                nameof(EmploymentRate),
                nameof(Salary),
                nameof(StartDate),
                nameof(EndDate)
            };

            return !propertiesToValidate.Any(p => this[p] != null);
        }
        public RelayCommand AddEmployeeCommand => new RelayCommand(execute => _ = AddEmployeeAsync(), canExecute => CanAddEmployee());
        private async Task AddEmployeeAsync()
        {
            Person person = _allPeople.Find(p => p.PersonIdentityDocument.DocumentNumber == IdentityDocumentNumber && p.TaxIdentifier.Number == TaxIdentifierNumber)
                    ?? new Person()
                    {
                        TaxIdentifier = SelectedTaxIdentifier,
                        PersonIdentityDocument = SelectedIdentityDocument,
                        LastName = LastName,
                        FirstName = FirstName,
                        MiddleName = MiddleName
                    };
            Employee employee = new Employee()
            {
                Person = person,
                Position = SelectedPosition,
                EmploymentRate = double.Parse(EmploymentRate),
                Salary = decimal.Parse(Salary),
                StartDate = (DateTime)StartDate,
                EndDate = EndDate
            };

            await _employeeService.AddEmplyoeeAsync(employee);
            MessageBox.Show("Employee added successfully", "Employee creating");
            ClearProperties();
        }

        private void ClearProperties()
        {
            IdentityDocumentNumber = string.Empty;
            TaxIdentifierNumber = string.Empty;
            LastName = string.Empty;
            FirstName = string.Empty;
            MiddleName = string.Empty;
            PositionName = string.Empty;
            IsDropDownOpenPosition = false;
            EmploymentRate = string.Empty;
            Salary = string.Empty;
            StartDate = null;
            EndDate = null;
        }

        public RelayCommand BackCommand => new RelayCommand(execute => _backAction?.Invoke());
    }
}

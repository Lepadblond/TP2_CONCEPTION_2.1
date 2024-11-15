using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Automate.Utils;
using Automate.Views;

namespace Automate.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region Champs

        private readonly MongoDBService _mongoService;
        private readonly Dictionary<string, List<string>> _errors = new();
        private string? _password;
        private string? _username;
        private readonly Window _window;

        #endregion

        #region Constructeur
        /// <summary>
        /// constructeur de la classe LoginViewModel
        /// </summary>
        /// <param name="openedWindow"></param>
        public LoginViewModel(Window openedWindow)
        {
            _mongoService = new MongoDBService("AutomateDB");
            AuthenticateCommand = new RelayCommand(Authenticate);
            _window = openedWindow;
        }

        #endregion

        #region Propriétés

        public ICommand AuthenticateCommand { get; }
        public bool HasPasswordErrors => _errors.ContainsKey(nameof(Password)) && _errors[nameof(Password)].Any();

        public string? Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
                ValidateProperty(nameof(Username));
            }
        }

        public string? Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                ValidateProperty(nameof(Password));
            }
        }

        public string ErrorMessages
        {
            get
            {
                var allErrors = new List<string>();
                foreach (var errorList in _errors.Values) allErrors.AddRange(errorList);
                allErrors.RemoveAll(error => string.IsNullOrWhiteSpace(error));
                return string.Join("\n", allErrors);
            }
        }

        #endregion

        #region Événements

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        public bool HasErrors => _errors.Count > 0;
        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region Méthodes
        /// <summary>
        /// Permet de récupérer les erreurs
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName)) return Enumerable.Empty<string>();
            return _errors[propertyName];
        }

        /// <summary>
        /// permet de notifier le changement de propriété
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Permet d'authentifier un utilisateur
        /// </summary>
        public void Authenticate()
        {
            ValidateProperty(nameof(Username));
            ValidateProperty(nameof(Password));

            if (!HasErrors)
            {
                var user = _mongoService.Authenticate(Username, Password);
                if (user == null)
                {
                    AddError("Username", "Nom d'utilisateur ou mot de passe invalide");
                    AddError("Password", "");
                    Trace.WriteLine("invalid");
                }
                else
                {
                    var windowAccueil = new AccueilWindow(user);
                    windowAccueil.Show();
                    _window.Close();
                    Trace.WriteLine("logged in");
                }
            }
        }

        /// <summary>
        /// permet de valider une propriété
        /// </summary>
        /// <param name="propertyName"></param>
        private void ValidateProperty(string? propertyName)
        {
            switch (propertyName)
            {
                case nameof(Username):
                    if (string.IsNullOrEmpty(Username))
                        AddError(nameof(Username), "Le nom d'utilisateur ne peut pas être vide.");
                    else
                        RemoveError(nameof(Username));
                    break;

                case nameof(Password):
                    if (string.IsNullOrEmpty(Password))
                        AddError(nameof(Password), "Le mot de passe ne peut pas être vide.");
                    else
                        RemoveError(nameof(Password));
                    break;
            }
        }


        /// <summary>
        /// Permet d'ajouter une erreur
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="errorMessage"></param>
        private void AddError(string propertyName, string errorMessage)
        {
            if (!_errors.ContainsKey(propertyName)) _errors[propertyName] = new List<string>();
            if (!_errors[propertyName].Contains(errorMessage))
            {
                _errors[propertyName].Add(errorMessage);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }

            OnPropertyChanged(nameof(ErrorMessages));
            OnPropertyChanged(nameof(HasPasswordErrors));
        }


        /// <summary>
        /// Permet de supprimer une erreur
        /// </summary>
        /// <param name="propertyName"></param>
        private void RemoveError(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }

            OnPropertyChanged(nameof(ErrorMessages));
            OnPropertyChanged(nameof(HasPasswordErrors));
        }

        #endregion
    }
}
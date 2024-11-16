using Microsoft.VisualStudio.TestTools.UnitTesting;
using Automate.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automate.Models;
using Automate.Utils;
using Moq;
using MongoDB.Bson;

namespace Automate.ViewModels.Tests
{
    [TestClass()]
    public class AccueilViewModelTests
    {

        private UserModel _testUser;

        [TestInitialize()]
        public void Setup()
        {
            _testUser = new UserModel { Username = "testuser", Role = "admin" };
        }

        [TestMethod()]
        public void Constructeur_Doit_Initialiser_Collection_Taches()
        {
            // Arrange
            var expectedUser = _testUser;

            // Act
            var viewModel = new AccueilViewModel(expectedUser);

            // Assert
            Assert.IsNotNull(viewModel.ObservableCollectionDeTaches);
        }

        [TestMethod()]
        public void Constructeur_Doit_Initialiser_Utilisateur_Connecte()
        {
            // Arrange
            var expectedUser = _testUser;

            // Act
            var viewModel = new AccueilViewModel(expectedUser);

            // Assert
            Assert.AreEqual(expectedUser, viewModel.UtilisateurConnecte);
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Automate.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automate.Interfaces;
using Automate.Models;
using Automate.Enum;
using TaskStatus = Automate.Enum.TaskStatus;
using MongoDB.Bson;
using Moq;

namespace Automate.Utils.Tests
{
    [TestClass()]
    public class MongoDBServiceTests
    {
        private Mock<IMongoDBService> _mockMongoDBService;
        private List<TacheModel> _listeTachesTest;
        private TacheModel _tacheAjouterTest;

        [TestInitialize]
        public void Setup()
        {
            _mockMongoDBService = new Mock<IMongoDBService>();

            var userId1 = ObjectId.GenerateNewId();
            var userId2 = ObjectId.GenerateNewId();

            // Arrange
            _listeTachesTest = new List<TacheModel>
            {
                new TacheModel
                {
                    IdEmployeAffecte = userId1,
                    Titre = "Test 1",
                    Description = "Description Test 1",
                    Type = TaskType.Semis,
                    Status = TaskStatus.EnCours,
                    DateDebut = DateTime.Now.AddDays(1),
                    DateFin = DateTime.Now.AddDays(7),
                    DateAjout = DateTime.Now.AddDays(-2),
                    DateDerniereModification = DateTime.Now.AddDays(-1)
                },
                new TacheModel
                {
                    IdEmployeAffecte = userId2,
                    Titre = "Test 2",
                    Description = "Description Test 2",
                    Type = TaskType.Recolte,
                    Status = TaskStatus.Complete,
                    DateDebut = DateTime.Now.AddDays(2),
                    DateFin = DateTime.Now.AddDays(8),
                    DateAjout = DateTime.Now.AddDays(-3),
                    DateDerniereModification = DateTime.Now.AddDays(-2)
                }
            };

            _mockMongoDBService.Setup(service => service.ObtenirToutesLesTaches()).Returns(_listeTachesTest);


            _tacheAjouterTest = new TacheModel
            {
                IdEmployeAffecte = userId1,
                Titre = "Test",
                Description = "Ceci est la description du test.",
                Type = TaskType.Semis,
                Status = TaskStatus.EnCours,
                DateDebut = DateTime.Now.AddDays(1),
                DateFin = DateTime.Now.AddDays(7),
                DateAjout = DateTime.Now.AddDays(-2),
                DateDerniereModification = DateTime.Now.AddDays(-1)
            };
        }

        [TestMethod()]
        public void ObtenirToutesLesTaches_Retourne_Toutes_Les_Taches()
        {
            // Act
            var listeTachesRecuperee = _mockMongoDBService.Object.ObtenirToutesLesTaches();

            // Assert
            Assert.AreEqual(_listeTachesTest.Count, listeTachesRecuperee.Count, "Le nombre de tâches retournées n'est pas correct.");

        }

        [TestMethod()]
        public void ObtenirToutesLesTaches_Retourne_Pas_Null()
        {
            // Act
            var listeTachesRecuperee = _mockMongoDBService.Object.ObtenirToutesLesTaches();

            // Assert
            Assert.IsNotNull(listeTachesRecuperee, "La liste des tâches ne devrait pas être nulle.");

        }

        [TestMethod]
        public void ObtenirTacheParId_Retourne_Tache()
        {
            // Arrange
            var tacheId = ObjectId.GenerateNewId();
            var tacheTest = new TacheModel
            {
                Id = tacheId,
                IdEmployeAffecte = ObjectId.GenerateNewId(),
                Titre = "Test bidon",
                Description = "Description Test bla bla bla",
                Type = TaskType.Semis,
                Status = TaskStatus.EnCours,
                DateDebut = DateTime.Now.AddDays(1),
                DateFin = DateTime.Now.AddDays(7),
                DateAjout = DateTime.Now.AddDays(-2),
                DateDerniereModification = DateTime.Now.AddDays(-1)
            };

            _listeTachesTest.Add(tacheTest);

            _mockMongoDBService.Setup(service => service.ObtenirTacheParId(tacheId)).Returns(tacheTest);

            // Act
            var tacheRecuperee = _mockMongoDBService.Object.ObtenirTacheParId(tacheId);

            // Assert
            Assert.AreEqual(tacheId, tacheRecuperee.Id);
        }

        [TestMethod]
        public void ObtenirTacheParId_Retourne_Pas_Null()
        {
            // Arrange
            var tacheId = ObjectId.GenerateNewId();
            var tacheTest = new TacheModel
            {
                Id = tacheId,
                IdEmployeAffecte = ObjectId.GenerateNewId(),
                Titre = "Test bidon",
                Description = "Description Test bla bla bla",
                Type = TaskType.Semis,
                Status = TaskStatus.EnCours,
                DateDebut = DateTime.Now.AddDays(1),
                DateFin = DateTime.Now.AddDays(7),
                DateAjout = DateTime.Now.AddDays(-2),
                DateDerniereModification = DateTime.Now.AddDays(-1)
            };

            _listeTachesTest.Add(tacheTest);

            _mockMongoDBService.Setup(service => service.ObtenirTacheParId(tacheId)).Returns(tacheTest);

            // Act
            var tacheRecuperee = _mockMongoDBService.Object.ObtenirTacheParId(tacheId);

            // Assert
            Assert.IsNotNull(tacheRecuperee);
        }

        [TestMethod()]
        public void AjouterTache_Ajoute_Tache_Dans_BD()
        {
            // Act
            _mockMongoDBService.Setup(service => service.AjouterTache(_tacheAjouterTest));
            _mockMongoDBService.Object.AjouterTache(_tacheAjouterTest);

            // Assert
            _mockMongoDBService.Verify(service => service.AjouterTache(It.Is<TacheModel>(
                t => t.IdEmployeAffecte == _tacheAjouterTest.IdEmployeAffecte &&
                     t.Titre == _tacheAjouterTest.Titre &&
                     t.Description == _tacheAjouterTest.Description &&
                     t.Type == _tacheAjouterTest.Type &&
                     t.Status == _tacheAjouterTest.Status &&
                     t.DateDebut == _tacheAjouterTest.DateDebut &&
                     t.DateFin == _tacheAjouterTest.DateFin &&
                     t.DateAjout == _tacheAjouterTest.DateAjout &&
                     t.DateDerniereModification == _tacheAjouterTest.DateDerniereModification
            )), Times.Once);
        }

        [TestMethod()]
        public void AjouterTache_Recupere_Bon_IdEmployeAffecte_Dans_Tache()
        {
            // Arrange
            _mockMongoDBService.Setup(service => service.ObtenirTacheParId(_tacheAjouterTest.Id)).Returns(_tacheAjouterTest);

            // Act
            var tacheRecuperee = _mockMongoDBService.Object.ObtenirTacheParId(_tacheAjouterTest.Id);

            // Assert
            Assert.AreEqual(_tacheAjouterTest.IdEmployeAffecte, tacheRecuperee.IdEmployeAffecte);
        }

        [TestMethod()]
        public void AjouterTache_Recupere_Bon_Titre_Dans_Tache()
        {
            // Arrange
            _mockMongoDBService.Setup(service => service.ObtenirTacheParId(_tacheAjouterTest.Id)).Returns(_tacheAjouterTest);

            // Act
            var tacheRecuperee = _mockMongoDBService.Object.ObtenirTacheParId(_tacheAjouterTest.Id);

            // Assert
            Assert.AreEqual(_tacheAjouterTest.Titre, tacheRecuperee.Titre);
        }

        [TestMethod()]
        public void AjouterTache_Recupere_Bonne_Description_Dans_Tache()
        {
            // Arrange
            _mockMongoDBService.Setup(service => service.ObtenirTacheParId(_tacheAjouterTest.Id)).Returns(_tacheAjouterTest);

            // Act
            var tacheRecuperee = _mockMongoDBService.Object.ObtenirTacheParId(_tacheAjouterTest.Id);

            // Assert
            Assert.AreEqual(_tacheAjouterTest.Description, tacheRecuperee.Description);
        }

        [TestMethod()]
        public void AjouterTache_Recupere_Bon_Type_Dans_Tache()
        {
            // Arrange
            _mockMongoDBService.Setup(service => service.ObtenirTacheParId(_tacheAjouterTest.Id)).Returns(_tacheAjouterTest);

            // Act
            var tacheRecuperee = _mockMongoDBService.Object.ObtenirTacheParId(_tacheAjouterTest.Id);

            // Assert
            Assert.AreEqual(_tacheAjouterTest.Type, tacheRecuperee.Type);
        }

        [TestMethod()]
        public void AjouterTache_Recupere_Bon_Statut_Dans_Tache()
        {
            // Arrange
            _mockMongoDBService.Setup(service => service.ObtenirTacheParId(_tacheAjouterTest.Id)).Returns(_tacheAjouterTest);

            // Act
            var tacheRecuperee = _mockMongoDBService.Object.ObtenirTacheParId(_tacheAjouterTest.Id);

            // Assert
            Assert.AreEqual(_tacheAjouterTest.Status, tacheRecuperee.Status);
        }

        [TestMethod()]
        public void AjouterTache_Recupere_Bonne_DateDebut_Dans_Tache()
        {
            // Arrange
            _mockMongoDBService.Setup(service => service.ObtenirTacheParId(_tacheAjouterTest.Id)).Returns(_tacheAjouterTest);

            // Act
            var tacheRecuperee = _mockMongoDBService.Object.ObtenirTacheParId(_tacheAjouterTest.Id);

            // Assert
            Assert.AreEqual(_tacheAjouterTest.DateDebut, tacheRecuperee.DateDebut);
        }

        [TestMethod()]
        public void AjouterTache_Recupere_Bonne_DateFin_Dans_Tache()
        {
            // Arrange
            _mockMongoDBService.Setup(service => service.ObtenirTacheParId(_tacheAjouterTest.Id)).Returns(_tacheAjouterTest);

            // Act
            var tacheRecuperee = _mockMongoDBService.Object.ObtenirTacheParId(_tacheAjouterTest.Id);

            // Assert
            Assert.AreEqual(_tacheAjouterTest.DateFin, tacheRecuperee.DateFin);
        }

        [TestMethod()]
        public void AjouterTache_Recupere_Bonne_DateAjout_Dans_Tache()
        {
            // Arrange
            _mockMongoDBService.Setup(service => service.ObtenirTacheParId(_tacheAjouterTest.Id)).Returns(_tacheAjouterTest);

            // Act
            var tacheRecuperee = _mockMongoDBService.Object.ObtenirTacheParId(_tacheAjouterTest.Id);

            // Assert
            Assert.AreEqual(_tacheAjouterTest.DateAjout, tacheRecuperee.DateAjout);
        }

        [TestMethod()]
        public void AjouterTache_Recupere_Bonne_DateDerniereModification_Dans_Tache()
        {
            // Arrange
            _mockMongoDBService.Setup(service => service.ObtenirTacheParId(_tacheAjouterTest.Id)).Returns(_tacheAjouterTest);

            // Act
            var tacheRecuperee = _mockMongoDBService.Object.ObtenirTacheParId(_tacheAjouterTest.Id);

            // Assert
            Assert.AreEqual(_tacheAjouterTest.DateDerniereModification, tacheRecuperee.DateDerniereModification);
        }

        [TestMethod()]
        public void ModifierTache_Modifie_Tache_Dans_BD()
        {
            // Arrange
            ObjectId idTache = _tacheAjouterTest.Id;

            TacheModel tacheModifiee = new TacheModel
            {
                Id = idTache,
                IdEmployeAffecte = ObjectId.GenerateNewId(),
                Titre = "Titre Modifié",
                Description = "Description Modifiée",
                Type = TaskType.Recolte,
                Status = TaskStatus.Complete,
                DateDebut = DateTime.Now,
                DateFin = DateTime.Now.AddDays(10),
                DateAjout = _tacheAjouterTest.DateAjout,
                DateDerniereModification = DateTime.Now
            };

            _mockMongoDBService.Setup(service => service.ModifierTache(idTache, tacheModifiee));
            _mockMongoDBService.Object.ModifierTache(idTache, tacheModifiee);

            _mockMongoDBService.Verify(service => service.ModifierTache(It.Is<ObjectId>(id => id == idTache),
                It.Is<TacheModel>(t =>
                    t.IdEmployeAffecte == tacheModifiee.IdEmployeAffecte &&
                    t.Titre == tacheModifiee.Titre &&
                    t.Description == tacheModifiee.Description &&
                    t.Type == tacheModifiee.Type &&
                    t.Status == tacheModifiee.Status &&
                    t.DateDebut == tacheModifiee.DateDebut &&
                    t.DateFin == tacheModifiee.DateFin &&
                    t.DateAjout == tacheModifiee.DateAjout &&
                    t.DateDerniereModification == tacheModifiee.DateDerniereModification
                )), Times.Once);
        }

        [TestMethod()]
        public void ModifierTache_Modifie_IdEmployeAffecte_Dans_Tache()
        {
            // Arrange
            ObjectId idTache = _tacheAjouterTest.Id;

            TacheModel tacheModifiee = new TacheModel
            {
                Id = idTache,
                IdEmployeAffecte = ObjectId.GenerateNewId(),
                Titre = "Titre Modifié",
                Description = "Description Modifiée",
                Type = TaskType.Recolte,
                Status = TaskStatus.Complete,
                DateDebut = DateTime.Now,
                DateFin = DateTime.Now.AddDays(10),
                DateAjout = _tacheAjouterTest.DateAjout,
                DateDerniereModification = DateTime.Now
            };

            _mockMongoDBService.Setup(service => service.ModifierTache(idTache, tacheModifiee));
            _mockMongoDBService.Object.ModifierTache(idTache, tacheModifiee);

            _mockMongoDBService.Verify(service => service.ModifierTache(It.Is<ObjectId>(id => id == idTache),
                It.Is<TacheModel>(t =>
                    t.IdEmployeAffecte == tacheModifiee.IdEmployeAffecte)), Times.Once);
        }


        [TestMethod()]
        public void ModifierTache_Modifie_Titre_Dans_Tache()
        {
            // Arrange
            ObjectId idTache = _tacheAjouterTest.Id;

            TacheModel tacheModifiee = new TacheModel
            {
                Id = idTache,
                IdEmployeAffecte = ObjectId.GenerateNewId(),
                Titre = "Titre Modifié",
                Description = "Description Modifiée",
                Type = TaskType.Recolte,
                Status = TaskStatus.Complete,
                DateDebut = DateTime.Now,
                DateFin = DateTime.Now.AddDays(10),
                DateAjout = _tacheAjouterTest.DateAjout,
                DateDerniereModification = DateTime.Now
            };

            _mockMongoDBService.Setup(service => service.ModifierTache(idTache, tacheModifiee));
            _mockMongoDBService.Object.ModifierTache(idTache, tacheModifiee);

            _mockMongoDBService.Verify(service => service.ModifierTache(It.Is<ObjectId>(id => id == idTache),
                It.Is<TacheModel>(t =>
                    t.Titre == tacheModifiee.Titre)), Times.Once);
        }

        [TestMethod()]
        public void ModifierTache_Modifie_Description_Dans_Tache()
        {
            // Arrange
            ObjectId idTache = _tacheAjouterTest.Id;

            TacheModel tacheModifiee = new TacheModel
            {
                Id = idTache,
                IdEmployeAffecte = ObjectId.GenerateNewId(),
                Titre = "Titre Modifié",
                Description = "Description Modifiée",
                Type = TaskType.Recolte,
                Status = TaskStatus.Complete,
                DateDebut = DateTime.Now,
                DateFin = DateTime.Now.AddDays(10),
                DateAjout = _tacheAjouterTest.DateAjout,
                DateDerniereModification = DateTime.Now
            };

            _mockMongoDBService.Setup(service => service.ModifierTache(idTache, tacheModifiee));
            _mockMongoDBService.Object.ModifierTache(idTache, tacheModifiee);

            _mockMongoDBService.Verify(service => service.ModifierTache(It.Is<ObjectId>(id => id == idTache),
                It.Is<TacheModel>(t =>
                    t.Description == tacheModifiee.Description)), Times.Once);
        }

        [TestMethod()]
        public void ModifierTache_Modifie_Type_Dans_Tache()
        {
            // Arrange
            ObjectId idTache = _tacheAjouterTest.Id;

            TacheModel tacheModifiee = new TacheModel
            {
                Id = idTache,
                IdEmployeAffecte = ObjectId.GenerateNewId(),
                Titre = "Titre Modifié",
                Description = "Description Modifiée",
                Type = TaskType.Recolte,
                Status = TaskStatus.Complete,
                DateDebut = DateTime.Now,
                DateFin = DateTime.Now.AddDays(10),
                DateAjout = _tacheAjouterTest.DateAjout,
                DateDerniereModification = DateTime.Now
            };

            _mockMongoDBService.Setup(service => service.ModifierTache(idTache, tacheModifiee));
            _mockMongoDBService.Object.ModifierTache(idTache, tacheModifiee);

            _mockMongoDBService.Verify(service => service.ModifierTache(It.Is<ObjectId>(id => id == idTache),
                It.Is<TacheModel>(t =>
                    t.Type == tacheModifiee.Type)), Times.Once);
        }


        [TestMethod()]
        public void ModifierTache_Modifie_Status_Dans_Tache()
        {
            // Arrange
            ObjectId idTache = _tacheAjouterTest.Id;

            TacheModel tacheModifiee = new TacheModel
            {
                Id = idTache,
                IdEmployeAffecte = ObjectId.GenerateNewId(),
                Titre = "Titre Modifié",
                Description = "Description Modifiée",
                Type = TaskType.Recolte,
                Status = TaskStatus.Complete,
                DateDebut = DateTime.Now,
                DateFin = DateTime.Now.AddDays(10),
                DateAjout = _tacheAjouterTest.DateAjout,
                DateDerniereModification = DateTime.Now
            };

            _mockMongoDBService.Setup(service => service.ModifierTache(idTache, tacheModifiee));
            _mockMongoDBService.Object.ModifierTache(idTache, tacheModifiee);

            _mockMongoDBService.Verify(service => service.ModifierTache(It.Is<ObjectId>(id => id == idTache),
                It.Is<TacheModel>(t =>
                    t.Status == tacheModifiee.Status)), Times.Once);
        }

        [TestMethod()]
        public void ModifierTache_Modifie_DateDebut_Dans_Tache()
        {
            // Arrange
            ObjectId idTache = _tacheAjouterTest.Id;

            TacheModel tacheModifiee = new TacheModel
            {
                Id = idTache,
                IdEmployeAffecte = ObjectId.GenerateNewId(),
                Titre = "Titre Modifié",
                Description = "Description Modifiée",
                Type = TaskType.Recolte,
                Status = TaskStatus.Complete,
                DateDebut = DateTime.Now,
                DateFin = DateTime.Now.AddDays(10),
                DateAjout = _tacheAjouterTest.DateAjout,
                DateDerniereModification = DateTime.Now
            };

            _mockMongoDBService.Setup(service => service.ModifierTache(idTache, tacheModifiee));
            _mockMongoDBService.Object.ModifierTache(idTache, tacheModifiee);

            _mockMongoDBService.Verify(service => service.ModifierTache(It.Is<ObjectId>(id => id == idTache),
                It.Is<TacheModel>(t =>
                    t.DateDebut == tacheModifiee.DateDebut)), Times.Once);
        }


        [TestMethod()]
        public void ModifierTache_Modifie_DateFin_Dans_Tache()
        {
            // Arrange
            ObjectId idTache = _tacheAjouterTest.Id;

            TacheModel tacheModifiee = new TacheModel
            {
                Id = idTache,
                IdEmployeAffecte = ObjectId.GenerateNewId(),
                Titre = "Titre Modifié",
                Description = "Description Modifiée",
                Type = TaskType.Recolte,
                Status = TaskStatus.Complete,
                DateDebut = DateTime.Now,
                DateFin = DateTime.Now.AddDays(10),
                DateAjout = _tacheAjouterTest.DateAjout,
                DateDerniereModification = DateTime.Now
            };

            _mockMongoDBService.Setup(service => service.ModifierTache(idTache, tacheModifiee));
            _mockMongoDBService.Object.ModifierTache(idTache, tacheModifiee);

            _mockMongoDBService.Verify(service => service.ModifierTache(It.Is<ObjectId>(id => id == idTache),
                It.Is<TacheModel>(t =>
                    t.DateFin == tacheModifiee.DateFin)), Times.Once);
        }


        [TestMethod()]
        public void ModifierTache_Modifie_DateAjout_Dans_Tache()
        {
            // Arrange
            ObjectId idTache = _tacheAjouterTest.Id;

            TacheModel tacheModifiee = new TacheModel
            {
                Id = idTache,
                IdEmployeAffecte = ObjectId.GenerateNewId(),
                Titre = "Titre Modifié",
                Description = "Description Modifiée",
                Type = TaskType.Recolte,
                Status = TaskStatus.Complete,
                DateDebut = DateTime.Now,
                DateFin = DateTime.Now.AddDays(10),
                DateAjout = _tacheAjouterTest.DateAjout,
                DateDerniereModification = DateTime.Now
            };

            _mockMongoDBService.Setup(service => service.ModifierTache(idTache, tacheModifiee));
            _mockMongoDBService.Object.ModifierTache(idTache, tacheModifiee);

            _mockMongoDBService.Verify(service => service.ModifierTache(It.Is<ObjectId>(id => id == idTache),
                It.Is<TacheModel>(t =>
                    t.DateAjout == tacheModifiee.DateAjout)), Times.Once);
        }

        [TestMethod()]
        public void ModifierTache_Modifie_DateDerniereModification_Dans_Tache()
        {
            // Arrange
            ObjectId idTache = _tacheAjouterTest.Id;

            TacheModel tacheModifiee = new TacheModel
            {
                Id = idTache,
                IdEmployeAffecte = ObjectId.GenerateNewId(),
                Titre = "Titre Modifié",
                Description = "Description Modifiée",
                Type = TaskType.Recolte,
                Status = TaskStatus.Complete,
                DateDebut = DateTime.Now,
                DateFin = DateTime.Now.AddDays(10),
                DateAjout = _tacheAjouterTest.DateAjout,
                DateDerniereModification = DateTime.Now
            };

            _mockMongoDBService.Setup(service => service.ModifierTache(idTache, tacheModifiee));
            _mockMongoDBService.Object.ModifierTache(idTache, tacheModifiee);

            _mockMongoDBService.Verify(service => service.ModifierTache(It.Is<ObjectId>(id => id == idTache),
                It.Is<TacheModel>(t =>
                    t.DateDerniereModification == tacheModifiee.DateDerniereModification)), Times.Once);
        }


    }
}
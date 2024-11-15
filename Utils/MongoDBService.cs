using Automate.Interfaces;
using Automate.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace Automate.Utils
{
    public class MongoDBService : IMongoDBService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<UserModel> _users;
        private readonly IMongoCollection<TacheModel> _taches;

        #region Constructeur

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="MongoDBService"/>.
        /// </summary>
        /// <param name="databaseName">Nom de la base de données.</param>
        public MongoDBService(string databaseName)
        {
            var client = new MongoClient("mongodb://localhost:27017"); // URL du serveur MongoDB
            _database = client.GetDatabase(databaseName);
            _users = _database.GetCollection<UserModel>("Users");
            _taches = _database.GetCollection<TacheModel>("Taches");
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Obtient une collection MongoDB.
        /// </summary>
        /// <typeparam name="T">Type de la collection.</typeparam>
        /// <param name="collectionName">Nom de la collection.</param>
        /// <returns>Collection MongoDB.</returns>
        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }

        /// <summary>
        /// Authentifie un utilisateur.
        /// </summary>
        /// <param name="username">Nom d'utilisateur.</param>
        /// <param name="password">Mot de passe.</param>
        /// <returns>Utilisateur authentifié.</returns>
        public UserModel Authenticate(string? username, string? password)
        {
            var user = _users.Find(u => u.Username == username && u.Password == password).FirstOrDefault();
            return user;
        }

        /// <summary>
        /// Obtient tous les utilisateurs.
        /// </summary>
        /// <returns>Liste des utilisateurs.</returns>
        public List<UserModel> ObtenirTousLesUtilisateurs()
        {
            var users = _users.Find(t => true).ToList();
            return users;
        }

        /// <summary>
        /// Obtient toutes les tâches.
        /// </summary>
        /// <returns>Liste des tâches.</returns>
        public List<TacheModel> ObtenirToutesLesTaches()
        {
            var taches = _taches.Find(t => true).ToList();
            return taches;
        }

        /// <summary>
        /// Obtient une tâche par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant de la tâche.</param>
        /// <returns>Tâche correspondante.</returns>
        public TacheModel ObtenirTacheParId(ObjectId id)
        {
            var tache = _taches.Find(t => t.Id == id).FirstOrDefault();
            return tache;
        }

        /// <summary>
        /// Ajoute une nouvelle tâche.
        /// </summary>
        /// <param name="tache">Tâche à ajouter.</param>
        public void AjouterTache(TacheModel tache)
        {
            _taches.InsertOne(tache);
        }

        /// <summary>
        /// Modifie une tâche existante.
        /// </summary>
        /// <param name="id">Identifiant de la tâche à modifier.</param>
        /// <param name="tacheModifiee">Tâche modifiée.</param>
        public void ModifierTache(ObjectId id, TacheModel tacheModifiee)
        {
            var updateDefinition = Builders<TacheModel>.Update
                .Set(t => t.IdEmployeAffecte, tacheModifiee.IdEmployeAffecte)
                .Set(t => t.Titre, tacheModifiee.Titre)
                .Set(t => t.Description, tacheModifiee.Description)
                .Set(t => t.Type, tacheModifiee.Type)
                .Set(t => t.Status, tacheModifiee.Status)
                .Set(t => t.DateDebut, tacheModifiee.DateDebut)
                .Set(t => t.DateFin, tacheModifiee.DateFin)
                .Set(t => t.DateAjout, tacheModifiee.DateAjout)
                .Set(t => t.DateDerniereModification, tacheModifiee.DateDerniereModification);

            var result = _taches.UpdateOne(t => t.Id == id, updateDefinition);

            if (result.MatchedCount == 0)
            {
                throw new Exception("Tâche non trouvée.");
            }
        }

        /// <summary>
        /// Supprime une tâche.
        /// </summary>
        /// <param name="tache">Tâche à supprimer.</param>
        public virtual void SupprimerTache(TacheModel tache)
        {
            _taches.DeleteOne(t => t.Id == tache.Id);
        }

        /// <summary>
        /// Filtre les tâches par date.
        /// </summary>
        /// <param name="dateSelectionnee">Date de sélection.</param>
        /// <returns>Liste des tâches filtrées.</returns>
        public virtual List<TacheModel> FiltrerTachesParDate(DateTime dateSelectionnee)
        {
            var filtre = Builders<TacheModel>.Filter.Eq(t => t.DateDebut, dateSelectionnee);
            var tachesFiltrees = _taches.Find(filtre).ToList();
            return tachesFiltrees;
        }

        #endregion
    }
}
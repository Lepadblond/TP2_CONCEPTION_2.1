using Automate.Interfaces;
using Automate.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using Automate.Models;
using MongoDB.Driver;

namespace Automate.Utils
{
    public class MongoDBService : IMongoDBService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<UserModel> _users;
        private readonly IMongoCollection<TacheModel> _taches;
        public MongoDBService(string databaseName)
        {
            var client = new MongoClient("mongodb://localhost:27017"); // URL du serveur MongoDB
            _database = client.GetDatabase(databaseName);
            _users = _database.GetCollection<UserModel>("Users");
            _taches = _database.GetCollection<TacheModel>("Taches");
        }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }

        public UserModel Authenticate(string? username, string? password)
        {
            var user = _users.Find(u => u.Username == username && u.Password == password).FirstOrDefault();
            return user;
        }
        public List<UserModel> ObtenirTousLesUtilisateurs()
        {
            var users = _users.Find(t => true).ToList();
            return users;
        }

        public List<TacheModel> ObtenirToutesLesTaches()
        {
            var taches = _taches.Find(t => true).ToList();
            return taches;
        }

        public TacheModel ObtenirTacheParId(ObjectId id)
        {
            var tache = _taches.Find(t => t.Id == id).FirstOrDefault();
            return tache;
        }

        public void AjouterTache(TacheModel tache)
        {
            _taches.InsertOne(tache);
        }

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

    public virtual void SupprimerTache(TacheModel tache)
    {
        _taches.DeleteOne(t => t.Id == tache.Id);
    }

    public virtual List<TacheModel> FiltrerTachesParDate(DateTime dateSelectionnee)
    {
        var filtre = Builders<TacheModel>.Filter.Eq(t => t.DateDebut, dateSelectionnee);
        var tachesFiltrees = _taches.Find(filtre).ToList();
        return tachesFiltrees;
    }
}
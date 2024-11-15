using Automate.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Automate.Utils
{
    public class MongoDBService
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

        public List<TacheModel> ObtenirToutLesTaches()
        {
            var taches = _taches.Find(t => true).ToList();
            return taches;
        }

        public virtual void AjouterTache(TacheModel tache)
        {
            _taches.InsertOne(tache);
        }

        public virtual void ModifierTache(TacheModel tache)
        {
            _taches.ReplaceOne(t => t.Id == tache.Id, tache);
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

}

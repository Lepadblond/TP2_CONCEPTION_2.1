using Automate.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace Automate.Interfaces
{
    public interface IMongoDBService
    {
        void AjouterTache(TacheModel tache);
        UserModel Authenticate(string? username, string? password);
        List<TacheModel> FiltrerTachesParDate(DateTime dateSelectionnee);
        IMongoCollection<T> GetCollection<T>(string collectionName);
        void ModifierTache(ObjectId id, TacheModel tacheModifiee);
        TacheModel ObtenirTacheParId(ObjectId id);
        List<UserModel> ObtenirTousLesUtilisateurs();
        List<TacheModel> ObtenirToutesLesTaches();
        void SupprimerTache(TacheModel tache);
    }
}
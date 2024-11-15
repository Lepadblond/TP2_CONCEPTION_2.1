using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Automate.Models
{
    public class TacheModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("IdEmployeAffecte")]
        public ObjectId? IdEmployeAffecte { get; set; }

        [BsonElement("Titre")]
        public string? Titre { get; set; }

        [BsonElement("Description")]
        public string? Description { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("Type")]
        public Automate.Enum.TaskType Type { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("Status")]
        public Automate.Enum.TaskStatus Status { get; set; }

        [BsonElement("DateDebut")]
        public DateTime? DateDebut { get; set; }

        [BsonElement("DateFin")]
        public DateTime? DateFin { get; set; }

        [BsonElement("DateAjout")]
        public DateTime DateAjout { get; set; }

        [BsonElement("DateDerniereModification")]
        public DateTime DateDerniereModification { get; set; }
    }
}
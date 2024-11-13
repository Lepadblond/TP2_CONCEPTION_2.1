using System.ComponentModel;

namespace Automate.Enum
{
    /// <summary>
    /// Enum spécifiant le type de tâche
    /// </summary>
    public enum TaskType
    {
        Semis = 0,
        Rempotage = 1,
        Entretien = 2,
        Arrosage = 3,
        [Description("Recolte")]
        Recolte = 4,
        Commandes = 5,
        [Description("Événement spéciaux")]
        EvenementSpeciaux = 6
    }
}
using System.ComponentModel;

namespace Automate.Enum
{
    /// <summary>
    /// Enum spécifiant le statut de la tâche
    /// </summary>
    public enum TaskStatus
    {
        [Description("À faire")]
        AFaire = 0,
        [Description("En cours")]
        EnCours = 1,
        [Description("Complété")]
        Complete = 2
    }
}
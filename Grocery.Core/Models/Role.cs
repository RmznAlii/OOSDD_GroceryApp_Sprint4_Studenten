namespace Grocery.Core.Models
{
    /// <summary>
    /// Deze enum stelt de mogelijke rollen van een client (gebruiker) in de applicatie voor.
    /// </summary>
    public enum Role
    {
        /// <summary>
        /// Geen rol toegewezen (standaardwaarde).
        /// </summary>
        None = 0,

        /// <summary>
        /// Admin-rol: heeft extra rechten en kan beheertaken uitvoeren.
        /// </summary>
        Admin = 1
    }
}
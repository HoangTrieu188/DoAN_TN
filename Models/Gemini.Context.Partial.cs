namespace Gemini.Models
{
    using System.Data.Entity;

    // Partial class to extend GeminiEntities with PosInventory DbSet
    // This file won't be overwritten when regenerating from database
    public partial class GeminiEntities
    {
        public DbSet<PosInventory> PosInventories { get; set; }
    }
}

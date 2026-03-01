namespace Gemini.Models
{
    using System.Collections.Generic;

    // Partial class to extend PosProduce with navigation property to PosInventories
    // This file won't be overwritten when regenerating from database
    public partial class PosProduce
    {


        public virtual ICollection<PosInventory> PosInventories { get; set; } = new HashSet<PosInventory>();
    }
}

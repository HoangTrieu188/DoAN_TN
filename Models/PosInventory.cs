namespace Gemini.Models
{
    using System;
    
    public partial class PosInventory
    {
        public Guid Guid { get; set; }
        public Guid GuidProduce { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }

        public virtual PosProduce PosProduce { get; set; }
    }
}

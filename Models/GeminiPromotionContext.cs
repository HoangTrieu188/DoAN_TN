using System.Data.Entity;

namespace Gemini.Models
{
    public class GeminiPromotionContext : DbContext
    {
        public GeminiPromotionContext() : base("name=GeminiPromotionContext")
        {
        }

        public virtual DbSet<PosPromotion> PosPromotions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Map to the existing table created via SQL
            modelBuilder.Entity<PosPromotion>().ToTable("PosPromotion");
        }
    }
}

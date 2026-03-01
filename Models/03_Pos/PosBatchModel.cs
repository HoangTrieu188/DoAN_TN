using System;
using System.ComponentModel.DataAnnotations;
using Antlr.Runtime.Misc;
using Gemini.Resources;

namespace Gemini.Models._03_Pos
{
    public class PosBatchModel
    {
        public int IsUpdate { get; set; }

        #region Properties
        [ScaffoldColumn(false)]
        public Guid Guid { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        public String Name { get; set; }

        [StringLength(2000, ErrorMessageResourceName = "ErrorMaxLength2000", ErrorMessageResourceType = typeof(Resource))]
        public String Note { get; set; }

        public bool Active { get; set; }

        public DateTime? ManufacturingAt { get; set; }

        [Editable(false)]
        public DateTime? CreatedAt { get; set; }

        [Editable(false)]
        [StringLength(25, ErrorMessageResourceName = "ErrorMaxLength25", ErrorMessageResourceType = typeof(Resource))]
        public String CreatedBy { get; set; }

        [Editable(false)]
        public DateTime? UpdatedAt { get; set; }

        [Editable(false)]
        [StringLength(25, ErrorMessageResourceName = "ErrorMaxLength25", ErrorMessageResourceType = typeof(Resource))]
        public String UpdatedBy { get; set; }

        #endregion

        #region Constructor
        public PosBatchModel()
        {
        }

        public PosBatchModel(PosBatch posBatch)
        {
            Guid = posBatch.Guid;
            Name = posBatch.Name;
            Note = posBatch.Note;
            ManufacturingAt = posBatch.ManufacturingAt;
            Active = posBatch.Active;
            CreatedAt = posBatch.CreatedAt;
            CreatedBy = posBatch.CreatedBy;
            UpdatedAt = posBatch.UpdatedAt;
            UpdatedBy = posBatch.UpdatedBy;
        }
        #endregion

        #region Function
        public void Setvalue(PosBatch posBatch)
        {
            if (IsUpdate == 0)
            {
                posBatch.Guid = Guid.NewGuid();
                posBatch.CreatedBy = CreatedBy;
                posBatch.CreatedAt = DateTime.Now;
            }
            posBatch.Name = Name;
            posBatch.Note = Note;
            posBatch.ManufacturingAt = ManufacturingAt;
            posBatch.Active = Active;
            posBatch.UpdatedAt = DateTime.Now;
            posBatch.UpdatedBy = UpdatedBy;
        }
        #endregion
    }
}
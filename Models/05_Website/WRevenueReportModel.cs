using System;
using Gemini.Resources;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Gemini.Models._05_Website
{
    public class WRevenueReportModel
    {
        #region Properties
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFill")]
        public DateTime FromAt { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFill")]
        public DateTime ToAt { get; set; }
        #endregion

        public string ListLabel { get; set; }

        public string ListRevenue { get; set; }
    }
}
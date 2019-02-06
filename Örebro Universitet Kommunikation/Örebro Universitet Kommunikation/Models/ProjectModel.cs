using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Örebro_Universitet_Kommunikation.Models {
    public class ProjectModel {
        [Key]
        public int ProjectId { get; set; }
        
        public string ProjectName { get; set; }
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }

        public ProjectModel() {
            ApplicationUsers = new HashSet<ApplicationUser>();
        }

    }
}
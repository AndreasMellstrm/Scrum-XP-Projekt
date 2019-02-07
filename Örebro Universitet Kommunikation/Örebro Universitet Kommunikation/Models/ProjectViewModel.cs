using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Örebro_Universitet_Kommunikation.Models {
    public class ProjectViewModel {
        public List<ProjectItem> ProjectItems { get; set; }
    }
    public class ProjectItem {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public int BlogAmount { get; set; }
        public bool IsMember { get; set; }
    }
}
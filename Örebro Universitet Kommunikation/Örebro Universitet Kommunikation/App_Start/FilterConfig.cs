using System.Web;
using System.Web.Mvc;

namespace Örebro_Universitet_Kommunikation {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }
    }
}

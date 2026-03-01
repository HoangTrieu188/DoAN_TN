using System.Web;
using System.Web.SessionState;

namespace Gemini.Controllers.Bussiness
{
    public class UserSession
    {
        private static HttpSessionState session { get { return HttpContext.Current.Session; } }

        public static string Giohangsp
        {
            get { return session["giohangsp"] as string; }
            set { session["giohangsp"] = value; }
        }

      
    }
}
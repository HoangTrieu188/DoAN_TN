using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

namespace Gemini.Controllers.Bussiness
{
    public class SitesLanguage
    {
           public static List<Languages> AvailableLanguages = new List<Languages>
                    {
                             new Languages{ LangFullName = "Việt nam", LangCultureName = "vi-VN"},
                              new Languages{ LangFullName = "English", LangCultureName = "en-GB"},
                             new Languages{ LangFullName = "USA", LangCultureName = "en-US"},
                    };
 
                    public static bool IsLanguageAvailable(string lang)
                    {
                        return AvailableLanguages.FirstOrDefault(a => a.LangCultureName.Equals(lang)) != null;
                    }
 
                    public static string GetDefaultLanguage()
                    {
                        return AvailableLanguages[0].LangCultureName;
                    }

                    public void SetLanguage(string lang)
                    {

                        try
                        {
                            if (!IsLanguageAvailable(lang))
                                lang = GetDefaultLanguage();
                            var cultureInfo = new CultureInfo(lang);
                            Thread.CurrentThread.CurrentUICulture = cultureInfo;
                            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
                            HttpCookie langCookie = new HttpCookie("culture", lang);
                            langCookie.Expires = DateTime.Now.AddYears(1);
                            HttpContext.Current.Response.Cookies.Add(langCookie);

                            string changlang = "EN";
                            if (lang == "vi-VN")
                            {
                                changlang = "VI";
                            }

                            HttpCookie langCookie1 = new HttpCookie("language", changlang);
                            langCookie1.Expires = DateTime.Now.AddYears(1);
                            HttpContext.Current.Response.Cookies.Add(langCookie1);

                        }
                        catch
                        {

                        }
                    }

    }
                public class Languages
                {
                    public string LangFullName { get; set; }
                    public string LangCultureName { get; set; }
                }
    
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Resources;
using System.Web.Mvc;
using Gemini.Controllers.Bussiness;
using Gemini.Models;
using Gemini.Models._01_Hethong;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SINNOVA.Core;

namespace Gemini.Controllers._01_Hethong
{

    public class ResourceController : GeminiController
    {
        public ResourceController()
        {
        }

        public ActionResult Index()
        {

            return RedirectToAction("List");
        }

        /// <summary>
        /// List view grid
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            GetSettingUser();
            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request, string tukhoa, string filename)
        {
            if (filename.IsNullOrEmpty())
            {
                filename = @"Resource.resx";
            }
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"Resources\" + filename;
            var resSet = new ResXResourceSet(filePath);
            IDictionaryEnumerator dict = resSet.GetEnumerator();
            var listResourceModel = new List<ResourceModel>();
            while (dict.MoveNext())
            {
                var key = (string)dict.Key;
                var resource = new ResourceModel { Key = key, Value = resSet.GetString(key) };
                listResourceModel.Add(resource);
            }

            var find = new FINDBASEModel { tukhoa = vString.GetValueTostring(tukhoa) };
            DataSourceResult result =
                listResourceModel.Where(s => s.Value.Contains(find.tukhoa))
                    .OrderBy(s => s.Key)
                    .ToDataSourceResult(request);
            return Json(result);
        }


        private static void AddOrUpdateResource(string key, string value, string filePath)
        {

            List<DictionaryEntry> resx;
            using (var reader = new ResXResourceReader(filePath))
            {
                resx = reader.Cast<DictionaryEntry>().ToList();
                var existingResource = resx.FirstOrDefault(r => r.Key.ToString() == key);
                {
                    var modifiedResx = new DictionaryEntry() { Key = existingResource.Key, Value = value };
                    resx.Remove(existingResource); // REMOVING RESOURCE!
                    resx.Add(modifiedResx); // AND THEN ADDING RESOURCE!
                }
            }
            using (var writer = new ResXResourceWriter(filePath))
            {
                resx.ForEach(r => writer.AddResource(r.Key.ToString(), r.Value.ToString()));
                writer.Generate();
            }
        }


        private static void DeleteResource(string key, string value, string filePath)
        {

            List<DictionaryEntry> resx;
            using (var reader = new ResXResourceReader(filePath))
            {
                resx = reader.Cast<DictionaryEntry>().ToList();
                var existingResource = resx.FirstOrDefault(r => r.Key.ToString() == key);
                {
                    var modifiedResx = new DictionaryEntry() { Key = existingResource.Key, Value = value };
                    resx.Remove(existingResource); // REMOVING RESOURCE!
                }
            }
            using (var writer = new ResXResourceWriter(filePath))
            {
                resx.ForEach(r => writer.AddResource(r.Key.ToString(), r.Value.ToString()));
                writer.Generate();
            }
        }
        public ActionResult Edit(string key, string filename)
        {
            try
            {
                if (filename.IsNullOrEmpty())
                {
                    filename = @"Resource.resx";
                }

                string filePath = AppDomain.CurrentDomain.BaseDirectory + @"Resources\" + filename;
                List<DictionaryEntry> resx;
                ResourceModel viewModel;
                using (var reader = new ResXResourceReader(filePath))
                {
                    resx = reader.Cast<DictionaryEntry>().ToList();
                    var existingResource = resx.FirstOrDefault(r => r.Key.ToString() == key);
                    {
                        viewModel = new ResourceModel() { Key = existingResource.Key.ToString(), Value = existingResource.Value.ToString(), FilePath = filePath };
                    }
                }
                return PartialView("Edit", viewModel);
            }
            catch (Exception)
            {
                return Redirect("/Error/ErrorList");
            }

        }


        public ActionResult Delete(string key, string filename)
        {
            try
            {
                if (filename.IsNullOrEmpty())
                {
                    filename = @"Resource.resx";
                }

                string filePath = AppDomain.CurrentDomain.BaseDirectory + @"Resources\" + filename;
                List<DictionaryEntry> resx;
                ResourceModel viewModel;
                using (var reader = new ResXResourceReader(filePath))
                {
                    resx = reader.Cast<DictionaryEntry>().ToList();
                    var existingResource = resx.FirstOrDefault(r => r.Key.ToString() == key);
                    {
                        viewModel = new ResourceModel() { Key = existingResource.Key.ToString(), Value = existingResource.Value.ToString(), FilePath = filePath };
                    }
                }

                DeleteResource(viewModel.Key, viewModel.Value, viewModel.FilePath);
                DataReturn.ActiveId = 0;
                DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }

            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update(ResourceModel viewModel)
        {
            try
            {
                AddOrUpdateResource(viewModel.Key, viewModel.Value, viewModel.FilePath);
                DataReturn.StatusCode = 200;
                DataReturn.ActiveId = 0;
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }





    }
}

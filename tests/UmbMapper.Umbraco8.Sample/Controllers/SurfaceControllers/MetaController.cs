using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UmbMapper.Extensions;
using UmbMapper.Umbraco8.Sample.Models.UmbracoDocTypes;
using UmbMapper.Umbraco8.Sample.Models.ViewModels;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.Mvc;

namespace UmbMapper.Umbraco8.Sample.Controllers.SurfaceControllers
{
    public class MetaController : SurfaceController
    {
        [ChildActionOnly]
        //public PartialViewResult PageMeta()
        public ActionResult PageMeta()
        {
            var siteRoot = this.GetSiteRoot();
            var pageMeta = this.CurrentPage.MapTo<MetaDataComposition>();

            string metaTitle;
            if (string.IsNullOrWhiteSpace(siteRoot?.SiteName) == false)
            {
                metaTitle = $"{siteRoot.SiteName} - {pageMeta.MetaDescription}";
            }
            else
            {
                metaTitle = pageMeta.MetaTitle;
            }

            PageMetaViewModel vm = new PageMetaViewModel
            {
                MetaTitle = metaTitle,
                MetaDescription = pageMeta.MetaDescription
            };


            return PartialView("~/Views/Partials/Meta/_PageMeta.cshtml", vm);
        }

        private SiteRoot GetSiteRoot()
        {
            IPublishedContent root = Umbraco?.ContentAtRoot()?.FirstOrDefault();

            if (root != null)
            {
                return root.MapTo<SiteRoot>();
            }

            return null;
        }
    }
}
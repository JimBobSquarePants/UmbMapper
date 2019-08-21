using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UmbMapper.Extensions;
using UmbMapper.Umbraco8.Sample.Models.UmbracoDocTypes;
using UmbMapper.Umbraco8.Sample.Models.ViewModels;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors.ValueConverters;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace UmbMapper.Umbraco8.Sample.Controllers.SurfaceControllers
{
    public class HeaderController : SurfaceController
    {
        [ChildActionOnly]
        public ActionResult BasicHeader()
        {
            var header = this.CurrentPage.MapTo<BasicHeaderComposition>();

            BasicHeaderViewModel vm = new BasicHeaderViewModel
            {
                ImageUrl = header.Image.Url,
                Strapline = header.Strapline,
                Link = header.Link
            };


            return PartialView("~/Views/Partials/Header/_BasicHeader.cshtml", vm);
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
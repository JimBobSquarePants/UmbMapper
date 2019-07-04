using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UmbMapper.Extensions;
using UmbMapper.Umbraco8.Sample.Models.Pages;
using UmbMapper.Umbraco8.Sample.RenderModels;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace UmbMapper.Umbraco8.Sample.Controllers.RenderMvcControllers
{
    public class HomeController : RenderMvcController
    {
        /// <inheritdoc/>
        public override ActionResult Index(ContentModel model)
        {
            Home home = model.Content.MapTo<Home>();
            var viewModel = new RenderPage<Home>(home);

            foreach (var s in viewModel.Content.Gallery)
            {
                var slide = s;
                var image = slide.Image;
            }

            return this.CurrentTemplate(viewModel);
        }
    }
}
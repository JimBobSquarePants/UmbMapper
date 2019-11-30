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
        private readonly IUmbMapperService umbMapperService;
        public HomeController(IUmbMapperService umbMapperService)
        {
            this.umbMapperService = umbMapperService;
        }
        /// <inheritdoc/>
        public override ActionResult Index(ContentModel model)
        {
            Home home = this.umbMapperService.MapTo<Home>(model.Content); //model.Content.MapTo<Home>();
            var viewModel = new RenderPage<Home>(home);

            return this.CurrentTemplate(viewModel);
        }
    }
}
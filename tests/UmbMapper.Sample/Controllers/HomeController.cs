// <copyright file="HomeController.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System.Web.Mvc;
using UmbMapper.Extensions;
using UmbMapper.Sample.Models.Pages;
using UmbMapper.Sample.RenderModels;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace UmbMapper.Sample.Controllers
{
    /// <summary>
    /// The home page controller
    /// </summary>
    public class HomeController : RenderMvcController
    {
        // GET: Home
        public override ActionResult Index(RenderModel model)
        {
            Home home = model.Content.MapTo<Home>();
            var viewModel = new RenderPage<Home>(home);

            return this.CurrentTemplate(viewModel);
        }
    }
}
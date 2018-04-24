// <copyright file="BlogController.cs" company="James Jackson-South">
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
    /// The blog page controller
    /// </summary>
    public class BlogController : RenderMvcController
    {
        /// <inheritdoc/>
        public override ActionResult Index(RenderModel model)
        {
            Blog blog = model.Content.MapTo<Blog>();
            var viewModel = new RenderPage<Blog>(blog);

            return this.CurrentTemplate(viewModel);
        }
    }
}
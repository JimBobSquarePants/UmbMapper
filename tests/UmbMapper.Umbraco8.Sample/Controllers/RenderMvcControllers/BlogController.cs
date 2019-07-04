// <copyright file="BlogController.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System.Web.Mvc;
using UmbMapper.Extensions;
using UmbMapper.Umbraco8.Sample.Models.Pages;
using UmbMapper.Umbraco8.Sample.RenderModels;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace UmbMapper.Umbraco8.Sample.Controllers.RenderMvcControllers
{
    /// <summary>
    /// The blog page controller
    /// </summary>
    public class BlogController : RenderMvcController
    {
        /// <inheritdoc/>
        public override ActionResult Index(ContentModel model)
        {
            Blog blog = model.Content.MapTo<Blog>();
            var viewModel = new RenderPage<Blog>(blog);

            return this.CurrentTemplate(viewModel);
        }
    }
}
// <copyright file="PostController.cs" company="James Jackson-South">
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
    /// The post page controller
    /// </summary>
    public class PostController : RenderMvcController
    {
        /// <inheritdoc/>
        public override ActionResult Index(RenderModel model)
        {
            Post post = model.Content.MapTo<Post>();
            var viewModel = new RenderPage<Post>(post);

            return this.CurrentTemplate(viewModel);
        }
    }
}
// <copyright file="PostController.cs" company="James Jackson-South">
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
    /// The post page controller
    /// </summary>
    public class PostController : RenderMvcController
    {
        private readonly IUmbMapperService umbMapperService;
        public PostController(IUmbMapperService umbMapperService)
        {
            this.umbMapperService = umbMapperService;
        }
        /// <inheritdoc/>
        public override ActionResult Index(ContentModel model)
        {
            Post post = this.umbMapperService.MapTo<Post>(model.Content); //model.Content.MapTo<Post>();
            var viewModel = new RenderPage<Post>(post);

            return this.CurrentTemplate(viewModel);
        }
    }
}
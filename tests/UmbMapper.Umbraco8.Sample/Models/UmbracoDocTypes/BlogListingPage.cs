﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UmbMapper.Umbraco8.Sample.Models.UmbracoDocTypes
{
    public class BlogListingPage
    {
        public int ItemsPerPage { get; set; }
        public IHtmlString BodyText { get; set; }
    }
}
﻿using MediaBrowser.Common.Extensions;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Entities;
using ServiceStack;
using System;
using System.IO;
using System.Linq;

namespace MediaBrowser.Api.Images
{
    /// <summary>
    /// Class GetGeneralImage
    /// </summary>
    [Route("/Images/General/{Name}/{Type}", "GET")]
    [Api(Description = "Gets a general image by name")]
    public class GetGeneralImage
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [ApiMember(Name = "Name", Description = "The name of the image", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string Name { get; set; }

        [ApiMember(Name = "Type", Description = "Image Type (primary, backdrop, logo, etc).", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Class GetRatingImage
    /// </summary>
    [Route("/Images/Ratings/{Theme}/{Name}", "GET")]
    [Api(Description = "Gets a rating image by name")]
    public class GetRatingImage
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [ApiMember(Name = "Name", Description = "The name of the image", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>The theme.</value>
        [ApiMember(Name = "Theme", Description = "The theme to get the image from", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string Theme { get; set; }
    }

    /// <summary>
    /// Class GetMediaInfoImage
    /// </summary>
    [Route("/Images/MediaInfo/{Theme}/{Name}", "GET")]
    [Api(Description = "Gets a media info image by name")]
    public class GetMediaInfoImage
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [ApiMember(Name = "Name", Description = "The name of the image", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>The theme.</value>
        [ApiMember(Name = "Theme", Description = "The theme to get the image from", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string Theme { get; set; }
    }

    /// <summary>
    /// Class ImageByNameService
    /// </summary>
    public class ImageByNameService : BaseApiService
    {
        /// <summary>
        /// The _app paths
        /// </summary>
        private readonly IServerApplicationPaths _appPaths;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageByNameService" /> class.
        /// </summary>
        /// <param name="appPaths">The app paths.</param>
        public ImageByNameService(IServerApplicationPaths appPaths)
        {
            _appPaths = appPaths;
        }

        /// <summary>
        /// Gets the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>System.Object.</returns>
        public object Get(GetGeneralImage request)
        {
            var filename = string.Equals(request.Type, "primary", StringComparison.OrdinalIgnoreCase)
                               ? "folder"
                               : request.Type;

            var paths = BaseItem.SupportedImageExtensions.Select(i => Path.Combine(_appPaths.GeneralPath, request.Name, filename + i)).ToList();

            var path = paths.FirstOrDefault(File.Exists) ?? paths.FirstOrDefault();

            return ToStaticFileResult(path);
        }

        /// <summary>
        /// Gets the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>System.Object.</returns>
        public object Get(GetRatingImage request)
        {
            var themeFolder = Path.Combine(_appPaths.RatingsPath, request.Theme);

            if (Directory.Exists(themeFolder))
            {
                var path = BaseItem.SupportedImageExtensions.Select(i => Path.Combine(themeFolder, request.Name + i))
                    .FirstOrDefault(File.Exists);

                if (!string.IsNullOrEmpty(path))
                {
                    return ToStaticFileResult(path);
                }
            }

            var allFolder = Path.Combine(_appPaths.RatingsPath, "all");

            if (Directory.Exists(allFolder))
            {
                // Avoid implicitly captured closure
                var currentRequest = request;

                var path = BaseItem.SupportedImageExtensions.Select(i => Path.Combine(allFolder, currentRequest.Name + i))
                    .FirstOrDefault(File.Exists);

                if (!string.IsNullOrEmpty(path))
                {
                    return ToStaticFileResult(path);
                }
            }

            throw new ResourceNotFoundException("MediaInfo image not found: " + request.Name);
        }

        /// <summary>
        /// Gets the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>System.Object.</returns>
        public object Get(GetMediaInfoImage request)
        {
            var themeFolder = Path.Combine(_appPaths.MediaInfoImagesPath, request.Theme);

            if (Directory.Exists(themeFolder))
            {
                var path = BaseItem.SupportedImageExtensions.Select(i => Path.Combine(themeFolder, request.Name + i))
                    .FirstOrDefault(File.Exists);

                if (!string.IsNullOrEmpty(path))
                {
                    return ToStaticFileResult(path);
                }
            }

            var allFolder = Path.Combine(_appPaths.MediaInfoImagesPath, "all");

            if (Directory.Exists(allFolder))
            {
                // Avoid implicitly captured closure
                var currentRequest = request;

                var path = BaseItem.SupportedImageExtensions.Select(i => Path.Combine(allFolder, currentRequest.Name + i))
                    .FirstOrDefault(File.Exists);
                
                if (!string.IsNullOrEmpty(path))
                {
                    return ToStaticFileResult(path);
                }
            }

            throw new ResourceNotFoundException("MediaInfo image not found: " + request.Name);
        }
    }
}

using AspNetCore.Identity.Dapper;
using Daarto.WebUI.Infrastructure.Filters;
using Daarto.WebUI.Infrastructure.Settings;
using Daarto.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Daarto.WebUI.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class FileController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<FileController> _logger;
        private readonly AppSettings _appSettings;
        private readonly UserManager<ApplicationUser> _userManager;

        public FileController(IHostingEnvironment hostingEnvironment, ILogger<FileController> logger, UserManager<ApplicationUser> userManager,
            IOptions<AppSettings> appSettings)
        {
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _appSettings = appSettings.Value;
            _userManager = userManager;
        }

        [HttpPost]
        [DisableFormValueModelBinding]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload()
        {
            string multipartBoundary = Request.GetMultipartBoundary();

            if (string.IsNullOrEmpty(multipartBoundary))
            {
                Response.StatusCode = 400;

                return Json(new UploadResult
                {
                    Succeeded = false,
                    Description = $"Expected a multipart request, but got '{Request.ContentType}'."
                });
            }

            var formAccumulator = new KeyValueAccumulator();
            var reader = new MultipartReader(multipartBoundary, HttpContext.Request.Body);
            MultipartSection section = await reader.ReadNextSectionAsync();
            string fileGuid = Guid.NewGuid().ToString();
            string userId = _userManager.GetUserId(HttpContext.User);
            string fileExtension = string.Empty;

            while (section != null)
            {
                FileMultipartSection fileSection = section.AsFileSection();

                if (fileSection != null)
                {
                    string fileName = fileSection.FileName;
                    fileExtension = Path.GetExtension(fileName);
                    string targetFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.UploadsFolder, userId);
                    string targetFilePath = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.UploadsFolder, userId, $"{fileGuid}{fileExtension}");

                    if (!Directory.Exists(targetFolderPath))
                    {
                        Directory.CreateDirectory(targetFolderPath);
                    }

                    using (var targetStream = System.IO.File.Create(targetFilePath))
                    {
                        await fileSection.FileStream.CopyToAsync(targetStream);
                        _logger.LogInformation($"Copied the uploaded file '{fileName}' to '{targetFilePath}'.");
                    }
                }
                else
                {
                    FormMultipartSection formSection = section.AsFormDataSection();

                    if (formSection != null)
                    {
                        string name = formSection.Name;
                        string value = await formSection.GetValueAsync();
                        formAccumulator.Append(name, value);

                        if (formAccumulator.ValueCount > FormReader.DefaultValueCountLimit)
                        {
                            throw new InvalidDataException($"Form key count limit {FormReader.DefaultValueCountLimit} exceeded.");
                        }
                    }
                }

                section = await reader.ReadNextSectionAsync();
            }

            var uploadResult = new UploadResult
            {
                Succeeded = true,
                Description = "File was uploaded successfully",
                FileUrl = $"{_appSettings.Domain}/{_appSettings.UploadsFolder}/{userId}/{fileGuid}{fileExtension}",
                FileName = $"{fileGuid}{fileExtension}"
            };

            return Json(uploadResult);
        }
    }
}
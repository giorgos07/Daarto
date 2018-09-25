using System;
using System.IO;
using System.Threading.Tasks;
using AspNetCore.Identity.Dapper;
using Daarto.Infrastructure.Filters;
using Daarto.Infrastructure.Settings;
using Daarto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Daarto.WebUI.Controllers
{
    [Authorize(Roles = "Administrator")]
    [Route("file")]
    public class FileController : Controller
    {
        public const string Name = "File";
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<FileController> _logger;
        private readonly AppSettings _appSettings;
        private readonly UserManager<ApplicationUser> _userManager;

        public FileController(IHostingEnvironment hostingEnvironment, ILogger<FileController> logger, UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings) {
            if (appSettings == null) {
                throw new ArgumentNullException(nameof(appSettings));
            }

            _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings.Value;
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [HttpPost("upload")]
        [DisableFormValueModelBinding]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<UploadResult>> Upload() {
            var multipartBoundary = Request.GetMultipartBoundary();

            if (string.IsNullOrEmpty(multipartBoundary)) {
                return BadRequest($"Expected a multipart request, but got '{Request.ContentType}'.");
            }

            var formAccumulator = new KeyValueAccumulator();
            var reader = new MultipartReader(multipartBoundary, HttpContext.Request.Body);
            var section = await reader.ReadNextSectionAsync();
            var fileGuid = Guid.NewGuid();
            var userId = _userManager.GetUserId(HttpContext.User);
            var fileExtension = string.Empty;

            while (section != null) {
                var fileSection = section.AsFileSection();

                if (fileSection != null) {
                    var fileName = fileSection.FileName;
                    fileExtension = Path.GetExtension(fileName);
                    var targetFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.UploadsFolder, userId);
                    var targetFilePath = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.UploadsFolder, userId, $"{fileGuid}{fileExtension}");

                    if (!Directory.Exists(targetFolderPath)) {
                        Directory.CreateDirectory(targetFolderPath);
                    }

                    using (var targetStream = System.IO.File.Create(targetFilePath)) {
                        await fileSection.FileStream.CopyToAsync(targetStream);
                        _logger.LogInformation($"Copied the uploaded file '{fileName}' to '{targetFilePath}'.");
                    }
                } else {
                    var formSection = section.AsFormDataSection();

                    if (formSection != null) {
                        var name = formSection.Name;
                        var value = await formSection.GetValueAsync();
                        formAccumulator.Append(name, value);

                        if (formAccumulator.ValueCount > FormReader.DefaultValueCountLimit) {
                            throw new InvalidDataException($"Form key count limit {FormReader.DefaultValueCountLimit} exceeded.");
                        }
                    }
                }

                section = await reader.ReadNextSectionAsync();
            }

            return Ok(new UploadResult {
                Succeeded = true,
                Description = "File was uploaded successfully",
                FileUrl = $"{_appSettings.Domain}/{_appSettings.UploadsFolder}/{userId}/{fileGuid}{fileExtension}",
                FileName = $"{fileGuid}{fileExtension}"
            });
        }
    }
}

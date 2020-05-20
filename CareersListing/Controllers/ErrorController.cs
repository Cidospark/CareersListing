using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;
        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("Error/{statusCode}")]
        [AllowAnonymous]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeReExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            switch (statusCode)
            {
                case 403:
                    ViewBag.ErrorMessage = $"{statusCode}: Forbidden!";
                    _logger.LogWarning($"403 Error Occurred. Path={statusCodeReExecuteFeature.OriginalPath}" +
                        $"and QueryString = {statusCodeReExecuteFeature.OriginalQueryString}");
                    break;
                case 404:
                    ViewBag.ErrorMessage = $"{statusCode}: Sorry! resource Not Found.";
                    _logger.LogWarning($"404 Error Occurred. Path={statusCodeReExecuteFeature.OriginalPath}" +
                        $"and QueryString = {statusCodeReExecuteFeature.OriginalQueryString}");
                    break;
                default:
                    ViewBag.ErrorMessage = $"An error occured while processing your request. For emergencies or " +
                        "enquiries, please contact us on sample@sample.com";
                    _logger.LogWarning($"{statusCode} Error Occurred. Path={statusCodeReExecuteFeature.OriginalPath}" +
                        $"and QueryString = {statusCodeReExecuteFeature.OriginalQueryString}");
                    break;
            }

            return View("NotFound");
        }

        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            _logger.LogError($"Error path: {exceptionHandlerPathFeature.Path}, Error thrown: {exceptionHandlerPathFeature.Error}");

            return View("Error");
        }
    }
}

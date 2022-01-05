using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Service.Core.DTOs;
using Service.Core.Services.Interfaces;
using Service.Web.Filters;
using Service.Web.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Service.Web.Controllers
{
    [Route("v1.0.0/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly ILogger<HotelsController> _logger;
        private IPropertyService _propertyService;
        private IRoomService _roomService;

        private readonly string[] _permittedExtensions = { ".txt" };
        private readonly long _fileSizeLimit;
        
        // Get the default form options so that we can use them to set the default 
        // limits for request body data.
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        public HotelsController(
            ILogger<HotelsController> logger,
            IPropertyService propertyService,
            IRoomService roomService,
            IConfiguration config
            )
        {
            _logger = logger;
            _propertyService = propertyService;
            _roomService = roomService;

            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
        }

        #region Properties
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                _logger.LogTrace("Get Hotel Controller");
                // _logger.LogDebug("Get Hotel Controller");
                // _logger.LogInformation("Get Hotel Controller");
                return Ok(_propertyService.Get());
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "error: {0}", e.Message);
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            try
            {
                return Ok(_propertyService.Get(id));
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "error: {0}", e.Message);
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult Add(PropertyInput property)
        {
            try
            {
                return Ok(_propertyService.Add(property));
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "error: {0}", e.Message);
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                _propertyService.Delete(id);

                return Ok();
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "error: {0}", e.Message);
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public IActionResult Edit(string id, PropertyInput property)
        {
            try
            {
                _propertyService.Edit(id, property);

                return Ok();
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "error: {0}", e.Message);
                return BadRequest();
            }
        }

        /// <summary>
        /// https://github.com/dotnet/AspNetCore.Docs/tree/master/aspnetcore/mvc/models/file-uploads/samples/3.x/SampleApp
        /// </summary>
        /// <returns></returns>
        [HttpPost("{id}/UploadImage")]
        [DisableFormValueModelBinding]
        //[ValidateAntiForgeryToken] //for mvc app to check is it true send it
        public async Task<IActionResult> UploadPhysical(string id)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                ModelState.AddModelError("File",
                    $"The request couldn't be processed (Error 1).");
                // Log error

                return BadRequest(ModelState);
            }

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);
            var section = await reader.ReadNextSectionAsync();

            List<string> strFile = new List<string>();

            while (section != null)
            {
                var hasContentDispositionHeader =
                    ContentDispositionHeaderValue.TryParse(
                        section.ContentDisposition, out var contentDisposition);

                if (hasContentDispositionHeader)
                {
                    // This check assumes that there's a file
                    // present without form data. If form data
                    // is present, this method immediately fails
                    // and returns the model error.
                    if (!MultipartRequestHelper
                        .HasFileContentDisposition(contentDisposition))
                    {
                        ModelState.AddModelError("File",
                            $"The request couldn't be processed (Error 2).");
                        // Log error

                        return BadRequest(ModelState);
                    }
                    else
                    {
                        // Don't trust the file name sent by the client. To display
                        // the file name, HTML-encode the value.
                        var trustedFileNameForDisplay = WebUtility.HtmlEncode(
                                contentDisposition.FileName.Value);
                        //var trustedFileNameForFileStorage = Path.GetRandomFileName();

                        var trustedFileNameForFileStorage = Path.Combine(id, trustedFileNameForDisplay);

                        // **WARNING!**
                        // In the following example, the file is saved without
                        // scanning the file's contents. In most production
                        // scenarios, an anti-virus/anti-malware scanner API
                        // is used on the file before making the file available
                        // for download or for use by other systems. 
                        // For more information, see the topic that accompanies 
                        // this sample.

                        var streamedFileContent = await FileHelpers.ProcessStreamedFile(
                            section, contentDisposition, ModelState,
                            _permittedExtensions, _fileSizeLimit);

                        if (!ModelState.IsValid)
                        {
                            return BadRequest(ModelState);
                        }

                        //trustedFileNameForFileStorage = Path.Combine(_targetFilePath, trustedFileNameForFileStorage);
                        //save to physical
                        strFile.Add(await _propertyService.SaveFilesAsync(streamedFileContent, trustedFileNameForFileStorage));
                    }
                }

                // Drain any remaining section body that hasn't been consumed and
                // read the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            if(strFile.Count > 1)
                return Created(nameof(HotelsController), strFile);
            else
                return Created(nameof(HotelsController), strFile[0]);
        }

        [HttpPost("DeleteImage")]
        public IActionResult DeleteFile(ImageInput lImage)
        {
            try
            {
                _propertyService.DeleteFiles(lImage.uri);

                return Ok();
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "error: {0}", e.Message);
                return BadRequest();
            }
        }
        #endregion

        #region rooms
        [HttpGet("{id}/room")]
        public IActionResult GetRooms(string id)
        {
            try
            {
                return Ok(_roomService.Get(id));
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "error: {0}", e.Message);
                return BadRequest();
            }
        }

        [HttpGet("{id}/room/{roomId}")]
        public IActionResult GetRoom(string id, string roomId)
        {
            try
            {
                return Ok(_roomService.Get(id, roomId));
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "error: {0}", e.Message);
                return BadRequest();
            }
        }

        [HttpPost("{id}/room")]
        public IActionResult AddRooms(string id, RoomInput room)
        {
            try
            {
                return Ok(_roomService.Add(id, room));
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "error: {0}", e.Message);
                return BadRequest();
            }
        }

        [HttpPut("{id}/room/{roomId}")]
        public IActionResult EditRooms(string id, string roomId, RoomInput room)
        {
            try
            {
                _roomService.Edit(id, roomId, room);

                return Ok();
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "error: {0}", e.Message);
                return BadRequest();
            }
        }

        [HttpDelete("{id}/room/{roomId}")]
        public IActionResult DeleteRooms(string id, string roomId)
        {
            try
            {
                _roomService.Delete(id, roomId);

                return Ok();
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "error: {0}", e.Message);
                return BadRequest();
            }
        }
        #endregion
    }
}
using AutoMapper;
using Service.Core.DTOs;
using Service.Core.Services.Interfaces;
using Service.Domain.Entities;
using Service.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;

namespace Service.Core.Services.Implementation
{
    public class PropertyService : IPropertyService
    {
        private readonly ILogger<PropertyService> _logger;
        private IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private IStorage _storage;
        // private readonly IMessageSenderService _messageSenderService;
        // private readonly IConfiguration _configuration;

        public PropertyService(
            ILogger<PropertyService> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            // IMessageSenderService messageSenderService,
            // IConfiguration configuration,
            IStorage storage
            )
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _storage = storage;
            // _messageSenderService = messageSenderService;
            // _configuration = configuration;
        }

        public IList<PropertyOutput> Get()
        {
            try
            {
                _logger.LogInformation("Get Hotel Service");

                // // var notif = new ImageInput()
                // // {
                // //     uri = new List<string>(){"123", "345", "567"}
                // // };

                // // var str = JsonConvert.SerializeObject(notif);
                // // var body = Encoding.ASCII.GetBytes(str);
                // var body = "haloooo";

                // var topics = _configuration.GetValue<string>("Messaging:Topics:Ping");

                // try
                // {
                //     _messageSenderService.Send(topics, body);
                // }
                // catch (Exception e)
                // {
                //     _logger.LogError(e, "error send event: {0}", e.Message);
                // }

                return _mapper.Map<List<PropertyOutput>>(_unitOfWork.PropertyRepository.GetByPage(10, 1));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error: {0}", e.Message);
                throw e;
            }
        }

        public PropertyOutput Get(string id)
        {
            try
            {
                return _mapper.Map<PropertyOutput>(_unitOfWork.PropertyRepository.GetById(Guid.Parse(id)));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error: {0}", e.Message);
                throw e;
            }
        }

        public IList<PropertyOutput> Page(int iPage, int iSize)
        {
            try
            {
                return _mapper.Map<List<PropertyOutput>>(_unitOfWork.PropertyRepository.GetByPage(iSize, iPage));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error: {0}", e.Message);
                throw e;
            }
        }

        public PropertyOutput Add(PropertyInput newProperty)
        {
            try
            {
                Property property = _mapper.Map<Property>(newProperty);
                property.Id = Guid.NewGuid();
                property.UpdatedDate = property.CreateDate = DateTime.UtcNow;

                var newData = _unitOfWork.PropertyRepository.Add(property);
                _unitOfWork.Save();

                return _mapper.Map<PropertyOutput>(newData);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error: {0}", e.Message);
                throw e;
            }
        }

        public void Delete(string strGuid)
        {
            try
            {
                _unitOfWork.PropertyRepository.Delete(_unitOfWork.PropertyRepository.GetById(Guid.Parse(strGuid)));
                _unitOfWork.Save();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error: {0}", e.Message);
                throw e;
            }
        }

        public void Edit(string id, PropertyInput newProperty)
        {
            try
            {
                Property old = _unitOfWork.PropertyRepository.GetById(Guid.Parse(id));
                old.UpdatedDate = DateTime.UtcNow;

                _mapper.Map(newProperty, old);

                _unitOfWork.PropertyRepository.Edit(old);
                _unitOfWork.Save();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error: {0}", e.Message);
                throw e;
            }
        }

        public async Task<string> SaveFilesAsync(byte[] streamedFileContent, string fileName)
        {
            try
            {
                string container = "Service";

                return await _storage.SaveFileAsync(streamedFileContent, container, fileName);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error: {0}", e.Message);
                throw e;
            }
        }

        public void DeleteFiles(List<string> uri)
        {
            try
            {
                _storage.DeleteFile(uri[0]);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error: {0}", e.Message);
                throw e;
            }
        }
    }
}

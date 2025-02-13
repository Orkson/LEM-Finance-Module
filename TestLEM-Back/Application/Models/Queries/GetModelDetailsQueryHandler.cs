using Application.Abstractions;
using Domain.Abstraction;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Models.Queries
{
    public class GetModelDetailsQueryHandler : IRequestHandler<GetModelDetailsQuery, ModelDetailsDto>
    {
        private readonly IModelRepository _modelRepository;
        private readonly IModelCooperationRepository _modelCooperationRepository;
        private readonly IApplicationDbContext _dbContext;
        private readonly IDeviceRepository _deviceRepository;

        public GetModelDetailsQueryHandler(IModelRepository modelRepository, IModelCooperationRepository modelCooperationRepository, IApplicationDbContext dbContext, IDeviceRepository deviceRepository)
        {
            _modelRepository = modelRepository;
            _modelCooperationRepository = modelCooperationRepository;
            _dbContext = dbContext;
            _deviceRepository = deviceRepository;
        }

        public async Task<ModelDetailsDto> Handle(GetModelDetailsQuery request, CancellationToken cancellationToken)
        {
            var model = await _modelRepository.GetModelById(request.modelId, cancellationToken);
            var modelsWithNameCount = _deviceRepository.TotalDevicesByModelCount(request.modelName);

            var modelDetailsDto = new ModelDetailsDto
            {
                Id = model.Id,
                TotalModelCount = modelsWithNameCount,
                Name = model.Name,
                SerialNumber = model.SerialNumber,
                CompanyName = model.Company?.Name,
                MeasuredValues = GetMeasuredValues(request.modelId),
                ModelDocuments = GetDocuments(request.modelId),
                RelatedModelsDetails = await GetRelatedModelsAsync(model.Id, cancellationToken)
            };
            return modelDetailsDto;
        }

        //TODO: move to separate repository
        private List<MeasuredValueDto> GetMeasuredValues(int modelId)
        {
            var measuredValues = _dbContext.MeasuredValues
                .Where(x => x.ModelId == modelId)
                .Select(x => new MeasuredValueDto
                {
                    Id = x.Id,
                    PhysicalMagnitudeName = x.PhysicalMagnitude.Name,
                    PhysicalMagnitudeUnit = x.PhysicalMagnitude.Unit,
                    MeasuredRanges = (ICollection<MeasuredRangesDto>)x.MeasuredRanges.Select(y => new MeasuredRangesDto
                    {
                        Id = y.Id,
                        Range = y.Range,
                        AccuracyInPercent = y.AccuracyInPercet
                    })
                }).ToList();

            return measuredValues;
        }

        //TODO: move to separate repository
        private ICollection<DocumentDto>? GetDocuments(int modelId)
        {
            var modelDocuments = _dbContext.Documents.Where(x => x.ModelId == modelId).ToList();
            if (!modelDocuments.Any())
            {
                return null;
            }

            return GetDocumentsDto(modelDocuments);
        }

        private List<DocumentDto> GetDocumentsDto(ICollection<Document>? documents)
        {
            var result = new List<DocumentDto>();
            foreach (var document in documents)
            {
                var documentDto = new DocumentDto
                {
                    Name = document.Name,
                    Format = document.Format,
                };
                result.Add(documentDto);
            }

            return result;
        }

        private async Task<ICollection<ModelDetailsDto>> GetRelatedModelsAsync(int modelId, CancellationToken cancellationToken)
        {
            var cooperatedModels = await _modelCooperationRepository.GetCooperationsForModelByModelId(modelId, cancellationToken);

            var relatedModelsFrom = cooperatedModels.Where(x => x.ModelFromId == modelId)
                .Select(y => new ModelDetailsDto
                {
                    Name = y.ModelTo.Name,
                    SerialNumber = y.ModelTo.SerialNumber,
                }).ToList();

            var relatedModelsTo = cooperatedModels.Where(x => x.ModelToId == modelId)
                .Select(y => new ModelDetailsDto
                {
                    Name = y.ModelFrom.Name,
                    SerialNumber = y.ModelFrom.SerialNumber,
                }).ToList();

            var relatedModels = new List<ModelDetailsDto>();

            if (relatedModelsFrom != null)
            {
                relatedModels.AddRange(relatedModelsFrom);
            }

            if (relatedModelsTo != null)
            {
                relatedModels.AddRange(relatedModelsTo);
            }

            return relatedModels;
        }
    }
}

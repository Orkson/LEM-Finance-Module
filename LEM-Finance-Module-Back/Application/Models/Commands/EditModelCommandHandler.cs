using Application.Abstractions.Messaging;
using AutoMapper;
using Domain.Abstraction;
using Domain.Entities;

namespace Application.Models.Commands
{
    internal class EditModelCommandHandler : ICommandHandler<EditModelCommand, int>
    {
        private readonly IModelRepository _modelRepository;
        private readonly IModelCooperationRepository _modelCooperationRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public EditModelCommandHandler(IModelRepository modelRepository, IModelCooperationRepository modelCooperationRepository, ICompanyRepository companyRepository, IMapper mapper)
        {
            _modelRepository = modelRepository;
            _modelCooperationRepository = modelCooperationRepository;
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<int> Handle(EditModelCommand request, CancellationToken cancellationToken)
        {
            var modelToEdit = await _modelRepository.GetModelById(request.modelId, cancellationToken);
            var newModel = request.newModelDto;

            if (modelToEdit.Name != newModel.Name)
            {
                modelToEdit.Name = newModel.Name;
            }
            if (modelToEdit.SerialNumber != newModel.SerialNumber)
            {
                modelToEdit.SerialNumber = newModel.SerialNumber;
            }
            if (modelToEdit.Company?.Name != newModel.CompanyName)
            {
                modelToEdit = await PrepareModelsCompany(modelToEdit, newModel.CompanyName, cancellationToken);
            }

            if (CheckIfCooperationsChanged(modelToEdit.CooperateTo, newModel.CooperatedModelsIds) || request.cooperationsIdsToBeRemoved?.Count != 0)
            {
                if (newModel.CooperatedModelsIds?.Count > 0)
                {
                    var modelCooperations = new List<int>();
                    var resultIds = new List<int>();
                    if (modelToEdit.CooperateTo?.Count > 0)
                    {
                        foreach (var modelCoopeartion in modelToEdit.CooperateTo)
                        {
                            modelCooperations.Add(modelCoopeartion.ModelFromId);
                        }
                    }
                    foreach (var id in newModel.CooperatedModelsIds)
                    {
                        if (!modelCooperations.Contains(id))
                        {
                            resultIds.Add(id);
                        }
                    }
                    await _modelCooperationRepository.AddModelCooperation(request.modelId, resultIds);
                }

                if (request.cooperationsIdsToBeRemoved != null)
                {
                    await _modelCooperationRepository.RemoveModelCooperations(request.cooperationsIdsToBeRemoved, cancellationToken);
                }
            }

            if (CheckIfMeasuredValuesChanged(modelToEdit.MeasuredValues, newModel.MeasuredValues))
            {
                modelToEdit.MeasuredValues = GetMeasuredValuesForModel(newModel.MeasuredValues);
            }

            var newMappedModel = _mapper.Map<Model>(modelToEdit);
            await _modelRepository.UpdateModelValuesAsync(modelToEdit.Id, newMappedModel, cancellationToken);

            return modelToEdit.Id;
        }


        private static bool CheckIfCooperationsChanged(ICollection<ModelCooperation>? cooperations, ICollection<int>? cooperatedModelsIds)
        {
            if (cooperations?.Count == 0 && cooperatedModelsIds?.Count == 0)
            {
                return false;
            }

            var cooperationsIds = new List<int>();

            foreach (var cooperation in cooperations)
            {
                cooperationsIds.Add(cooperation.ModelFromId);
            }

            if (cooperationsIds.Count != cooperatedModelsIds.Count)
            {
                return true;
            }

            var result = cooperatedModelsIds.OrderBy(x => x).SequenceEqual(cooperationsIds.OrderBy(x => x));


            return result;
        }

        private static bool CheckIfMeasuredValuesChanged(ICollection<MeasuredValue>? oldMeasuredValues, ICollection<MeasuredValueDto>? newMeasuredValues)
        {
            if (oldMeasuredValues == null && newMeasuredValues != null || oldMeasuredValues != null && newMeasuredValues == null || oldMeasuredValues?.Count != newMeasuredValues?.Count)
            {
                return true;
            }
            var oldMeasuredValuesDtos = new List<MeasuredValueDto>();

            //this should be moved to separate mapper
            if (oldMeasuredValues != null && oldMeasuredValues.Count > 0)
            {
                foreach (var measuredValue in oldMeasuredValues)
                {
                    var oldMeasuredValueDto = new MeasuredValueDto
                    {
                        PhysicalMagnitudeName = measuredValue.PhysicalMagnitude.Name,
                        PhysicalMagnitudeUnit = measuredValue.PhysicalMagnitude.Unit,
                        MeasuredRanges = GetMeasuredRangesForMeasuredRange(measuredValue.MeasuredRanges)
                    };
                    oldMeasuredValuesDtos.Add(oldMeasuredValueDto);
                }
            }

            var oldMeasuredRanges = new List<ICollection<MeasuredRangesDto>>();
            var newMeasuredRanges = new List<ICollection<MeasuredRangesDto>>();

            foreach (var oldMeasuredValueDto in oldMeasuredValuesDtos)
            {
                if (oldMeasuredValueDto.MeasuredRanges.Count > 0)
                {
                    oldMeasuredRanges.Add(oldMeasuredValueDto.MeasuredRanges);
                }
            }

            foreach (var newMeasuredRange in newMeasuredValues)
            {
                if (newMeasuredRange.MeasuredRanges.Count > 0)
                {
                    newMeasuredRanges.Add(newMeasuredRange.MeasuredRanges);
                }
            }

            if (oldMeasuredRanges.Count != newMeasuredRanges.Count)
            {
                return true;
            }

            for (int i = 0; i < oldMeasuredRanges.Count; i++)
            {
                for (int j = 0; j < oldMeasuredRanges[i].Count; j++)
                {
                    List<MeasuredRangesDto> oldList = (List<MeasuredRangesDto>)oldMeasuredRanges[i];
                    List<MeasuredRangesDto> newList = (List<MeasuredRangesDto>)newMeasuredRanges[i];

                    if (!CheckIfMeasuredRangeAreTheSame(oldList[j], newList[j]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool CheckIfMeasuredRangeAreTheSame(MeasuredRangesDto oldMeasuredRange, MeasuredRangesDto newMeasuredRange)
        {
            return oldMeasuredRange.Range == newMeasuredRange.Range && oldMeasuredRange.AccuracyInPercent == newMeasuredRange.AccuracyInPercent;
        }


        private static ICollection<MeasuredRangesDto> GetMeasuredRangesForMeasuredRange(ICollection<MeasuredRange>? measuredRanges)
        {
            var measuredRangesDto = new List<MeasuredRangesDto>();

            if (measuredRanges == null)
            {
                return measuredRangesDto;
            }

            foreach (var measuredRange in measuredRanges)
            {
                var measuredRangeDto = new MeasuredRangesDto
                {
                    Range = measuredRange.Range,
                    AccuracyInPercent = measuredRange.AccuracyInPercet
                };
                measuredRangesDto.Add(measuredRangeDto);
            }
            return measuredRangesDto;
        }

        private static bool CheckIfMeasuredRangesChanged(ICollection<MeasuredRange>? oldMeasuredRanges, ICollection<MeasuredRangesDto>? newMeasuredRanges)
        {
            if (oldMeasuredRanges == null && newMeasuredRanges != null)
            {
                return true;
            }
            return false;
        }

        private ICollection<MeasuredValue> GetMeasuredValuesForModel(ICollection<MeasuredValueDto> measuredValuesDtos)
        {
            var measuredValues = new List<MeasuredValue>();
            if (measuredValuesDtos == null)
            {
                return measuredValues;
            }

            foreach (var measuredValueDto in measuredValuesDtos)
            {
                var measuredValue = new MeasuredValue
                {
                    PhysicalMagnitude = new PhysicalMagnitude
                    {
                        Name = measuredValueDto.PhysicalMagnitudeName,
                        Unit = measuredValueDto.PhysicalMagnitudeUnit
                    }
                };

                if (measuredValueDto.MeasuredRanges != null)
                {
                    measuredValue.MeasuredRanges = GetMeasuredRanges(measuredValueDto.MeasuredRanges);
                }
                measuredValues.Add(measuredValue);
            }
            return measuredValues;
        }

        private ICollection<MeasuredRange> GetMeasuredRanges(ICollection<MeasuredRangesDto> measuredRangesDtos)
        {
            var measuredRanges = new List<MeasuredRange>();

            foreach (var measuredRangeDto in measuredRangesDtos)
            {
                var measuredRange = new MeasuredRange
                {
                    Range = measuredRangeDto.Range,
                    AccuracyInPercet = measuredRangeDto.AccuracyInPercent
                };
                measuredRanges.Add(measuredRange);
            }
            return measuredRanges;
        }

        private async Task<Model> PrepareModelsCompany(Model model, string companyName, CancellationToken cancellationToken)
        {
            if(companyName == null)
            {
                model.Company = null;
                return model;
            }

            var companyExists = await _companyRepository.CheckIfCompanyExists(companyName);

            if (!companyExists)
            {
                var company = new Company
                {
                    Name = companyName
                };
                model.Company = company;
            }
            else
            {
                model.CompanyId = await _companyRepository.GetCompanyIdByItsName(companyName, cancellationToken);
            }

            return model;
        }

    }
}

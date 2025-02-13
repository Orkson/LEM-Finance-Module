using Application.Abstractions.Messaging;
using AutoMapper;
using Domain.Abstraction;
using Domain.Entities;
using MediatR;

namespace Application.Models.Commands
{
    internal class CreateModelCommandHandler : ICommandHandler<CreateModelCommand, int>
    {
        private readonly IModelRepository _modelRepository;
        private readonly IModelCooperationRepository _modelCooperationRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;
        private readonly ISender _sender;

        public CreateModelCommandHandler(IModelRepository modelRepository, IModelCooperationRepository modelCooperationRepository,
                                         ICompanyRepository companyRepository, IMapper mapper, ISender sender)
        {
            _companyRepository = companyRepository;
            _modelRepository = modelRepository;
            _modelCooperationRepository = modelCooperationRepository;
            _mapper = mapper;
            _sender = sender;
        }

        public async Task<int> Handle(CreateModelCommand request, CancellationToken cancellationToken)
        {
            var modelExists = await _modelRepository.ChcekIfModelExists(request.ModelDto.Name, request.ModelDto.SerialNumber);
            if (modelExists)
            {
                return _modelRepository.GetModelId(request.ModelDto.Name, request.ModelDto.SerialNumber);
            }

            var model = _mapper.Map<Model>(request.ModelDto);

            if (request.ModelDto.CompanyName != null)
            {
                model = await PrepareModelsCompany(model, request.ModelDto.CompanyName, cancellationToken);
            }

            if (request.ModelDto.MeasuredValues != null)
            {
                var measuredValues = GetMeasuredValuesForModel(request.ModelDto.MeasuredValues);
                model.MeasuredValues = measuredValues;
            }

            await _modelRepository.AddModel(model);

            var cooperatedModelsIds = request.ModelDto.CooperatedModelsIds;
            if (cooperatedModelsIds != null)
            {
                await _modelCooperationRepository.AddModelCooperation(model.Id, cooperatedModelsIds);
            }

            return model.Id;
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

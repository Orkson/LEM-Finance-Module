using Application.Models;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappers
{
    public class CalibrationCostProfile : Profile
    {
        public CalibrationCostProfile()
        {
            CreateMap<CalibrationCost, CalibrationCostDto>();
        }
    }
}

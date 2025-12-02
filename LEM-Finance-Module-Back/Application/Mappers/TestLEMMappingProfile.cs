using Application.Models;
using AutoMapper;
using Domain.Entities;

namespace TestLEM.Mappers
{
    public class TestLEMMappingProfile : Profile
    {
        public TestLEMMappingProfile()
        {
            CreateMap<AddDeviceDto, Device>()
                .ForMember(dest => dest.MeasuredValues, opt => opt.MapFrom(src => src.MeasuredValues));

            CreateMap<MeasuredValueDto, MeasuredValue>()
                .ForMember(dest => dest.MeasuredRanges, opt => opt.MapFrom(src => src.MeasuredRanges));

            CreateMap<MeasuredRangesDto, MeasuredRange>();
        }
    }
}

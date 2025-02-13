using Application.Models;
using AutoMapper;
using Domain.Entities;

namespace TestLEM.Mappers
{
    public class TestLEMMappingProfile : Profile
    {
        public TestLEMMappingProfile()
        {
            CreateMap<ModelDto, Model>()
                .ForMember(x => x.Company, y => y.Ignore())
                .ForMember(x => x.MeasuredValues, y => y.Ignore());

            CreateMap<AddDeviceDto, Device>()
                .ForMember(x => x.Model, y => y.Ignore());
        }
    }
}

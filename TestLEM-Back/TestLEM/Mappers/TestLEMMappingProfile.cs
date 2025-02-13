using AutoMapper;
using TestLEM.Entities;
using TestLEM.Models;

namespace TestLEM.Mappers
{
    public class TestLEMMappingProfile : Profile
    {
        public TestLEMMappingProfile()
        {
            CreateMap<ModelDto, Model>()
                .ForMember(x => x.Company, y => y.MapFrom(x => new Company
                {
                    Name = x.CompanyName
                }))
                .ForMember(x => x.MeasuredValues, y => y.Ignore());

            CreateMap<AddDeviceDto, Device>()
                .ForMember(x => x.Model, y => y.Ignore());
        }
    }
}

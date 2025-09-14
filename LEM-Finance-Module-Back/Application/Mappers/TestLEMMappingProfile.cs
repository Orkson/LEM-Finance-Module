using Application.Models;
using AutoMapper;
using Domain.Entities;

namespace TestLEM.Mappers
{
    public class TestLEMMappingProfile : Profile
    {
        public TestLEMMappingProfile()
        {
            CreateMap<AddDeviceDto, Device>();
        }
    }
}

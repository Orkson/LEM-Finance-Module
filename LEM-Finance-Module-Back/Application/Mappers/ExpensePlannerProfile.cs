using Application.Models;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappers
{
    public class ExpensePlannerProfile : Profile
    {
        public ExpensePlannerProfile()
        {
            CreateMap<ExpensePlanner, ExpensePlannerDto>();
        }
    }
}

using Application.Models;
using AutoMapper;
using Domain.Abstraction;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ExpensesPlanner.Commands
{
    public record EditExpensePlannerHandler : IRequestHandler<EditExpensePlannerCommand>
    {
        private readonly IExpensePlannerRepository _repository;
        private readonly IMapper _mapper;

        public EditExpensePlannerHandler(IExpensePlannerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task Handle(EditExpensePlannerCommand request, CancellationToken cancellationToken)
        {
            var expensePlanner = await _repository.GetPlannerByIdAsync(request.Id);

            if (expensePlanner == null)
                throw new Exception($"Expense Planner with ID {request.Id} not found.");

            expensePlanner.PlannedDate = request.PlannedDate;
            expensePlanner.StorageLocationName = request.StorageLocationName;
            expensePlanner.NetPrice = request.NetPrice;
            expensePlanner.GrossPrice = request.GrossPrice;
            expensePlanner.Tax = request.Tax;
            expensePlanner.Currency = request.Currency;
            expensePlanner.Description = request.Description;
            expensePlanner.Status = request.Status;
            expensePlanner.Service = await _repository.GetServiceByIdAsync(request.ServiceId);
            expensePlanner.Device = await _repository.GetDeviceByIdAsync(request.DeviceId);

            await _repository.UpdatePlannerAsync(expensePlanner);
        }
    }
}

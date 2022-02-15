using AutoMapper;
using Newtonsoft.Json;
using ReportMachine.BusinessLogic.DTOs;
using ReportMachine.BusinessLogic.Interfaces;
using ReportMachine.Domain.Entities;
using ReportMachine.Domain.Enums;
using ReportMachine.Repository.Interfaces;

namespace ReportMachine.BusinessLogic.Services
{
    public class MessageHandlingService : IMessageHandlingService
    {
        private readonly IMapper mapper;
        private readonly IReportRepository reportRepository;

        public MessageHandlingService(IMapper mapper, IReportRepository reportRepository)
        {
            this.mapper = mapper;
            this.reportRepository = reportRepository;
        }

        public async Task HandleApprovedBooking(string message)
        {
            var approvedReportDto = JsonConvert.DeserializeObject<ReportApprovedDto>(message);
            var report = await reportRepository.GetReport(approvedReportDto.Id);

            if (report == null)
                return;

            report.Status = BookingStatus.Approved;
            report.ManagedDate = approvedReportDto.ManagedDate;

            await reportRepository.UpdateReport(report);
        }

        public async Task HandleDeclinedBooking(string message)
        {
            var declinedReportDto = JsonConvert.DeserializeObject<ReportDeclinedDto>(message);
            var report = await reportRepository.GetReport(declinedReportDto.Id);

            if (report == null)
                return;

            report.Status = BookingStatus.Declined;
            report.ManagedDate = declinedReportDto.ManagedDate;
            report.Reason = declinedReportDto.Reason;

            await reportRepository.UpdateReport(report);
        }

        public async Task HandleNewBooking(string message)
        {
            var reportDto = JsonConvert.DeserializeObject<ReportDto>(message);
            var report = mapper.Map<Report>(reportDto);
            var employee = new Worker { FirstName = reportDto.EmployeeFirstName, LastName = reportDto.EmployeeLastName };
            var manager = new Worker { FirstName = reportDto.ManagerFirstName, LastName = reportDto.ManagerLastName };
            report.Employee = employee;
            report.Manager = manager;
            await reportRepository.CreateReport(report);

        }
    }
}

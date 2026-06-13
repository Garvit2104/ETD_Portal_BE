using AutoMapper;
using ETD_Portal.Models;
using ETD_Portal.Reimbursement_Mgmt.DTOs.ResponseDTO;
using ETD_Portal.Reimbursement_Mgmt.DTOs.ResquestDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reimbursement_Mgmt.Mappings
{
    public class ReimbursementMappingProfile : Profile
    {
        public ReimbursementMappingProfile()
        {
            CreateMap<ReimbursementType, ReimbursementTypeResponseDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.type, opt => opt.MapFrom(src => src.Type));


            CreateMap<ReimbursementRequestDTO, ReimbursementRequest>()
                .ForMember(dest => dest.TravelRequestId, opt => opt.MapFrom(src => src.travel_request_id))
                .ForMember(dest => dest.RequestRaisedByEmployeeId, opt => opt.MapFrom(src => src.request_raised_by_employee_id))
                .ForMember(dest => dest.ReimbursementTypeId, opt => opt.MapFrom(src => src.reimbursement_type_id))
                .ForMember(dest => dest.InvoiceDate, opt => opt.MapFrom(src => src.invoice_date))
                .ForMember(dest => dest.InvoiceAmount, opt => opt.MapFrom(src => src.invoice_amount))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceNo, opt => opt.Ignore())
                .ForMember(dest => dest.DocumentUrl, opt => opt.Ignore())
                .ForMember(dest => dest.RequestDate, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.RequestProcessedOn, opt => opt.Ignore())
                .ForMember(dest => dest.RequestProcessedByEmployeeId, opt => opt.Ignore())
                .ForMember(dest => dest.Remarks, opt => opt.Ignore())
                .ForMember(dest => dest.ReimbursementType, opt => opt.Ignore());

            // Add Reimbursement: ReimbursementRequest -> ReimbursementResponseDTO
            CreateMap<ReimbursementRequest, ReimbursementResponseDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.travel_request_id, opt => opt.MapFrom(src => src.TravelRequestId))
                .ForMember(dest => dest.request_raised_by_employee_id, opt => opt.MapFrom(src => src.RequestRaisedByEmployeeId))
                .ForMember(dest => dest.request_date, opt => opt.MapFrom(src => src.RequestDate))
                .ForMember(dest => dest.reimbursement_type_id, opt => opt.MapFrom(src => src.ReimbursementTypeId))
                .ForMember(dest => dest.reimbursement_type_name, opt => opt.MapFrom(src => src.ReimbursementType.Type))
                .ForMember(dest => dest.invoice_no, opt => opt.MapFrom(src => src.InvoiceNo))
                .ForMember(dest => dest.invoice_date, opt => opt.MapFrom(src => src.InvoiceDate))
                .ForMember(dest => dest.invoice_amount, opt => opt.MapFrom(src => src.InvoiceAmount))
                .ForMember(dest => dest.status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.remarks, opt => opt.MapFrom(src => src.Remarks));
        }
    }
}

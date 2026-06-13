using AutoMapper;
using ETD_Portal.Models;
using ETD_Portal.Reservation_Mgmt.DTOs.RequestDTO;
using ETD_Portal.Reservation_Mgmt.DTOs.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reservation_Mgmt.Mappings
{
    public class ReservationMappingProfile : Profile
    {
        public ReservationMappingProfile()
        {
            // Mapping 1: ReservationType ↔ DTOs
            CreateMap<ReservationType, ReservationTypeResponseDTO>()
                .ForMember(dest => dest.type_id, opt => opt.MapFrom(src => src.TypeId))
                .ForMember(dest => dest.type_name, opt => opt.MapFrom(src => src.TypeName));

            CreateMap<ReservationTypeRequestDTO, ReservationType>();

            // Mapping 2: Reservation ↔ DTOs
            CreateMap<Reservation, ReservationResponseDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.reservation_done_by_employee_id, opt => opt.MapFrom(src => src.ReservationDoneByEmployeeId))
                .ForMember(dest => dest.travel_request_id, opt => opt.MapFrom(src => src.TravelRequestId))
                .ForMember(dest => dest.reservation_type_id, opt => opt.MapFrom(src => src.ReservationTypeId))
                .ForMember(dest => dest.created_on, opt => opt.MapFrom(src => src.CreatedOn))
                .ForMember(dest => dest.reservation_done_with_entity, opt => opt.MapFrom(src => src.ReservationDoneWithEntity))
                .ForMember(dest => dest.reservation_date, opt => opt.MapFrom(src => src.ReservationDate))
                .ForMember(dest => dest.amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.remarks, opt => opt.MapFrom(src => src.Remarks));

            CreateMap<ReservationRequestDTO, Reservation>()
                .ForMember(dest => dest.ReservationDoneByEmployeeId, opt => opt.MapFrom(src => src.reservation_done_by_employee_id))
                .ForMember(dest => dest.TravelRequestId, opt => opt.MapFrom(src => src.travel_request_id))
                .ForMember(dest => dest.ReservationTypeId, opt => opt.MapFrom(src => src.reservation_type_id))
                .ForMember(dest => dest.ReservationDoneWithEntity, opt => opt.MapFrom(src => src.reservation_done_with_entity))
                .ForMember(dest => dest.ReservationDate, opt => opt.MapFrom(src => src.reservation_date))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.amount))
                .ForMember(dest => dest.Remarks, opt => opt.MapFrom(src => src.remarks))
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ConfirmationId, opt => opt.Ignore());

            // Mapping 3: ReservationDoc ↔ DTOs
            CreateMap<ReservationDoc, ReservationDocResponseDTO>();
            CreateMap<ReservationDocRequestDTO, ReservationDoc>();
        }
    }
}

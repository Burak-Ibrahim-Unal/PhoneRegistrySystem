using AutoMapper;
using PhoneRegistry.Application.Common.DTOs;
using PhoneRegistry.Domain.Entities;

namespace PhoneRegistry.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Person, PersonDto>()
            .ForMember(dest => dest.ContactInfos, opt => opt.MapFrom(src => src.ContactInfos));

        CreateMap<Person, PersonSummaryDto>()
            .ForMember(dest => dest.ContactInfoCount, opt => opt.MapFrom(src => src.ContactInfos.Count));

        CreateMap<ContactInfo, ContactInfoDto>();

        CreateMap<Report, ReportDto>()
            .ForMember(dest => dest.LocationStatistics, opt => opt.MapFrom(src => src.LocationStatistics));

        CreateMap<LocationStatistic, LocationStatisticDto>();
    }
}

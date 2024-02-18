using AutoMapper;
using cbs.common.dtos.DTOs.Messages;
using cbs.queue.monitoring.model.Messages;

namespace cbs.queue.monitoring.api.Configurations.AutoMappingProfiles.Messages;

internal class MessageToMessageUiAutoMapperProfile : Profile
{
    public MessageToMessageUiAutoMapperProfile()
    {
        ConfigureMapping();
    }

    public void ConfigureMapping()
    {
        CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.Firstname))
            .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.Lastname))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
            .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.Dob))
            .ForMember(dest => dest.Nationality, opt => opt.MapFrom(src => src.Nationality))
            .ForMember(dest => dest.Calls, opt => opt.MapFrom(src => src.Calls))
            .ReverseMap()
            .MaxDepth(1);
    }
}
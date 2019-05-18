using AutoMapper;
using NotesDemo.Entities;
using NotesDemo.Models;

namespace NotesDemo.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Note, NoteModel>()
                    .ReverseMap()
                        .ForMember(dest => dest.User, opt => opt.Ignore())
                        .ForMember(dest => dest.UserId, opt => opt.Ignore());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TBTT_Data.Entities;
using TBTT_Board.Models;

namespace TBTT_Board.Infrastructure
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Available, AvailableViewModel>().ReverseMap();
            CreateMap<Waiting, WaitingViewModel>().ReverseMap();
            CreateMap<Game, GameViewModel>().ReverseMap();
            CreateMap<Court, CourtViewModel>().ReverseMap();
            CreateMap<Member, MemberViewModel>().ReverseMap();
        }         

    }
}

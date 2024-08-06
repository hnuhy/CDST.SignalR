using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDST.SignalR.Jobs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateUpdateJobTaskItemDto, JobTaskItemDto>().ReverseMap();
            CreateMap<CreateUpdateJobTaskItemDetailDto, JobTaskItemDetailDto>().ReverseMap();
        }
    }
}

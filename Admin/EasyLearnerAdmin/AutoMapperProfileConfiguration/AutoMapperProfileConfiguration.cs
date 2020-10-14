using AutoMapper;
using EasyLearner.Service.Dto;
using EasyLearnerAdmin.Data.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EasyLearnerAdmin.AutoMapperProfileConfiguration
{
    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration()
        {
            CreateMap<GradeDto, Grades>().ReverseMap();
            CreateMap<LessonDto, Lessons>().ReverseMap();
            CreateMap<StudentDto, Students>().ReverseMap();
            CreateMap<SettingDto, Settings>().ReverseMap();
            CreateMap<TutorDto, Tutors>().ReverseMap();
            CreateMap<RelevantLessonDto, TutorRelevantLesson>().ReverseMap();
            CreateMap<SubscriptionDto, Subscription>().ReverseMap();
            CreateMap<SubscriptionTypeDto, SubscriptionType>().ReverseMap();
            CreateMap<FriendsDto,Friends>().ReverseMap();
            CreateMap<MessageDto,Message>().ReverseMap();
            CreateMap<PaymentDto, Payments>().ReverseMap();
            CreateMap<ExamDto, Exams>().ReverseMap();
            CreateMap<ExamDto, ExamsQuestions>().ReverseMap();
            CreateMap<InvitationCodeDto, InvitationCode>().ReverseMap();

        }
    }

    public static class MappingExpression
    {
        public static IMappingExpression<TSource, TDestination> Ignore<TSource, TDestination>(this IMappingExpression<TSource, TDestination> map, Expression<Func<TDestination, object>> selector)
        {
            map.ForMember(selector, config => config.Ignore());
            return map;
        }
    }

}

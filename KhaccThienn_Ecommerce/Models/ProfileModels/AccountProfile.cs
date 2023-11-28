using AutoMapper;
using KhaccThienn_Ecommerce.Models.DataModels;
using KhaccThienn_Ecommerce.Models.ViewModels;

namespace KhaccThienn_Ecommerce.Models.ProfileModels
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<CreateAccountViewModels, User>()
                .ReverseMap();
            CreateMap<UpdateAccountViewModels, User>()
                .ReverseMap();
            CreateMap<RegisterViewModel, User>()
                .ReverseMap();

            //CreateMap<Account, CreateAccountViewModels>();
        }
    }
}

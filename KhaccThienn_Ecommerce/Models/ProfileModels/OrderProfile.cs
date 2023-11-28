using AutoMapper;
using KhaccThienn_Ecommerce.Models.DataModels;
using KhaccThienn_Ecommerce.Models.ViewModels;

namespace KhaccThienn_Ecommerce.Models.ProfileModels
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<CheckoutViewModel, Order>()
                .ReverseMap();
        }
    }
}

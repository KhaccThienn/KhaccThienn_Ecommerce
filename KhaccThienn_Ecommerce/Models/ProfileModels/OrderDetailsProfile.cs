using AutoMapper;
using KhaccThienn_Ecommerce.Models.DataModels;
using KhaccThienn_Ecommerce.Models.ViewModels;

namespace KhaccThienn_Ecommerce.Models.ProfileModels
{
    public class OrderDetailsProfile : Profile
    {
        public OrderDetailsProfile()
        {
            CreateMap<OrderDetailsViewModel, OrderDetail>()
                .ReverseMap();
        }
    }
}

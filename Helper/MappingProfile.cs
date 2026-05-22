

using AutoMapper;
namespace backend_netcore_dotnet06.Helper;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ProductInsertDTO, Product>()
            .ForMember(dest => dest.Alias, opt => opt.MapFrom(src => HelperFunction.StringToSlug(src.Name)))
            .ForMember(dest => dest.Deleted, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.Now));
        // CreateMap<Product, ProductDTO>();

        CreateMap<ProductUpdateDTO, Product>();
    }
}
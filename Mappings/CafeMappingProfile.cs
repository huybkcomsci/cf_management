using AutoMapper;
using CafeManagement.DTOs;
using CafeManagement.Models.Entities;

namespace CafeManagement.Mappings;

public class CafeMappingProfile : Profile
{
    public CafeMappingProfile()
    {
        CreateMap<Sanpham, SanphamListItemDto>()
            .ForMember(dest => dest.TenNhom, opt => opt.MapFrom(src => src.NhomSP.TenNhom));

        CreateMap<Sanpham, SanphamDetailsDto>()
            .ForMember(dest => dest.TenNhom, opt => opt.MapFrom(src => src.NhomSP.TenNhom));

        CreateMap<SanphamUpsertDto, Sanpham>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.NgayTao, opt => opt.Ignore())
            .ForMember(dest => dest.NgayCapNhat, opt => opt.Ignore())
            .ForMember(dest => dest.NhomSP, opt => opt.Ignore())
            .ForMember(dest => dest.HoadonCTs, opt => opt.Ignore())
            .ForMember(dest => dest.Dongnhaps, opt => opt.Ignore())
            .ForMember(dest => dest.Tieuhaos, opt => opt.Ignore())
            .ForMember(dest => dest.Dinhluongs, opt => opt.Ignore());
    }
}
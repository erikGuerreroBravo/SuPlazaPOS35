using DsiCodeTech.Repository.PosCaja;

namespace SuPlazaPOS35.Infraestructure
{
    public class AutoMapperProfiles :AutoMapper.Profile
    {
        public AutoMapperProfiles() {
            CreateMap <SuPlazaPOS35.domain.articulo,articulo>()
                .ForMember(x=> x.impuestos, i=>i.MapFrom(c=>c.impuestos)).ReverseMap();
        }
        public static void Run()
        {

            AutoMapper.Mapper.Initialize(a =>
            {
                a.AddProfile<AutoMapperProfiles>();
            });
        }
    }
}

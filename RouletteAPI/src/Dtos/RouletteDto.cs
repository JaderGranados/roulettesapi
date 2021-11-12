using RoulettesAPI.Types;

namespace RoulettesAPI.Dtos
{
    public class RouletteDto
    {
        public string Id { get; set; }
        
        public string Name { get; set; }

        public RouletteStatus Status { get; set; }

        
    }
}
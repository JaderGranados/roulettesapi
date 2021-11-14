using System;
using RoulettesAPI.Types;

namespace RoulettesAPI.Dtos
{
    public class ReadRouletteDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public RouletteStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
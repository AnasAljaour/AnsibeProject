﻿namespace AnsibeProject.Models
{
    public class SectionsType
    {
        public List<Section>? Sections { get; set; } 
        public List<Section>? TempSections { get; set; } 
        public string Type {  get; set; } = string.Empty;
    }
}

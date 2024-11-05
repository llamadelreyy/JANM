namespace PBTPro.Shared.Models.RequestPayLoad
{
    public class ParamFormFieldInputModel
    {
        public string FormType { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Label { get; set; } = null!;

        public string Type { get; set; } = null!;

        public string? Option { get; set; }

        public string? SourceUrl { get; set; }

        public bool Required { get; set; }

        public bool ApiSeeded { get; set; }

        public int Orders { get; set; }
    }
}

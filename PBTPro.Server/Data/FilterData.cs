namespace PBT.Data
{
    public class FilterData
    {
        public int TypeId
        { get; set; }
        public string Description
        { get; set; }

        public string Color
        { get; set; }

        public string ColorDesc
        {
            get
            {
                return Description + ":" + Color;
            }
        }
    }
}

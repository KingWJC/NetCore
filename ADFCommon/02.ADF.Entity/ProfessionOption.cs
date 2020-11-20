namespace ADF.Entity
{
    public class ProfessionOption
    {
        public int PROFESSION_ID { get; set; }
        public string PROFESSION_NAME { get; set; }
        public string OPTION_NAME { get; set; }
        public string OPTION_TYPE { get; set; }
        public string OPTION_VALUE { get; set; }
    }

    
    public class ProfessionContext
    {
        public string PROFESSION_NAME { get; set; }
        public string PROFESSION_CONTEXT { get; set; }
    }
}
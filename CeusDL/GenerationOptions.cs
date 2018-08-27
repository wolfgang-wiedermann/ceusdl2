public class GenerationOptions {
    public GenerationOptions() {
        GenerateStar = false;
        GenerateSnowflake = false;
        DbConnectionString = null;
    }
    public bool GenerateStar { get; set; }
    public bool GenerateSnowflake { get; set; }
    public string DbConnectionString { get; set; }
}
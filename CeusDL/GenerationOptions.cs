public class GenerationOptions {
    public GenerationOptions() {
        GenerateStar = false;
        GenerateSnowflake = false;
        DbConnectionString = null;
        ExecuteUpdate = false;
        ExecuteReplace = false;
    }
    public bool GenerateStar { get; set; }
    public bool GenerateSnowflake { get; set; }
    public bool ExecuteUpdate { get; set; }
    public bool ExecuteReplace { get; set; }
    public string DbConnectionString { get; set; }
}
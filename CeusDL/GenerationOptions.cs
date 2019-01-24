public class GenerationOptions {

    public GenerationOptions() {
        GenerateStar = false;
        GenerateSnowflake = false;
        DbConnectionString = null;
        ExecuteUpdate = false;
        ExecuteUpdateWithReload = false;
        ExecuteReplace = false;
        GenerateMsSql = true;
        GenerateMySql = false;
        GenerateConstraints = false;
    }
    public bool GenerateStar { get; set; }
    public bool GenerateSnowflake { get; set; }
    public bool GenerateMySql { get; set; }
    public bool GenerateConstraints { get; set; }
    public bool GenerateMsSql { get; set; }
    public bool ExecuteUpdate { get; set; }
    public bool ExecuteUpdateWithReload { get; set; }
    public bool ExecuteReplace { get; set; }
    public string DbConnectionString { get; set; }
}
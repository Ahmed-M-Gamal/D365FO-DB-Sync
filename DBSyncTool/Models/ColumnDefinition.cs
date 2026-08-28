namespace DBSyncTool.Models
{
    /// <summary>
    /// Physical SQL Server column definition, read from INFORMATION_SCHEMA.COLUMNS.
    /// Used by the Sync UAT Schema feature to replicate a Tier2 column onto AxDB.
    /// </summary>
    public class ColumnDefinition
    {
        public string ColumnName { get; set; } = "";
        public string DataType { get; set; } = "";
        public int? MaxLength { get; set; }
        public int? Precision { get; set; }
        public int? Scale { get; set; }
        public int? DateTimePrecision { get; set; }
        public string? Collation { get; set; }

        private static readonly HashSet<string> LengthTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "char", "varchar", "nchar", "nvarchar", "binary", "varbinary"
        };

        private static readonly HashSet<string> PrecisionScaleTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "decimal", "numeric"
        };

        private static readonly HashSet<string> DateTimePrecisionTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "datetime2", "datetimeoffset", "time"
        };

        private static readonly HashSet<string> CharacterTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "char", "varchar", "nchar", "nvarchar", "text", "ntext"
        };

        /// <summary>
        /// Builds the SQL type fragment for use in an ALTER TABLE ADD statement,
        /// e.g. "NVARCHAR(60)", "DECIMAL(28,8)", "DATETIME2(7)", "INT".
        /// </summary>
        public string SqlTypeString
        {
            get
            {
                string type;
                if (LengthTypes.Contains(DataType))
                {
                    string length = MaxLength.HasValue && MaxLength.Value == -1 ? "MAX" : (MaxLength ?? 0).ToString();
                    type = $"{DataType.ToUpperInvariant()}({length})";
                }
                else if (PrecisionScaleTypes.Contains(DataType))
                {
                    type = $"{DataType.ToUpperInvariant()}({Precision ?? 18},{Scale ?? 0})";
                }
                else if (DateTimePrecisionTypes.Contains(DataType))
                {
                    type = $"{DataType.ToUpperInvariant()}({DateTimePrecision ?? 7})";
                }
                else
                {
                    type = DataType.ToUpperInvariant();
                }

                if (CharacterTypes.Contains(DataType) && !string.IsNullOrWhiteSpace(Collation))
                    type += $" COLLATE {Collation}";

                return type;
            }
        }
    }
}

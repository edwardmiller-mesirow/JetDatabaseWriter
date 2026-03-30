namespace JetDatabaseReader
{
    /// <summary>
    /// Result returned by <see cref="IAccessReader.ReadFirstTable"/>.
    /// Extends <see cref="TableResult"/> with the total number of user tables
    /// found in the database.
    /// </summary>
    public sealed class FirstTableResult : TableResult
    {
        /// <summary>Total number of user tables found in the database.</summary>
        public int TableCount { get; set; }
    }
}

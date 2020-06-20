namespace Orleans.Storage
{
    internal class MorsteadGrainStorageModel
    {
        public byte[] Contents { get; set; }
        public string ETag { get; set; }
    }
}
namespace Orleans.Persistence.Morstead.Provider
{
    internal enum MorsteadProviderErrorCode
    {
        ProvidersBase = 200000,

        // Morstead storage provider related
        MorsteadProviderBase = ProvidersBase + 2000,
        MorsteadProvider_DataNotFound = MorsteadProviderBase + 1,
        MorsteadProvider_ReadingData = MorsteadProviderBase + 2,
        MorsteadProvider_WritingData = MorsteadProviderBase + 3,
        MorsteadProvider_Storage_Reading = MorsteadProviderBase + 4,
        MorsteadProvider_Storage_Writing = MorsteadProviderBase + 5,
        MorsteadProvider_Storage_DataRead = MorsteadProviderBase + 6,
        MorsteadProvider_WriteError = MorsteadProviderBase + 7,
        MorsteadProvider_DeleteError = MorsteadProviderBase + 8,
        MorsteadProvider_InitProvider = MorsteadProviderBase + 9,
        MorsteadProvider_ParamConnectionString = MorsteadProviderBase + 10,
        MorsteadProvider_DatabaseNotFound = MorsteadProviderBase +11
    }
}

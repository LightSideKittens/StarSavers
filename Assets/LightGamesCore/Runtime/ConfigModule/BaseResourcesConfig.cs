namespace LGCore.ConfigModule
{
    public class BaseResourcesConfig<T> : BaseConfig<T> where T : BaseResourcesConfig<T>, new()
    {
        protected override string GeneralFolderName => FolderNames.DefaultSaveData;
    }
}
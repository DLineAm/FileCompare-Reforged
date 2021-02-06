namespace FileCompare_Reforged
{
    public class FileResult
    {
        public enum FileHandlerResult
        {
            NoChanges, WithChanges, NoFiles, WithFindWord
        }

        private  FileHandlerResult _fileHandlerResult { get; set; }
        public  FileHandlerResult Result
        {
            get => _fileHandlerResult;
            set
            {
                _fileHandlerResult = value;
                //ResultHandler();
            }
        }
    }
}
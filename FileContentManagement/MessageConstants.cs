namespace FileContentManagement
{
    public static class MessageConstants
    {
        public const string StoringSuccess = "The file with id {0} has been successfully stored.";

        public const string StoringFailed = "The file with id {0} was not stored. Reason: {1}";

        public const string ExistsFailed = "A file with id {0} does not exist. Exception: {1}";

        public const string GetFailed = "A file with id {0} could not be downloaded. Exception: {1}";
    } 
}

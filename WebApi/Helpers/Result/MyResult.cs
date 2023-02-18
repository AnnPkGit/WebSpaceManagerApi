namespace WebSpaceManager.Helpers.Result
{
    public class MyResult<T>
    {
        public bool IsSuccessful { get; private set; }

        public string? ErrorMessage { get; private set; }

        public T? Value { get; private set; }

        public static MyResult<T> Successful(T value)
        {
            return new MyResult<T>(true, value);
        }

        public static MyResult<T> Failed(string errorMessage)
        {
            return new MyResult<T>(false, errorMessage);
        }

        private MyResult(bool isSuccessful, string? errorMessage = null)
        {
            IsSuccessful = isSuccessful;
            ErrorMessage = errorMessage;
        }

        private MyResult(bool isSuccessful, T value, string? errorMessage = null)
        {
            IsSuccessful = isSuccessful;
            ErrorMessage = errorMessage;
            Value = value;
        }
    }
}

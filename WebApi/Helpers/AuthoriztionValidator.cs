namespace WebSpaceManager.Helpers
{
    public class AuthoriztionValidator : IAuthoriztionValidator
    {
        private readonly string _key;
        private readonly string _devKey;

        public AuthoriztionValidator(IConfiguration config)
        {
            _key = config.GetValue(typeof(string), "ApiSecret").ToString();
            _devKey = config.GetValue(typeof(string), "DevApiSecret").ToString();
        }

        public bool IsValidKey(string key)
        {
            return key.Equals(_key) ? true : false;
        }

        public bool IsValidDevKey(string key)
        {
            return key.Equals(_devKey) ? true : false;
        }
    }
}

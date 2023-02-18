namespace WebSpaceManager.Helpers
{
    public interface IAuthoriztionValidator
    {
        bool IsValidKey(string key);

        bool IsValidDevKey(string key);
    }
}

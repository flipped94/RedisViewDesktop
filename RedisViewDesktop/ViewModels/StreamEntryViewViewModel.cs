
namespace RedisViewDesktop.ViewModels
{
    public class StreamEntryViewViewModel(string entryId, string value) : ViewModelBase
    {
        public string EntryID { get; } = entryId;

        public string Value { get; } = value;
    }
}
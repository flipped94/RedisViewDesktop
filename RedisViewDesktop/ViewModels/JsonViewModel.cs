using ReactiveUI;
using RedisViewDesktop.Helpers;
using System.Collections.Generic;
using System.Windows.Input;

namespace RedisViewDesktop.ViewModels
{
    public class JsonViewModel : KeyViewModelBase
    {
        public string? Text { get; set; }

        public JsonViewModel()
        {
        }

        public async void Ready(AvaloniaEdit.TextEditor editor)
        {
            Dictionary<string, object> info = await RedisHelper.JsonDetail(Key);
            MemoryUsage = (string)info.GetValueOrDefault("memory", "");
            Length = (string)info.GetValueOrDefault("len", "");
            TTL = (string)info.GetValueOrDefault("ttl", "");
            string str = (string)info.GetValueOrDefault("content", "");
            Text = JsonHelper.Format(str);
            editor.Text = Text;
        }
    }
}
using AvaloniaEdit.Utils;
using ReactiveUI;
using RedisViewDesktop.Helpers;
using System.Collections.ObjectModel;

namespace RedisViewDesktop.Models
{
    public class KeyNode : ReactiveObject
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Type { get; set; }
        public string Color { get; set; }
        public string? ParentId { get; set; }

        public bool IsKey { get; set; }
        public int KeyCount { get => LoadKeyCount(); }

        private int LoadKeyCount()
        {
            if (IsKey)
            {
                return 0;
            }
            else
            {
                return KeyHelper.AllKeys().FindAll(s => s.StartsWith(Id[..^1])).Count;
            }
        }

        private bool isOpen = false;
        public bool IsOpen
        {
            get => isOpen;
            set
            {
                this.RaiseAndSetIfChanged(ref isOpen, value, nameof(IsOpen));
            }
        }       

        public ObservableCollection<KeyNode> Children
        {
            get => LoadChildren();
        }

        private ObservableCollection<KeyNode> LoadChildren()
        {
            var result = new ObservableCollection<KeyNode>();
            var children = KeyHelper.LoadChildren(Id);
            result.AddRange(children);
            return result;
        }        
    }

}

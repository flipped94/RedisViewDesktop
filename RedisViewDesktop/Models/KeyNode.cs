using ReactiveUI;
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
        public int KeyCount { get; set; }

        private bool isOpen = false;
        public bool IsOpen
        {
            get => isOpen;
            set
            {
                this.RaiseAndSetIfChanged(ref isOpen, value,nameof(IsOpen));
            }
        }

        public ObservableCollection<KeyNode>? Children { get; set; } = [];
    }

}

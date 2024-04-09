using ReactiveUI;
using RedisViewDesktop.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RedisViewDesktop.ViewModels
{
    public class StringViewModel : KeyViewModelBase
    {
        private bool isEditing = false;
        public bool IsEditing
        {
            get => isEditing;
            set
            {
                this.RaiseAndSetIfChanged(ref isEditing, value);
            }
        }

        private string? content;
        public string? Content
        {
            get => content;
            set
            {
                this.RaiseAndSetIfChanged(ref content, value);
            }
        }

        public ICommand EditCommand { get; }
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand DeleteCommand { get; }

        public StringViewModel()
        {
            EditCommand = ReactiveCommand.Create(() =>
            {
                IsEditing = true;
            });
            ConfirmCommand = ReactiveCommand.Create(async () =>
            {
                await RedisHelper.StringSet(Key, Content);
                IsEditing = false;
            });
            CancelCommand = ReactiveCommand.Create(() =>
            {
                IsEditing = false;
            });
            DeleteCommand = ReactiveCommand.Create(async () =>
            {
                await RedisHelper.DeleteKey(Key);
                KeysPage!.CurrentPage = new SelectAKeyViewModel();
            });
        }

        public async void Ready()
        {
            Dictionary<string, string> info = await RedisHelper.StringDetail(Key);
            MemoryUsage = info.GetValueOrDefault("memory", "");
            Length = info.GetValueOrDefault("len", "");
            TTL = info.GetValueOrDefault("ttl", "");
            Content = info.GetValueOrDefault("content", "");
        }

        public async Task<bool> Set()
        {
            if (Content != null)
            {
                return await RedisHelper.StringSet(Key, Content);
            }
            return true;
        }

        public async Task<bool> Delete()
        {
            return await RedisHelper.DeleteKey(Key);
        }
    }

}
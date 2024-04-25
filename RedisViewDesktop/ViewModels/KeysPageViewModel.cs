using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DynamicData;
using ReactiveUI;
using RedisViewDesktop.Enums;
using RedisViewDesktop.Helpers;
using RedisViewDesktop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RedisViewDesktop.ViewModels
{

    public class KeysPageViewModel : ViewModelBase
    {
        public bool isList = true;
        public bool IsList
        {
            get => isList;
            set
            {
                this.RaiseAndSetIfChanged(ref isList, value);
            }
        }

        public long dbSize = -1;
        public long DbSize
        {
            get => dbSize;
            set
            {
                this.RaiseAndSetIfChanged(ref dbSize, value);
            }
        }

        public Connection? Connection { get; set; }

        public string Alias => Connection!.Alias;

        public int db;
        public int Db
        {
            get => Connection!.Db;
            set
            {
                this.RaiseAndSetIfChanged(ref db, value);
                Connection!.Db = value;
                _ = Connection.UpdateDb(Connection.Id, db);
                RedisHelper.SwitchDb(value);
                KeysTree.Clear();
                ListNodes.Clear();
                ScannedKeys = 0;
                _ = PrepareData(true);
            }
        }

        public bool IsShowDb => !"Cluster".Equals(Connection.ServerType);

        public bool IsConnected => Connection != null;

        private KeyNode? selectedNode;
        public KeyNode? SelectedNode
        {
            get => selectedNode;
            set
            {
                if (value != null && value.IsKey)
                {
                    DoShowValue(value.Name, value.Type);
                }
                this.RaiseAndSetIfChanged(ref selectedNode, value);
            }
        }

        private KeyViewModelBase? currentPage = new SelectAKeyViewModel();
        public KeyViewModelBase? CurrentPage
        {
            get => currentPage;
            set
            {
                this.RaiseAndSetIfChanged(ref currentPage, value);
            }
        }

        private ViewModelBase? splitViewPane;
        public ViewModelBase? SplitViewPane
        {
            get => splitViewPane;
            set
            {
                this.RaiseAndSetIfChanged(ref splitViewPane, value);
            }
        }

        public ReactiveCommand<string, Unit> SwitchViewCommand { get; }

        private bool isPaneOpen = false;
        public bool IsPaneOpen
        {
            get => isPaneOpen;
            set
            {
                this.RaiseAndSetIfChanged(ref isPaneOpen, value);
            }
        }

        public bool isHasMore;
        public bool IsHasMore
        {
            get => isHasMore;
            set
            {
                this.RaiseAndSetIfChanged(ref isHasMore, value);
            }
        }

        public ReactiveCommand<string, Unit> OpenPaneCommand { get; }

        public ICommand LoadMoreCommand { get; }

        public ICommand LoadAllCommand { get; }

        public ICommand ResetCommand { get; }

        public string pattern;
        public string Pattern
        {
            get => pattern;
            set
            {
                this.RaiseAndSetIfChanged(ref pattern, value);
            }
        }
        public ICommand SearchCommand { get; }

        public ICommand NewKeyCommand { get; }
        public Interaction<NewKeyViewModel, AddKey?> AddNewKeyInteraction;

        public KeysPageViewModel()
        {
            RxApp.MainThreadScheduler.Schedule(async () =>
            {
                await Ready();
            });

            SwitchViewCommand = ReactiveCommand.Create<string>(view =>
            {
                IsList = "List".Equals(view);
                if (!IsList)
                {

                }
            });

            OpenPaneCommand = ReactiveCommand.Create<string>((pane) =>
            {
                Type? paneType = PaneHelper.GetPane(pane);
                if (paneType != null)
                {
                    ViewModelBase paneView = (ViewModelBase)Activator.CreateInstance(paneType);
                    if (paneView != null)
                    {
                        SplitViewPane = paneView;
                        if (!IsPaneOpen)
                        {
                            IsPaneOpen = !IsPaneOpen;
                        }
                    }
                }
            });

            LoadMoreCommand = ReactiveCommand.Create(async () =>
            {
                await PrepareData(false);
            });

            ResetCommand = ReactiveCommand.Create(async () =>
            {
                Reset();
                Request = new ScanKeyQequestBuilder().Build();
                await PrepareData(true);
            });

            SearchCommand = ReactiveCommand.Create(async () =>
            {
                Reset();
                Request = new ScanKeyQequestBuilder().SetPattern(Pattern).Build();
                await PrepareData(true);
            });

            Source.RowSelection.SingleSelect = true;
            Source.RowSelection.SelectionChanged += SelectionChanged;

            AddNewKeyInteraction = new Interaction<NewKeyViewModel, AddKey?>();
            NewKeyCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                NewKeyViewModel input = new NewKeyViewModel();
                var newKey = await AddNewKeyInteraction.Handle(input);
                await AddKey(newKey);
            });
        }

        private async Task AddKey(AddKey? newKey)
        {
            if (newKey is not null)
            {
                switch (newKey.NewKey.KeyType)
                {
                    case "HASH":
                        await RedisHelper.ExecuteAsync("HSET", [newKey.Key, "New Field", "New Value"]);
                        break;
                    case "LIST":
                        await RedisHelper.ExecuteAsync("LPUSH", [newKey.Key, "New Element"]);
                        break;
                    case "SET":
                        await RedisHelper.ExecuteAsync("SADD", [newKey.Key, "New Member"]);
                        break;
                    case "ZSET":
                        await RedisHelper.ExecuteAsync("ZADD", [newKey.Key, 0, "New Member"]);
                        break;
                    case "STRING":
                        await RedisHelper.ExecuteAsync("SET", [newKey.Key, ""]);
                        break;
                    case "JSON":
                        await RedisHelper.ExecuteAsync("JSON.SET", [newKey.Key, ".", "{\"New key\":\"New value\"}"]);
                        break;
                    case "STREAM":
                        await RedisHelper.ExecuteAsync("XADD", [newKey.Key, "*", "New key", "New value"]);
                        break;
                }
                BuildListView([newKey.Key]);
                Source.Items = BuildTreeView([newKey.Key]);
                ScannedKeys += 1;
                DbSize += 1;
            }
        }

        private void SelectionChanged(object? sender, TreeSelectionModelSelectionChangedEventArgs<KeyNode> e)
        {
            var selectionNode = Source.RowSelection.SelectedItem;
            if (selectionNode != null)
            {
                if (!selectionNode.IsKey)
                {
                    if (selectionNode.IsOpen)
                    {
                        Source.Collapse(Source.RowSelection.SelectedIndex);
                    }
                    else
                    {
                        Source.Expand(Source.RowSelection.SelectedIndex);
                    }
                    // NOTE
                    Source.RowSelection.Deselect(Source.RowSelection.SelectedIndex);
                }
                else
                {
                    SelectedNode = selectionNode;
                }
            }
        }

        public void DoShowValue(string key, string keyType)
        {
            Type? type = KeyViewModleHelper.GetViewType(keyType);
            if (type != null)
            {
                KeyViewModelBase keyViewModel = (KeyViewModelBase)Activator.CreateInstance(type)!;
                keyViewModel!.Key = key;
                keyViewModel!.Type = keyType.ToUpper();
                keyViewModel!.Color = KeyTypeHelper.GetColor(keyType);
                CurrentPage = keyViewModel;
                keyViewModel.KeysPage = this;
            }
        }

        public ScanKeyRequest Request = new ScanKeyQequestBuilder() { Count = 300 }.Build();

        public async Task Ready()
        {
            Connection = await Connection.GetLastConnectedConnectionAsync();
            await RedisHelper.ConnectAsync(Connection!);
            await PrepareData(true);
        }

        public long scannedKeys;
        public long ScannedKeys
        {
            get => scannedKeys;
            set
            {
                this.RaiseAndSetIfChanged(ref scannedKeys, value);
            }
        }
        public List<KeyNode> ScannedNodes { get; } = [];
        public ObservableCollection<KeyNode> ListNodes { get; } = [];
        public Dictionary<string, KeyNode> TreeNodeDict = [];
        public ObservableCollection<KeyNode> KeysTree { get; } = [];
        public HierarchicalTreeDataGridSource<KeyNode> Source { get; } = new HierarchicalTreeDataGridSource<KeyNode>([])
        {
            Columns =
            {
                 new HierarchicalExpanderColumn<KeyNode>(new TemplateColumn<KeyNode>
                     ("Name","KeyCell",null,new GridLength(1, GridUnitType.Star))
                     ,x=>x.Children,
                     x=>null!=x.Children&& x.Children.Count>0,
                     x=>x.IsOpen)
            }
        };
        private async Task PrepareData(bool isInit)
        {
            DbSize = await RedisHelper.DbSize();
            ScanKeyResponse response = await RedisHelper.Scan(Request, isInit);
            IsHasMore = response.ShowMore;
            Request.NodeCursor = response.NodeCursor;
            ScannedKeys += response.Keys.Count;
            BuildListView(response.Keys);
            List<KeyNode> tree = BuildTreeView(response.Keys);
            Source.Items = tree;
        }

        private void BuildListView(List<string> keys)
        {
            var listNodes = KeyHelper.BuildListNodes(keys);
            ListNodes.AddRange(listNodes);
        }

        private List<KeyNode> BuildTreeView(List<string> keys)
        {
            List<KeyNode> treeNodes = KeyHelper.BuildTreeNodes(keys, Connection.Delimiter);
            foreach (var treeNode in treeNodes)
            {
                TreeNodeDict.TryAdd(treeNode.Id, treeNode);
            }
            var nodes = new List<KeyNode>();
            foreach (var item in TreeNodeDict)
            {
                nodes.Add(item.Value);
            }
            var tree = KeyHelper.BuildTree(nodes);
            return tree;
        }

        public void Reset()
        {
            ScannedKeys = 0;
            KeysTree.Clear();
            TreeNodeDict.Clear();
            ListNodes.Clear();
            Request = new ScanKeyQequestBuilder().Build();
        }

        private static IconConverter? s_iconConverter;

        public static IMultiValueConverter FileIconConverter
        {
            get
            {
                if (s_iconConverter is null)
                {
                    using var folderStream = AssetLoader.Open(new Uri("avares://RedisViewDesktop/Assets/folder.png"));
                    using var folderOpenStream = AssetLoader.Open(new Uri("avares://RedisViewDesktop/Assets/folder-open.png"));
                    var folderIcon = new Bitmap(folderStream);
                    var folderOpenIcon = new Bitmap(folderOpenStream);

                    s_iconConverter = new IconConverter(folderOpenIcon, folderIcon);
                }

                return s_iconConverter;
            }
        }
    }

    public class IconConverter(Bitmap folderExpanded, Bitmap folderCollapsed) : IMultiValueConverter
    {
        private readonly Bitmap _folderExpanded = folderExpanded;
        private readonly Bitmap _folderCollapsed = folderCollapsed;

        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count == 2 &&
                values[0] is bool isKey &&
                values[1] is bool isOpen)
            {
                if (!isKey && isOpen)
                {
                    return _folderExpanded;
                }
                if (!isKey && !isOpen)
                {
                    return _folderCollapsed;
                }
            }

            return null;
        }
    }

}

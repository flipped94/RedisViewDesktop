using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaEdit.Utils;
using DynamicData;
using ReactiveUI;
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
using System.Timers;
using System.Windows.Input;

namespace RedisViewDesktop.ViewModels
{

    public class KeysPageViewModel : ViewModelBase
    {
        private bool isList = true;
        public bool IsList
        {
            get => isList;
            set
            {
                this.RaiseAndSetIfChanged(ref isList, value);
            }
        }

        private long dbSize = -1;
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

        private int db;
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

        private bool IsShowDb => !"Cluster".Equals(Connection.ServerType);

        public bool IsConnected => Connection != null;

        private bool isAutoRefresh = false;
        public bool IsAutoRefresh
        {
            get => isAutoRefresh;
            set
            {                
                this.RaiseAndSetIfChanged(ref isAutoRefresh, value);
            }
        }

        private int refreshRate = 60;
        public int RefreshRate
        {
            get => refreshRate;
            set
            {
                this.RaiseAndSetIfChanged(ref refreshRate, value);
            }
        }

        private bool isEditAutoRefresh = false;
        public bool IsEditAutoRefresh
        {
            get => isEditAutoRefresh;
            set
            {
                this.RaiseAndSetIfChanged(ref isEditAutoRefresh, value);
            }
        }

        private DateTime lastRefreshTime;
        private string lastRefreshStr;
        public string LastRefreshStr
        {
            get => lastRefreshStr;
            set
            {
                this.RaiseAndSetIfChanged(ref lastRefreshStr, value);
            }
        }

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

        private Timer timer;

        public KeysPageViewModel()
        {
            Source = new HierarchicalTreeDataGridSource<KeyNode>(KeysTree)
            {
                Columns =
                {
                   new HierarchicalExpanderColumn<KeyNode>(
                       new TemplateColumn<KeyNode>("Name",
                       "KeyCell",
                       null,
                       new GridLength(1, GridUnitType.Star)),
                       x=>x.Children,
                       x=>null!=x.Children&& x.Children.Count>0,
                       x=>x.IsOpen
                       )
                }
            };
            Source.RowSelection.SingleSelect = true;
            Source.RowSelection.SelectionChanged += SelectionChanged;
            Source.RowCollapsed += Source_RowCollapsed;
            Source.RowExpanded += Source_RowExpanded;

            RxApp.MainThreadScheduler.Schedule(async () =>
            {
                lastRefreshTime = DateTime.Now;
                LastRefreshStr = "last refresh: now";
                timer = new Timer(30*1000);
                timer.Elapsed += RefreshElapsed;
                timer.Start();
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
                lastRefreshTime = DateTime.Now;
                LastRefreshStr = "last refresh: now";
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

            AddNewKeyInteraction = new Interaction<NewKeyViewModel, AddKey?>();
            NewKeyCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                NewKeyViewModel input = new NewKeyViewModel();
                var newKey = await AddNewKeyInteraction.Handle(input);
                await AddKey(newKey);
            });
        }       

        private void RefreshElapsed(object? sender, ElapsedEventArgs e)
        {
            var passed = DateTime.Now - lastRefreshTime;
            LastRefreshStr = $"last refresh: {TimeSpanDescHelper.Dscription(passed)}";
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
                ScannedKeys += 1;
                DbSize += 1;
                Keys.Add(newKey.Key);
                BuildListView();
                BuildTreeView();
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
                        ExpandDict.Remove(selectionNode.Id);
                    }
                    else
                    {
                        Source.Expand(Source.RowSelection.SelectedIndex);
                        ExpandDict.TryAdd(selectionNode.Id, true);
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

        private void Source_RowCollapsed(object? sender, RowEventArgs<HierarchicalRow<KeyNode>> e)
        {
            ExpandDict.Remove(e.Row.Model.Id);
        }

        private void Source_RowExpanded(object? sender, RowEventArgs<HierarchicalRow<KeyNode>> e)
        {
            ExpandDict.TryAdd(e.Row.Model.Id, true);
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
        public List<string> Keys { get; } = [];
        public ObservableCollection<KeyNode> ListNodes { get; } = [];
        public ObservableCollection<KeyNode> KeysTree { get; } = [];
        public Dictionary<string, bool> ExpandDict { get; } = [];
        public HierarchicalTreeDataGridSource<KeyNode> Source { get; }
        private async Task PrepareData(bool isInit)
        {
            DbSize = await RedisHelper.DbSize();
            ScanKeyResponse response = await RedisHelper.Scan(Request, isInit);
            IsHasMore = response.ShowMore;
            Request.NodeCursor = response.NodeCursor;
            ScannedKeys += response.Keys.Count;
            Keys.AddRange(response.Keys);
            BuildListView();
            BuildTreeView();
        }

        private void BuildListView()
        {
            ListNodes.Clear();
            var listNodes = KeyHelper.BuildListNodes(Keys);
            ListNodes.AddRange(listNodes);
        }

        private void BuildTreeView()
        {
            KeysTree.Clear();
            List<KeyNode> treeNodes = KeyHelper.BuildTreeNodes(Keys, Connection.Delimiter);
            treeNodes.ForEach(x =>
            {
                if (!x.IsKey)
                {
                    if (ExpandDict.TryGetValue(x.Id, out var expand))
                    {
                        x.IsOpen = expand;
                    }
                }
            });
            KeysTree.AddRange(KeyHelper.BuildTree(treeNodes));
        }

        public void Reset()
        {
            ScannedKeys = 0;
            Keys.Clear();
            Request = new ScanKeyQequestBuilder().Build();
        }

        public async Task DeleteKey()
        {
            if (SelectedNode is not null)
            {
                var res = await RedisHelper.DeleteKey(SelectedNode.Name);
                if (res)
                {
                    ScannedKeys -= 1;
                    DbSize -= 1;
                    if (SelectedNode is not null && SelectedNode.IsKey)
                    {
                        Keys.Remove(SelectedNode.Name);
                        BuildListView();
                        BuildTreeView();
                        CurrentPage = new SelectAKeyViewModel();
                    }
                }
            }
        }

        public async Task RefreshInterval()
        {
            lastRefreshTime = DateTime.Now;
            LastRefreshStr = "last refresh: now";
            Reset();
            await PrepareData(true);
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

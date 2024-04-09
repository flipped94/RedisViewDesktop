using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
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

        public ReactiveCommand<string, Unit> SwitchViewCommand { get; }

        private bool isOpenCli = false;
        public bool IsOpenCli
        {
            get => isOpenCli;
            set
            {
                this.RaiseAndSetIfChanged(ref isOpenCli, value);
            }
        }

        private string commandAndArgs;
        public string CommandAndArgs
        {
            get => commandAndArgs; set
            {
                this.RaiseAndSetIfChanged(ref commandAndArgs, value);
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

        public ICommand OpenCliCommand { get; }

        public ICommand ExecuteCommand { get; }

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

            OpenCliCommand = ReactiveCommand.Create(() =>
            {
                IsOpenCli = !IsOpenCli;
            });

            ExecuteCommand = ReactiveCommand.Create(async () =>
            {
                await Exexute();
                CommandAndArgs = "";
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
            var listNodes = KeyHelper.BuildListNodes(response.Keys);
            ListNodes.AddRange(listNodes);


            List<KeyNode> treeNodes = KeyHelper.BuildTreeNodes(response.Keys, Connection.Delimiter);
            foreach (var treeNode in treeNodes)
            {
                TreeNodeDict.TryAdd(treeNode.Id, treeNode);
            }
            var nodes = new List<KeyNode>();
            foreach (var item in TreeNodeDict)
            {
                nodes.Add(item.Value);
            }
            Source.Items = KeyHelper.BuildTree(nodes);
        }

        public void Reset()
        {
            ScannedKeys=0;
            KeysTree.Clear();
            TreeNodeDict.Clear();
            ListNodes.Clear();
            Request = new ScanKeyQequestBuilder().Build();
        }

        public ObservableCollection<ExecuteResult> Results { get; } = [];

        public async Task Exexute()
        {
            if (string.IsNullOrEmpty(CommandAndArgs))
            {
                return;
            }
            var cmas = CommandAndArgs.Split(" ");
            ExecuteResult executeResult;
            if (cmas.Length == 1)
            {
                executeResult = await RedisHelper.ExecuteAsync(cmas[0], null);
            }
            else
            {
                object[] args = new object[cmas.Length - 1];
                for (int i = 1; i < cmas.Length; i++)
                {
                    args[i - 1] = cmas[i];
                }
                executeResult = await RedisHelper.ExecuteAsync(cmas[0], args);
            }
            if (null == executeResult)
            {
                return;
            }

            var c = string.Join("\t", cmas);
            executeResult.Message = $"Command:\t{c}\r\n" + executeResult.Message;
            Results.Add(executeResult);

        }

        public void ClearResults()
        {
            Results.Clear();
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

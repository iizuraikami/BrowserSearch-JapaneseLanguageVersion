using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.PowerToys.Settings.UI.Library;
using Wox.Plugin;
using Wox.Plugin.Logger;
using BrowserInfo = Wox.Plugin.Common.DefaultBrowserInfo;
using BrowserSearch.Browsers;

namespace Community.Powertoys.Run.Plugin.BrowserSearch
{
    public class Main : IPlugin, ISettingProvider, IReloadable
    {
        public static string PluginID => "E5A9FC7A3F7F4320BE612DA95C57C32D";
        public string Name => "Browser Search";
        public string Description => "ブラウザの履歴を検索します。";

        public IEnumerable<PluginAdditionalOption> AdditionalOptions => new List<PluginAdditionalOption>()
        {
            new()
            {
                Key = MaxResults,
                DisplayLabel = "表示する結果の数",
                DisplayDescription = "表示する結果の最大数です。-1に設定することで、すべてを表示できます（パフォーマンスが低下する可能性があります）。",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Numberbox,
                NumberValue = 15
            },
            new()
            {
                Key = SingleProfile,
                DisplayLabel = "ブラウザのプロファイル",
                DisplayDescription = "履歴が読み込まれるブラウザのプロファイル名。\n" +
                                     "空の場合は、すべてのプロファイルの履歴が読み込まれます。",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Textbox
            }
        };

        private const string MaxResults = nameof(MaxResults);
        private const string SingleProfile = nameof(SingleProfile);
        private PluginInitContext? _context;
        private IBrowser? _defaultBrowser;
        private long _lastUpdateTickCount = -300L;
        private int _maxResults;
        private string? _selectedProfileName;

        public void Init(PluginInitContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            InitDefaultBrowser();
        }

        public void ReloadData()
        {
            if (_context is null)
            {
                return;
            }

            // When the plugin is disabled and then re-enabled,
            // ReloadData() is called multiple times so this is needed
            if (Environment.TickCount64 - _lastUpdateTickCount >= 300)
            {
                InitDefaultBrowser();
                _lastUpdateTickCount = Environment.TickCount64;
            }
        }

        public Control CreateSettingPanel()
        {
            throw new NotImplementedException();
        }

        public void UpdateSettings(PowerLauncherPluginSettings settings)
        {
            _maxResults = (int)(settings?.AdditionalOptions?.FirstOrDefault(x => x.Key == MaxResults)?.NumberValue ?? 15);

            PluginAdditionalOption? profile = settings?.AdditionalOptions?.FirstOrDefault(x => x.Key == SingleProfile);
            if (profile is not null && profile.TextValue?.Length > 0)
            {
                _selectedProfileName = profile.TextValue;
            }
            else
            {
                _selectedProfileName = null;
            }
        }

        private void InitDefaultBrowser()
        {
            // Retrieve default browser info
            BrowserInfo.UpdateIfTimePassed();

            // It takes some time until BrowserInfo is updated, wait up to 500 ms
            int msSlept = 0;
            while (BrowserInfo.Name is null && msSlept < 500)
            {
                Thread.Sleep(50);
                msSlept += 50;
            }

            if (BrowserInfo.Name is null)
            {
                Log.Error("デフォルトのブラウザ名を取得できませんでした。", typeof(Main));
                return;
            }

            _defaultBrowser = null;
            string localappdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string roamingappdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            switch (BrowserInfo.Name)
            {
                case "Arc":
                    _defaultBrowser = new Chromium(
                        Path.Join(localappdata, @"Packages\TheBrowserCompany.Arc_ttt1ap7aakyb4\LocalCache\Local\Arc\User Data"), _selectedProfileName);
                    break;
                case "Brave":
                    _defaultBrowser = new Chromium(
                        Path.Join(localappdata, @"BraveSoftware\Brave-Browser\User Data"), _selectedProfileName);
                    break;
                case "Brave Beta":
                    _defaultBrowser = new Chromium(
                        Path.Join(localappdata, @"BraveSoftware\Brave-Browser-Beta\User Data"), _selectedProfileName
                    );
                    break;
                case "Brave Nightly":
                    _defaultBrowser = new Chromium(
                        Path.Join(localappdata, @"BraveSoftware\Brave-Browser-Nightly\User Data"), _selectedProfileName
                    );
                    break;
                case "Google Chrome":
                    _defaultBrowser = new Chromium(
                        Path.Join(localappdata, @"Google\Chrome\User Data"), _selectedProfileName
                    );
                    break;
                case "Google Chrome Beta":
                    _defaultBrowser = new Chromium(
                        Path.Join(localappdata, @"Google\Chrome Beta\User Data"), _selectedProfileName
                    );
                    break;
                case "Google Chrome Dev":
                    _defaultBrowser = new Chromium(
                        Path.Join(localappdata, @"Google\Chrome Dev\User Data"), _selectedProfileName
                    );
                    break;
                case "Google Chrome Canary":
                    _defaultBrowser = new Chromium(
                        Path.Join(localappdata, @"Google\Chrome SxS\User Data"), _selectedProfileName
                    );
                    break;
                case "Microsoft Edge":
                    _defaultBrowser = new Chromium(
                        Path.Join(localappdata, @"Microsoft\Edge\User Data"), _selectedProfileName
                    );
                    break;
                case "Microsoft Edge Beta":
                    _defaultBrowser = new Chromium(
                        Path.Join(localappdata, @"Microsoft\Edge Beta\User Data"), _selectedProfileName
                    );
                    break;
                case "Microsoft Edge Dev":
                    _defaultBrowser = new Chromium(
                        Path.Join(localappdata, @"Microsoft\Edge Dev\User Data"), _selectedProfileName
                    );
                    break;
                case "Microsoft Edge Canary":
                    _defaultBrowser = new Chromium(
                        Path.Join(localappdata, @"Microsoft\Edge SxS\User Data"), _selectedProfileName
                    );
                    break;
                case "Opera GX":
                    _defaultBrowser = new OperaGX(
                        Path.Join(roamingappdata, @"Opera Software\Opera GX Stable"), _selectedProfileName
                    );
                    break;
                case "Thorium":
                    _defaultBrowser = new Chromium(
                        Path.Join(localappdata, @"Thorium\User Data"), _selectedProfileName
                    );
                    break;
                case "Vivaldi":
                    _defaultBrowser = new Chromium(
                        Path.Join(localappdata, @"Vivaldi\User Data"), _selectedProfileName
                    );
                    break;
                case "Wavebox":
                    _defaultBrowser = new Chromium(
                        Path.Join(localappdata, @"WaveboxApp\User Data"), _selectedProfileName
                    );
                    break;
                case "Firefox":
                    _defaultBrowser = new Firefox(
                        Path.Join(roamingappdata, @"Mozilla\Firefox"), _selectedProfileName
                    );
                    break;
                default:
                    Log.Error($"サポートされていない/認識されないデフォルトブラウザ '{BrowserInfo.Name}'", typeof(Main));
                    MessageBox.Show($"ブラウザ '{BrowserInfo.Name}' はサポートされていません。", "BrowserSearch");
                    return;
            }

            Log.Info($"ブラウザ'{BrowserInfo.Name}'の初期化", typeof(Main));
            _defaultBrowser.Init();
        }

        public List<Result> Query(Query query)
        {
            ArgumentNullException.ThrowIfNull(query);
            if (_defaultBrowser is null)
            {
                return [];
            }

            List<Result> history = _defaultBrowser.GetHistory();
            // ユーザーがActionKeyword（デフォルトでは "b?"）を入力した場合に発生します。
            if (string.IsNullOrEmpty(query.Search))
            {
                // ここで履歴全体を返すと検索に時間がかかるので、一部の項目だけを返すようにする。
                int amount = _maxResults == -1 ? 15 : _maxResults;
                return history.TakeLast(amount).ToList();
            }

            List<Result> results = new(history.Count);
            for (int i = 0; i < history.Count; i++)
            {
                Result r = history[i];

                int score = CalculateScore(query.Search, r.Title, r.SubTitle);
                if (score <= 0)
                {
                    continue;
                }

                r.Score = score;
                results.Add(r);
            }

            if (_maxResults != -1)
            {
                // すべての検索エントリーのUIをレンダリングするのは遅いので、上位の結果のみを表示する。
                results.Sort((x, y) => y.Score.CompareTo(x.Score));
                results = results.Take(_maxResults).ToList();
            }

            return results;
        }

        private int CalculateScore(string query, string title, string url)
        {
            // PT RunのFuzzySearchは遅すぎるし、履歴には通常たくさんの項目があります、
            // より速い（しかし精度は低い）方法で手動で得点を計算できます。
            float titleScore = title.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                ? ((float)query.Length / (float)title.Length * 100f)
                : 0;
            float urlScore = url.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                ? ((float)query.Length / (float)url.Length * 100f)
                : 0;

            float score = new[] { titleScore, urlScore }.Max();
            score += _defaultBrowser!.CalculateExtraScore(query, title, url);

            return (int)score;
        }
    }

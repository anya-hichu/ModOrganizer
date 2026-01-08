using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Microsoft.Extensions.DependencyInjection;
using ModOrganizer.Backups;
using ModOrganizer.Commands;
using ModOrganizer.Configs;
using ModOrganizer.Json.Penumbra.DefaultMods;
using ModOrganizer.Json.Penumbra.Groups;
using ModOrganizer.Json.Penumbra.LocalModDatas;
using ModOrganizer.Json.Penumbra.ModMetas;
using ModOrganizer.Json.Penumbra.SortOrders;
using ModOrganizer.Json.Readers;
using ModOrganizer.Mods;
using ModOrganizer.Rules;
using ModOrganizer.Windows;
using ModOrganizer.Windows.Configs;
using ModOrganizer.Windows.Results;
using ModOrganizer.Windows.Results.Rules;

namespace ModOrganizer.Providers;

public class RootProvider : CachedProvider
{
    private IDalamudPluginInterface PluginInterface { get; init; }

    [PluginService] public IChatGui? ChatGui { get; set; }
    [PluginService] public ICommandManager? CommandManager { get; set; }
    [PluginService] public INotificationManager? NotificationManager { get; set; }
    [PluginService] public IPluginLog? PluginLog { get; set; }

    public RootProvider(IDalamudPluginInterface pluginInterface)
    {
        PluginInterface = pluginInterface;

        PluginInterface.Inject(this);
    }

    protected override ServiceProvider BuildServiceProvider()
    {
        var collection = new ServiceCollection();

        AddISingletons(collection);
        AddSingletons(collection);

        return collection.BuildServiceProvider();
    }

    private void AddISingletons(IServiceCollection collection)
    {
        if (ChatGui != null) collection.AddSingleton(ChatGui);
        if (CommandManager != null) collection.AddSingleton(CommandManager);
        if (NotificationManager != null) collection.AddSingleton(NotificationManager);
        if (PluginInterface != null) collection.AddSingleton(PluginInterface);
        if (PluginLog != null) collection.AddSingleton(PluginLog);

        collection
            .AddSingleton<IRuleDefaults, RuleDefaults>(p => new())
            .AddSingleton<IConfigDefault, ConfigDefault>(p => new(p.GetRequiredService<IRuleDefaults>()))
            .AddSingleton<IConfigLoader, ConfigLoader>(p => new(p.GetRequiredService<IConfigDefault>(), p.GetRequiredService<IDalamudPluginInterface>()))
            .AddSingleton(p => p.GetRequiredService<IConfigLoader>().GetOrDefault())

            .AddSingleton<IReaderProvider, ReaderProvider>(p => new(p.GetRequiredService<IPluginLog>()))

            .AddSingleton<IModInterop, ModInterop>(rootProvider =>
            {
                var readerProvider = rootProvider.GetRequiredService<IReaderProvider>();

                return new(rootProvider.GetRequiredService<ICommandManager>(), readerProvider.Get<IDefaultModReader>(),
                    readerProvider.Get<IGroupReaderFactory>(), readerProvider.Get<ILocalModDataReader>(), readerProvider.Get<IModMetaReader>(),
                    rootProvider.GetRequiredService<IDalamudPluginInterface>(), rootProvider.GetRequiredService<IPluginLog>(), readerProvider.Get<ISortOrderReader>());
            })

            .AddSingleton<IRuleEvaluator, RuleEvaluator>(p => new(p.GetRequiredService<IPluginLog>()))

            .AddSingleton<IBackupManager, BackupManager>(p => new(p.GetRequiredService<IConfig>(), p.GetRequiredService<IModInterop>(),
                p.GetRequiredService<IDalamudPluginInterface>(), p.GetRequiredService<IPluginLog>(), p.GetRequiredService<IReaderProvider>().Get<ISortOrderReader>()))

            .AddSingleton<IModProcessor, ModProcessor>(p => new(p.GetRequiredService<IBackupManager>(), p.GetRequiredService<IConfig>(),
                p.GetRequiredService<IModInterop>(), p.GetRequiredService<IPluginLog>(), p.GetRequiredService<IRuleEvaluator>()))

            .AddSingleton<IModAutoProcessor, ModAutoProcessor>(p => new(p.GetRequiredService<IConfig>(), p.GetRequiredService<INotificationManager>(),
                p.GetRequiredService<IModInterop>(), p.GetRequiredService<IModProcessor>(), p.GetRequiredService<IPluginLog>()))

            .AddSingleton<IModFileSystem, ModFileSystem>(p => new(p.GetRequiredService<IModInterop>()))
            .AddSingleton<ICommandPrinter, CommandPrinter>(p => new(p.GetService<IChatGui>()))
            .AddSingleton<ICommand, Command>(p => new(p.GetRequiredService<ICommandManager>(), p.GetRequiredService<ICommandPrinter>(), 
                p.GetRequiredService<AboutWindow>().Toggle, p.GetRequiredService<BackupWindow>().Toggle, p.GetRequiredService<ConfigWindow>().Toggle, 
                p.GetRequiredService<ConfigExportWindow>().Toggle, p.GetRequiredService<ConfigImportWindow>().Toggle, p.GetRequiredService<MainWindow>().Toggle, 
                p.GetRequiredService<PreviewWindow>().Toggle));
    }

    private static void AddSingletons(IServiceCollection collection)
    {
        collection
            .AddSingleton<AboutWindow>(p => new())

            .AddSingleton<BackupResultState>(p => new(p.GetRequiredService<IBackupManager>(), p.GetRequiredService<IModInterop>(), p.GetRequiredService<IPluginLog>()))
            .AddSingleton<BackupWindow>(p => new(p.GetRequiredService<IBackupManager>(), p.GetRequiredService<BackupResultState>(), p.GetRequiredService<IConfig>(), 
                p.GetRequiredService<IModInterop>(), p.GetRequiredService<IPluginLog>()))

            .AddSingleton<ConfigWindow>(p => new(p.GetRequiredService<IConfig>(), p.GetRequiredService<IDalamudPluginInterface>(), 
                p.GetRequiredService<BackupWindow>().Toggle))
            .AddSingleton<ConfigExportWindow>(p => new())
            .AddSingleton<ConfigImportWindow>(p => new())
            
            .AddSingleton<RuleResultState>(p => new(p.GetRequiredService<IBackupManager>(), p.GetRequiredService<IConfig>(), p.GetRequiredService<IModInterop>(), p.GetRequiredService<IModProcessor>(), p.GetRequiredService<IPluginLog>()))
            .AddSingleton<EvaluationResultState>(p => new(p.GetRequiredService<IModInterop>(), p.GetRequiredService<IPluginLog>()))
            .AddSingleton<MainWindow>(p => new(p.GetRequiredService<IConfig>(), p.GetRequiredService<IModInterop>(), p.GetRequiredService<EvaluationResultState>(), 
                p.GetRequiredService<IModFileSystem>(), p.GetRequiredService<RuleResultState>(), p.GetRequiredService<BackupWindow>().Toggle, 
                p.GetRequiredService<ConfigWindow>().Toggle, p.GetRequiredService<PreviewWindow>().Toggle))

            .AddSingleton<RuleResultFileSystem>(p => new(p.GetRequiredService<RuleResultState>()))
            .AddSingleton<PreviewWindow>(p => new(p.GetRequiredService<RuleResultFileSystem>()))
            
            .AddSingleton(p =>
            {
                var windowSystem = new WindowSystem(nameof(ModOrganizer));

                windowSystem.AddWindow(p.GetRequiredService<AboutWindow>());
                windowSystem.AddWindow(p.GetRequiredService<BackupWindow>());
                windowSystem.AddWindow(p.GetRequiredService<ConfigWindow>());
                windowSystem.AddWindow(p.GetRequiredService<ConfigExportWindow>());
                windowSystem.AddWindow(p.GetRequiredService<ConfigImportWindow>());
                windowSystem.AddWindow(p.GetRequiredService<MainWindow>());
                windowSystem.AddWindow(p.GetRequiredService<PreviewWindow>());

                return windowSystem;
            });
    }
}

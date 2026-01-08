using Dalamud.Game.Text;
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
using ModOrganizer.Windows.States;

namespace ModOrganizer.Providers;

public class RootProvider : CachedProvider
{
    [PluginService] public IChatGui? ChatGui { get; set; }
    [PluginService] public ICommandManager? CommandManager { get; set; }
    [PluginService] public INotificationManager? NotificationManager { get; set; }
    [PluginService] public IDalamudPluginInterface? PluginInterface { get; set; }
    [PluginService] public IPluginLog? PluginLog { get; set; }

    protected override ServiceProvider BuildServiceProvider()
    {
        var collection = new ServiceCollection();

        AddAbstractSingletons(collection);
        AddConcreteSingletons(collection);

        return collection.BuildServiceProvider();
    }

    private void AddAbstractSingletons(ServiceCollection collection)
    {
        if (ChatGui != null) collection.AddSingleton(ChatGui);
        if (CommandManager != null) collection.AddSingleton(CommandManager);
        if (NotificationManager != null) collection.AddSingleton(NotificationManager);
        if (PluginInterface != null) collection.AddSingleton(PluginInterface);
        if (PluginLog != null) collection.AddSingleton(PluginLog);

        collection.AddSingleton<IRuleDefaults, RuleDefaults>(p => new());
        collection.AddSingleton<IConfigDefault, ConfigDefault>(p => new(p.GetRequiredService<IRuleDefaults>()));
        collection.AddSingleton<IConfigLoader, ConfigLoader>(p => new(p.GetRequiredService<IConfigDefault>(), p.GetRequiredService<IDalamudPluginInterface>()));

        collection.AddSingleton(p => p.GetRequiredService<IConfigLoader>().GetOrDefault());

        collection.AddSingleton<IReaderProvider, ReaderProvider>(p => new(p.GetRequiredService<IPluginLog>()));
        collection.AddSingleton<IModInterop, ModInterop>(p =>
        {
            var readerProvider = p.GetRequiredService<IReaderProvider>();
            return new(p.GetRequiredService<ICommandManager>(), readerProvider.Get<IDefaultModReader>(),
                readerProvider.Get<IGroupReaderFactory>(), readerProvider.Get<ILocalModDataReader>(), readerProvider.Get<IModMetaReader>(),
                p.GetRequiredService<IDalamudPluginInterface>(), p.GetRequiredService<IPluginLog>(), readerProvider.Get<ISortOrderReader>());
        });

        collection.AddSingleton<IRuleEvaluator, RuleEvaluator>(p => new(p.GetRequiredService<IPluginLog>()));

        collection.AddSingleton<IBackupManager, BackupManager>(p => new(p.GetRequiredService<IConfig>(), p.GetRequiredService<IModInterop>(),
            p.GetRequiredService<IDalamudPluginInterface>(), p.GetRequiredService<IPluginLog>(), p.GetRequiredService<IReaderProvider>().Get<ISortOrderReader>()));

        collection.AddSingleton<IModProcessor, ModProcessor>(p => new(p.GetRequiredService<IBackupManager>(), p.GetRequiredService<IConfig>(),
            p.GetRequiredService<IModInterop>(), p.GetRequiredService<IPluginLog>(), p.GetRequiredService<IRuleEvaluator>()));

        collection.AddSingleton<IModAutoProcessor, ModAutoProcessor>(p => new(p.GetRequiredService<IConfig>(), p.GetRequiredService<INotificationManager>(),
            p.GetRequiredService<IModInterop>(), p.GetRequiredService<IModProcessor>(), p.GetRequiredService<IPluginLog>()));

        collection.AddSingleton<IModFileSystem, ModFileSystem>(p => new(p.GetRequiredService<IModInterop>()));

        collection.AddSingleton<ICommandPrinter, CommandPrinter>(p => new(p.GetService<IChatGui>()));

        collection.AddSingleton<ICommand, Command>(p => new(p.GetRequiredService<ICommandManager>(), p.GetRequiredService<ICommandPrinter>(), p.GetRequiredService<AboutWindow>().Toggle, 
            p.GetRequiredService<BackupWindow>().Toggle, p.GetRequiredService<ConfigWindow>().Toggle, p.GetRequiredService<ConfigExportWindow>().Toggle, 
            p.GetRequiredService<ConfigImportWindow>().Toggle, p.GetRequiredService<MainWindow>().Toggle, p.GetRequiredService<PreviewWindow>().Toggle));
    }

    private void AddConcreteSingletons(ServiceCollection collection)
    {
        collection.AddSingleton(p => new AboutWindow());
        collection.AddSingleton(p => new BackupWindow(p.GetRequiredService<IBackupManager>(), p.GetRequiredService<IConfig>(), p.GetRequiredService<IModInterop>(), p.GetRequiredService<IPluginLog>()));
        collection.AddSingleton(p => new ConfigWindow(p.GetRequiredService<IConfig>(), p.GetRequiredService<IDalamudPluginInterface>(), p.GetRequiredService<BackupWindow>().Toggle));
        collection.AddSingleton(p => new ConfigExportWindow());
        collection.AddSingleton(p => new ConfigImportWindow());

        collection.AddSingleton(p => new RuleState(p.GetRequiredService<IBackupManager>(), p.GetRequiredService<IConfig>(), p.GetRequiredService<IModInterop>(), p.GetRequiredService<IModProcessor>(), p.GetRequiredService<IPluginLog>()));
        collection.AddSingleton(p => new MainWindow(p.GetRequiredService<IConfig>(), p.GetRequiredService<IModInterop>(), p.GetRequiredService<IModFileSystem>(), p.GetRequiredService<IPluginLog>(), p.GetRequiredService<RuleState>(),
            p.GetRequiredService<BackupWindow>().Toggle, p.GetRequiredService<ConfigWindow>().Toggle, p.GetRequiredService<PreviewWindow>().Toggle));
        collection.AddSingleton(p => new PreviewWindow(p.GetRequiredService<RuleState>()));

        collection.AddSingleton(p =>
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

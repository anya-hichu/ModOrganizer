using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Microsoft.Extensions.DependencyInjection;
using ModOrganizer.Backups;
using ModOrganizer.Commands;
using ModOrganizer.Configs;
using ModOrganizer.Configs.Defaults;
using ModOrganizer.Configs.Loaders;
using ModOrganizer.Configs.Mergers;
using ModOrganizer.Json.ConfigDatas;
using ModOrganizer.Json.Penumbra.Containers;
using ModOrganizer.Json.Penumbra.DefaultMods;
using ModOrganizer.Json.Penumbra.Groups;
using ModOrganizer.Json.Penumbra.Groups.Bases;
using ModOrganizer.Json.Penumbra.Groups.Combinings;
using ModOrganizer.Json.Penumbra.Groups.Generics;
using ModOrganizer.Json.Penumbra.Groups.Imcs;
using ModOrganizer.Json.Penumbra.Groups.Multis;
using ModOrganizer.Json.Penumbra.Groups.Singles;
using ModOrganizer.Json.Penumbra.LocalModDatas;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Atchs;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Atchs.Entries;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Atrs;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Eqdps;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Eqps;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Ests;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Geqps;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Gmps;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Gmps.Entries;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs.Entries;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs.Identifiers;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Rsps;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Shps;
using ModOrganizer.Json.Penumbra.Manipulations.Wrappers;
using ModOrganizer.Json.Penumbra.Manipulations.Wrappers.Generics;
using ModOrganizer.Json.Penumbra.ModMetas;
using ModOrganizer.Json.Penumbra.Options;
using ModOrganizer.Json.Penumbra.Options.Containers;
using ModOrganizer.Json.Penumbra.Options.Imcs;
using ModOrganizer.Json.Penumbra.Options.Imcs.AttributeMasks;
using ModOrganizer.Json.Penumbra.Options.Imcs.Generics;
using ModOrganizer.Json.Penumbra.Options.Imcs.IsDisableSubMods;
using ModOrganizer.Json.Penumbra.SortOrders;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using ModOrganizer.Json.RuleDatas;
using ModOrganizer.Json.RuleExports;
using ModOrganizer.Mods;
using ModOrganizer.Rules;
using ModOrganizer.Shared;
using ModOrganizer.Windows;
using ModOrganizer.Windows.Configs;
using ModOrganizer.Windows.Managers;
using ModOrganizer.Windows.Results.Backups;
using ModOrganizer.Windows.Results.Evaluations;
using ModOrganizer.Windows.Results.Rules;
using ModOrganizer.Windows.Togglers;

namespace ModOrganizer.Providers;

public class PluginProvider : CachedProvider, IPluginProvider
{
    private IDalamudPluginInterface PluginInterface { get; init; }

    [PluginService] public IChatGui? MaybeChatGui { get; set; }
    [PluginService] public ICommandManager? MaybeCommandManager { get; set; }
    [PluginService] public INotificationManager? MaybeNotificationManager { get; set; }
    [PluginService] public IPluginLog? MaybePluginLog { get; set; }

    public PluginProvider(IDalamudPluginInterface pluginInterface)
    {
        PluginInterface = pluginInterface;

        PluginInterface.Inject(this);
    }

    protected override ServiceProvider BuildServiceProvider()
    {
        var collection = new ServiceCollection();

        if (MaybeChatGui != null) collection.AddSingleton(MaybeChatGui);
        if (MaybeCommandManager != null) collection.AddSingleton(MaybeCommandManager);
        if (MaybeNotificationManager != null) collection.AddSingleton(MaybeNotificationManager);
        if (MaybePluginLog != null) collection.AddSingleton(MaybePluginLog);

        collection
            .AddSingleton<IPluginProvider>(this)

            .AddSingleton(PluginInterface)
            .AddSingleton<IRuleDefaults, RuleDefaults>()
            .AddSingleton<IConfigDefault, ConfigDefault>()
            .AddSingleton<IConfigLoader, ConfigLoader>()
            .AddSingleton(p => p.GetRequiredService<IConfigLoader>().GetOrDefault())

            .AddSingleton<IModInterop, ModInterop>()
            .AddSingleton<IRuleEvaluator, RuleEvaluator>()
            .AddSingleton<IBackupManager, BackupManager>()
            .AddSingleton<IModProcessor, ModProcessor>()
            .AddSingleton<IModAutoProcessor, ModAutoProcessor>()
            .AddSingleton<IModFileSystem, ModFileSystem>()
            .AddSingleton<ICommandPrinter, CommandPrinter>(p => new(p.GetService<IChatGui>()))

            .AddSingleton<ICommand, Command>()

            // Json
            .AddSingleton<IElementReader, ElementReader>()
            .AddSingleton<IDefaultModReader, DefaultModReader>()
            .AddSingleton<ILocalModDataReader, LocalModDataReader>()
            .AddSingleton<IModMetaReader, ModMetaReader>()
            .AddSingleton<ISortOrderReader, SortOrderReader>()
            .AddSingleton<IConfigDataReader, ConfigDataReader>()

            .AddSingleton<IConverter<ConfigData, Config>, ConfigDataConverter>()
            .AddSingleton<IConverter<RuleData, Rule>, RuleDataConverter>()
            .AddSingleton<IReader<RuleData>, RuleDataReader>()

            .AddSingleton<IReader<Container>, ContainerReader>()
            .AddSingleton<IReader<NamedContainer>, NamedContainerReader>()

            .AddSingleton<IGroupBaseReader, GroupBaseReader>()
            .AddKeyedSingleton<IReader<Group>, GroupCombiningReader>(GroupCombiningReader.TYPE)
            .AddKeyedSingleton<IReader<Group>, GroupImcReader>(GroupImcReader.TYPE)
            .AddKeyedSingleton<IReader<Group>, GroupMultiReader>(GroupMultiReader.TYPE)
            .AddKeyedSingleton<IReader<Group>, GroupSingleReader>(GroupSingleReader.TYPE)

            .AddSingleton<IGroupGenericReader, GroupGenericReader>(p => new(
                p.GetRequiredService<IElementReader>(),
                p.GetRequiredKeyedService<IReader<Group>>(GroupCombiningReader.TYPE),
                p.GetRequiredKeyedService<IReader<Group>>(GroupImcReader.TYPE),
                p.GetRequiredKeyedService<IReader<Group>>(GroupMultiReader.TYPE),
                p.GetRequiredKeyedService<IReader<Group>>(GroupSingleReader.TYPE),
                p.GetRequiredService<IPluginLog>()
            ))

            .AddSingleton<IReader<MetaImcEntry>, MetaImcEntryReader>()
            .AddSingleton<IReader<MetaImcIdentifier>, MetaImcIdentifierReader>()

            .AddSingleton<IReader<MetaAtchEntry>, MetaAtchEntryReader>()
            .AddSingleton<IReader<MetaAtch>, MetaAtchReader>()
            .AddKeyedSingleton<IReader<ManipulationWrapper>, MetaAtchWrapperReader>(MetaAtchWrapperReader.TYPE)

            .AddSingleton<IReader<MetaAtr>, MetaAtrReader>()
            .AddKeyedSingleton<IReader<ManipulationWrapper>, MetaAtrWrapperReader>(MetaAtrWrapperReader.TYPE)

            .AddSingleton<IReader<MetaEqdp>, MetaEqdpReader>()
            .AddKeyedSingleton<IReader<ManipulationWrapper>, MetaEqdpWrapperReader>(MetaEqdpWrapperReader.TYPE)

            .AddSingleton<IReader<MetaEqp>, MetaEqpReader>()
            .AddKeyedSingleton<IReader<ManipulationWrapper>, MetaEqpWrapperReader>(MetaEqpWrapperReader.TYPE)

            .AddSingleton<IReader<MetaEst>, MetaEstReader>()
            .AddKeyedSingleton<IReader<ManipulationWrapper>, MetaEstWrapperReader>(MetaEstWrapperReader.TYPE)

            .AddSingleton<IReader<MetaGeqp>, MetaGeqpReader>()
            .AddKeyedSingleton<IReader<ManipulationWrapper>, MetaGeqpWrapperReader>(MetaGeqpWrapperReader.TYPE)

            .AddSingleton<IReader<MetaGmpEntry>, MetaGmpEntryReader>()
            .AddSingleton<IReader<MetaGmp>, MetaGmpReader>()
            .AddKeyedSingleton<IReader<ManipulationWrapper>, MetaGmpWrapperReader>(MetaGmpWrapperReader.TYPE)

            .AddSingleton<IReader<MetaImc>, MetaImcReader>()
            .AddKeyedSingleton<IReader<ManipulationWrapper>, MetaImcWrapperReader>(MetaImcWrapperReader.TYPE)

            .AddSingleton<IReader<MetaRsp>, MetaRspReader>()
            .AddKeyedSingleton<IReader<ManipulationWrapper>, MetaRspWrapperReader>(MetaRspWrapperReader.TYPE)

            .AddSingleton<IReader<MetaShp>, MetaShpReader>()
            .AddKeyedSingleton<IReader<ManipulationWrapper>, MetaShpWrapperReader>(MetaShpWrapperReader.TYPE)

            .AddSingleton<IManipulationWrapperGenericReader, ManipulationWrapperGenericReader>(p => new(
                p.GetRequiredKeyedService<IReader<ManipulationWrapper>>(MetaAtchWrapperReader.TYPE),
                p.GetRequiredKeyedService<IReader<ManipulationWrapper>>(MetaAtrWrapperReader.TYPE),
                p.GetRequiredKeyedService<IReader<ManipulationWrapper>>(MetaEqdpWrapperReader.TYPE),
                p.GetRequiredKeyedService<IReader<ManipulationWrapper>>(MetaEqpWrapperReader.TYPE),
                p.GetRequiredKeyedService<IReader<ManipulationWrapper>>(MetaEstWrapperReader.TYPE),
                p.GetRequiredKeyedService<IReader<ManipulationWrapper>>(MetaGeqpWrapperReader.TYPE),
                p.GetRequiredKeyedService<IReader<ManipulationWrapper>>(MetaGmpWrapperReader.TYPE),
                p.GetRequiredKeyedService<IReader<ManipulationWrapper>>(MetaImcWrapperReader.TYPE),
                p.GetRequiredKeyedService<IReader<ManipulationWrapper>>(MetaRspWrapperReader.TYPE),
                p.GetRequiredKeyedService<IReader<ManipulationWrapper>>(MetaShpWrapperReader.TYPE),
                p.GetRequiredService<IPluginLog>()
            ))

            .AddSingleton<IReader<Option>, OptionReader>()
            .AddSingleton<IReader<OptionContainer>, OptionContainerReader>()
            .AddSingleton<IOptionImcAttributeMaskReader, OptionImcAttributeMaskReader>()
            .AddSingleton<IOptionImcIsDisableSubModReader, OptionImcIsDisableSubModReader>()
            .AddSingleton<IOptionImcGenericReader, OptionImcGenericReader>()

            // Windows
            .AddSingleton<IWindowToggler, WindowToggler>()
            .AddSingleton<IWindowManager, WindowManager>()

            .AddSingleton<AboutWindow>()

            .AddSingleton<IBackupResultState, BackupResultState>()
            .AddSingleton<BackupWindow>()

            .AddSingleton<ConfigWindow>()
            .AddTransient<ConfigExportWindow>()

            .AddSingleton<IConfigMerger, ConfigMerger>()
            .AddTransient<ConfigImportWindow>()

            .AddSingleton<IRuleResultState, RuleResultState>()
            .AddSingleton<IEvaluationResultState, EvaluationResultState>()
            .AddSingleton<MainWindow>()

            .AddSingleton<IRuleResultFileSystem, RuleResultFileSystem>()
            .AddSingleton<PreviewWindow>()

            .AddSingleton(p =>
            {
                var windowSystem = new WindowSystem(nameof(ModOrganizer));

                windowSystem.AddWindow(p.GetRequiredService<AboutWindow>());
                windowSystem.AddWindow(p.GetRequiredService<BackupWindow>());
                windowSystem.AddWindow(p.GetRequiredService<ConfigWindow>());
                windowSystem.AddWindow(p.GetRequiredService<MainWindow>());
                windowSystem.AddWindow(p.GetRequiredService<PreviewWindow>());

                var windowManager = p.GetRequiredService<IWindowManager>();
                var windowToggler = p.GetRequiredService<IWindowToggler>();
                windowToggler.MaybeWindowSystem = windowManager.MaybeWindowSystem = windowSystem;

                return windowSystem;
            });


        return collection.BuildServiceProvider();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace ModOrganizer.GameData;

public static class FullEquipTypeExtensions
{
    public static bool IsUnknown(this FullEquipType type) => type is FullEquipType.Unknown or FullEquipType.UnknownMainhand or FullEquipType.UnknownOffhand;

    public static bool IsWeapon(this FullEquipType type) => type switch
    {
        FullEquipType.Fists => true,
        FullEquipType.Sword => true,
        FullEquipType.Axe => true,
        FullEquipType.Bow => true,
        FullEquipType.Lance => true,
        FullEquipType.Staff => true,
        FullEquipType.Wand => true,
        FullEquipType.Book => true,
        FullEquipType.Daggers => true,
        FullEquipType.Broadsword => true,
        FullEquipType.Gun => true,
        FullEquipType.Orrery => true,
        FullEquipType.Katana => true,
        FullEquipType.Rapier => true,
        FullEquipType.Cane => true,
        FullEquipType.Gunblade => true,
        FullEquipType.Glaives => true,
        FullEquipType.Scythe => true,
        FullEquipType.Nouliths => true,
        FullEquipType.Shield => true,
        FullEquipType.Brush => true,
        FullEquipType.Twinfangs => true,
        FullEquipType.UnknownMainhand => true,
        _ => false,
    };

    public static bool IsTool(this FullEquipType type) => type switch
    {
        FullEquipType.Saw => true,
        FullEquipType.CrossPeinHammer => true,
        FullEquipType.RaisingHammer => true,
        FullEquipType.LapidaryHammer => true,
        FullEquipType.Knife => true,
        FullEquipType.Needle => true,
        FullEquipType.Alembic => true,
        FullEquipType.Frypan => true,
        FullEquipType.Pickaxe => true,
        FullEquipType.Hatchet => true,
        FullEquipType.FishingRod => true,
        FullEquipType.ClawHammer => true,
        FullEquipType.File => true,
        FullEquipType.Pliers => true,
        FullEquipType.GrindingWheel => true,
        FullEquipType.Awl => true,
        FullEquipType.SpinningWheel => true,
        FullEquipType.Mortar => true,
        FullEquipType.CulinaryKnife => true,
        FullEquipType.Sledgehammer => true,
        FullEquipType.GardenScythe => true,
        FullEquipType.Gig => true,
        _ => false,
    };

    public static bool IsEquipment(this FullEquipType type) => type switch
    {
        FullEquipType.Head => true,
        FullEquipType.Body => true,
        FullEquipType.Hands => true,
        FullEquipType.Legs => true,
        FullEquipType.Feet => true,
        _ => false,
    };

    public static bool IsAccessory(this FullEquipType type) => type switch
    {
        FullEquipType.Ears => true,
        FullEquipType.Neck => true,
        FullEquipType.Wrists => true,
        FullEquipType.Finger => true,
        _ => false,
    };

    public static bool IsBonus(this FullEquipType type) => type switch
    {
        FullEquipType.Glasses => true,
        _ => false,
    };

    internal static bool IsOffhand(this FullEquipType type) => type switch
    {
        FullEquipType.FistsOff => true,
        FullEquipType.DaggersOff => true,
        FullEquipType.GunOff => true,
        FullEquipType.OrreryOff => true,
        FullEquipType.RapierOff => true,
        FullEquipType.GlaivesOff => true,
        FullEquipType.BowOff => true,
        FullEquipType.KatanaOff => true,
        FullEquipType.TwinfangsOff => true,
        FullEquipType.Palette => true,
        _ => false,
    };

    public static readonly IReadOnlyList<FullEquipType> Types = Enum.GetValues<FullEquipType>();

    public static readonly IReadOnlyList<FullEquipType> WeaponTypes = [.. Types.Where(IsWeapon).Except([FullEquipType.UnknownMainhand])];

    public static readonly IReadOnlyList<FullEquipType> ToolTypes = [.. Types.Where(IsTool)];

    public static readonly IReadOnlyList<FullEquipType> EquipmentTypes = [.. Types.Where(IsEquipment)];

    public static readonly IReadOnlyList<FullEquipType> AccessoryTypes = [.. Types.Where(IsAccessory)];

    public static readonly IReadOnlyList<FullEquipType> OffhandTypes = [.. Types.Where(IsOffhand)];

    public static readonly IReadOnlyList<FullEquipType> BonusTypes = [.. Types.Where(IsBonus)];
}

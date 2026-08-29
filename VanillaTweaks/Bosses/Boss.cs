using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Terraria;
using Terraria.ID;

namespace BossRush.VanillaTweaks.Bosses;
public struct Boss
{
    /// <summary>
    /// also effects projectiles and minions summoned by the boss
    /// </summary>
    public int? DamageMult;
    public int? Damage;
    public int? Health;
    public int? healthMult;
    public PostAI? PostAI;

    /// <summary>
    /// Extra enemies that will be affected by damage and health multipliers
    /// </summary>
    public long[]? extraEnemiesToBuff;
    /// <summary>
    /// Extra projectiles that will be affected by damage and health multipliers
    /// </summary>
    public long[]? extraProjectilesToBuff;
    public required long Type;

    public Boss(long type)
    {
        Type = type;
    }
    public override readonly bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        if (obj is Boss boss)
        {
            return SameType(this, boss);
        }
        else
        {
            return false;
        }
    }
    private readonly bool SameType(Boss boss, Boss boss2)
    {
        return boss.Type == boss2.Type;
    } 
    public static implicit operator long(Boss boss)
    {
        return boss.Type;
    }
    public static implicit operator Boss(long id)
    {
        return new Boss(){Type = id};
    }

    public static bool operator ==(Boss left, Boss right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Boss left, Boss right)
    {
        return !(left == right);
    }
}
public delegate void PostAI(NPC npc, BossRushModeBoss globalNPC);

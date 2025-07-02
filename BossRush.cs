using System;
using System.IO;
using System.Reflection;
using BossRush.Systems;
using log4net;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SteelSeries.GameSense;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Mono.Cecil.Cil.OpCodes;

namespace BossRush
{
    public class BossRush : Mod
    {
        private const bool DEBUG = true;
        public override void Load()
        {
            IL_NPC.VanillaAI_Inner += il =>
            {
                ILCursor cursor = new(il);
                il.Body.InitLocals = true;
                // find the EoC part of the if statement
                // 0x00165150 02            IL_03FC: ldarg.0
                // 0x00165151 7B94030004    IL_03FD: ldfld     int32 Terraria.NPC::aiStyle
                // 0x00165156 1A            IL_0402: ldc.i4.4
                // 0x00165157 409D280000    IL_0403: bne.un    IL_2CA5
                cursor.GotoNext(
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<NPC>(nameof(NPC.aiStyle)),
                    x => x.MatchLdcI4(4)
                );
                // go to part where it checks if it's day
                // IL_0880: call      bool Terraria.Main::IsItDay()
                //	IL_0885: ldloc.s   V_13
                //IL_0887: or
                // IL_0888: brfalse.s IL_08AC
                cursor.GotoNext(
                    x => x.MatchCall<Main>(nameof(Main.IsItDay)),
                    x => x.Match(Ldloc_S),
                    x => x.MatchOr()
                );
                ILLabel daytimeCheck = cursor.MarkLabel();
                Logger.Debug($"Daytime check instruction: {daytimeCheck.Target}");
                // code right after the despawn if statement
                // IL_08AC: ldarg.0
                // IL_08AD: ldfld     float32[] Terraria.NPC::ai
                // IL_08B2: ldc.i4.0
                // IL_08B3: ldelem.r4
                //  IL_08B4: ldc.r4    0.0
                cursor.GotoNext(
                    x => x.MatchLdarg0(),
                    x => x.MatchLdfld<NPC>(nameof(NPC.ai)),
                    x => x.MatchLdcI4(0),
                    x => x.MatchLdelemR4(),
                    x => x.MatchLdcR4(0)

                );

                ILLabel afterDespawnCheck = cursor.MarkLabel();
                cursor.GotoLabel(daytimeCheck, MoveType.AfterLabel);

                cursor.EmitDelegate(static () =>
                {
                    return ModContent.GetInstance<BossRushSystem>().isBossRushMode;
                });
                cursor.EmitBrtrue(afterDespawnCheck);
                if (DEBUG)
                {
                    string path = "DebuEoCIl.ilLog";
                    Logger.Debug($"Writing IL after adding EoC code to: {path} ");
                    File.WriteAllText(path, il.ToString());
                }

            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Avali
{
	// Token: 0x02000022 RID: 34
	[StaticConstructorOnStartup]
	public static class HarmonyPatches
	{
		// Token: 0x0600008D RID: 141 RVA: 0x000066A4 File Offset: 0x000048A4
		static HarmonyPatches()
		{
			HarmonyLib harmony = new Harmony("rimworld.erisss.avali");
			harmony.Patch(AccessTools.Method(typeof(VerbTracker), "GetVerbsCommands", null, null), null, new HarmonyMethod(HarmonyPatches.patchType, "GetVerbsCommandsPostfix", null), null, null);
			harmony.Patch(AccessTools.Method(typeof(Pawn), "GetInspectString", null, null), null, new HarmonyMethod(HarmonyPatches.patchType, "GetInspectStringPostfix", null), null, null);
			harmony.Patch(AccessTools.Method(typeof(NegativeInteractionUtility), "NegativeInteractionChanceFactor", null, null), null, new HarmonyMethod(HarmonyPatches.patchType, "NegativeInteractionChanceFactorPostfix", null), null, null);
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00006894 File Offset: 0x00004A94
		public static void GetVerbsCommandsPostfix(ref VerbTracker __instance, ref IEnumerable<Command> __result)
		{
			CompEquippable compEquippable = __instance.directOwner as CompEquippable;
			if (compEquippable == null)
			{
				return;
			}
			Thing parent = compEquippable.parent;
			CompRangedWeaponAvali compWeaponAvali = null;
			if (parent != null)
			{
				compWeaponAvali = parent.TryGetComp<CompRangedWeaponAvali>();
			}
			if (compWeaponAvali == null)
			{
				return;
			}
			string currentBindMode = compWeaponAvali.currentBindMode;
			bool disabled = false;
			string disabledReason = "CannotOrderNonControlled".Translate();
			Pawn casterPawn = compEquippable.PrimaryVerb.CasterPawn;
			if (casterPawn == null || !casterPawn.IsColonist)
			{
				return;
			}
			if (!casterPawn.Awake())
			{
				disabled = true;
				disabledReason = string.Format("NotAwake".Translate(), casterPawn.LabelShort.CapitalizeFirst());
			}
			else if (compWeaponAvali.ownerPawn != null && casterPawn != compWeaponAvali.ownerPawn)
			{
				if (currentBindMode == CompRangedWeaponAvali.bindMode.OwnerPawnOnly.ToString() || currentBindMode == CompRangedWeaponAvali.bindMode.AnyPawnInFaction.ToString())
				{
					disabled = true;
					disabledReason = "OwnerNotCasterPawn".Translate();
				}
			}
			else if (casterPawn.Drafted)
			{
				disabled = true;
				disabledReason = string.Format("ShouldBeUndrafted".Translate(), casterPawn.LabelShort.CapitalizeFirst());
			}
			List<Command> list = __result.ToList<Command>();
			list.Add(new Command_Action
			{
				action = delegate()
				{
					compWeaponAvali.EraseOwnerPawnInfo();
				},
				disabled = disabled,
				disabledReason = disabledReason,
				defaultDesc = "EraseOwnerPawnInfoDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get("UI/Commands/EraseOwnerPawnInfo", true),
				hotKey = KeyBindingDefOf.Misc5
			});
			List<Verb> allVerbs = __instance.AllVerbs;
			for (int i = 0; i < allVerbs.Count; i++)
			{
				Verb verb = allVerbs[i];
				if (verb.verbProps.hasStandardCommand)
				{
					__result = list;
				}
			}
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00006A84 File Offset: 0x00004C84
		public static void GetInspectStringPostfix(ref Pawn __instance, ref string __result)
		{
			if (!__instance.IsColonist)
			{
				return;
			}
			ThingWithComps primary = __instance.equipment.Primary;
			if (primary != null)
			{
				CompRangedWeaponAvali comp = primary.GetComp<CompRangedWeaponAvali>();
				if (comp == null)
				{
					return;
				}
				if (comp.ownerPawn != null)
				{
					__result = __result + "\n" + "EquipedWeaponOwnerPawn".Translate() + comp.ownerPawn.Name;
					return;
				}
				__result = __result + "\n" + "EquipedWeaponOwnerPawn".Translate() + "None".Translate();
			}
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00006B24 File Offset: 0x00004D24
		public static void NegativeInteractionChanceFactorPostfix(ref Pawn initiator, ref Pawn recipient, ref float __result)
		{
			if (initiator.def != ThingDefOfAvali.Avali)
			{
				return;
			}
			if (initiator.story.traits.HasTrait(TraitDefOf.Kind))
			{
				return;
			}
			if (initiator.HavePackRelation(recipient))
			{
				__result *= 1.15f;
				return;
			}
			if (recipient.def == ThingDefOfAvali.Avali)
			{
				__result *= 1.725f;
				return;
			}
			__result *= 2.3f;
		}

		// Token: 0x04000067 RID: 103
		private static readonly Type patchType = typeof(HarmonyPatches);

		// Token: 0x04000068 RID: 104
		private static readonly SimpleCurve CompatibilityFactorCurve = new SimpleCurve
		{
			{
				new CurvePoint(-2.5f, 4f),
				true
			},
			{
				new CurvePoint(-1.5f, 3f),
				true
			},
			{
				new CurvePoint(-0.5f, 2f),
				true
			},
			{
				new CurvePoint(0.5f, 1f),
				true
			},
			{
				new CurvePoint(1f, 0.75f),
				true
			},
			{
				new CurvePoint(2f, 0.5f),
				true
			},
			{
				new CurvePoint(3f, 0.4f),
				true
			}
		};

		// Token: 0x04000069 RID: 105
		private static readonly SimpleCurve OpinionFactorCurve = new SimpleCurve
		{
			{
				new CurvePoint(-100f, 6f),
				true
			},
			{
				new CurvePoint(-50f, 4f),
				true
			},
			{
				new CurvePoint(-25f, 2f),
				true
			},
			{
				new CurvePoint(0f, 1f),
				true
			},
			{
				new CurvePoint(50f, 0.1f),
				true
			},
			{
				new CurvePoint(100f, 0f),
				true
			}
		};
	}
}

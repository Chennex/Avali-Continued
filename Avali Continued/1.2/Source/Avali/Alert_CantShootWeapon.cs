using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Avali
{
	// Token: 0x02000002 RID: 2
	public class Alert_CantShootWeapon : Alert
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public Alert_CantShootWeapon()
		{
			this.defaultPriority = AlertPriority.High;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000032C8 File Offset: 0x000014C8
		public override string GetLabel()
		{
			int num = this.PawnsCantShootWeapon().Count<Thing>();
			if (num == 1)
			{
				return "PawnCantShootWeapon".Translate();
			}
			return string.Format("PawnsCantShootWeapons".Translate(), num + 1);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00003314 File Offset: 0x00001514
		private IEnumerable<Thing> PawnsCantShootWeapon()
		{
			List<Pawn> allMaps_FreeColonistsSpawned = PawnsFinder.AllMaps_FreeColonistsSpawned.ToList<Pawn>();
			for (int i = 0; i < allMaps_FreeColonistsSpawned.Count; i++)
			{
				Pawn pawn = allMaps_FreeColonistsSpawned[i];
				if (pawn.equipment.Primary != null)
				{
					CompRangedWeaponAvali compWeaponAvali = pawn.equipment.Primary.GetComp<CompRangedWeaponAvali>();
					if (compWeaponAvali == null || compWeaponAvali.ownerPawn == null)
					{
						break;
					}
					if (compWeaponAvali.currentBindMode == CompRangedWeaponAvali.bindMode.OwnerPawnOnly.ToString() && pawn != compWeaponAvali.ownerPawn)
					{
						yield return pawn;
					}
					else if (compWeaponAvali.currentBindMode == CompRangedWeaponAvali.bindMode.AnyPawnInFaction.ToString() && pawn.Faction != compWeaponAvali.ownerPawn.Faction)
					{
						yield return pawn;
					}
				}
			}
			yield break;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00003334 File Offset: 0x00001534
		public override TaggedString GetExplanation()
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<Thing> list = this.PawnsCantShootWeapon().ToList<Thing>();
			for (int i = 0; i < list.Count; i++)
			{
				Thing thing = list[i];
				stringBuilder.AppendLine("    " + thing.LabelShort.CapitalizeFirst());
			}
			return string.Format("PawnsCantShootWeaponsDesc".Translate(), stringBuilder);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x0000205F File Offset: 0x0000025F
		public override AlertReport GetReport()
		{
			return AlertReport.CulpritsAre(this.PawnsCantShootWeapon().ToList<Thing>());
		}
	}
}

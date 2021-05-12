using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Avali
{
	// Token: 0x02000044 RID: 68
	public class IngestionOutcomeDoer_GiveHediffRaceDependant : IngestionOutcomeDoer
	{
		// Token: 0x0600016C RID: 364 RVA: 0x0000DC1C File Offset: 0x0000BE1C
		protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
		{
			if (this.raceDependencies != null)
			{
				for (int i = 0; i < this.raceDependencies.Count; i++)
				{
					raceDependencies raceDependencies = this.raceDependencies[i];
					if (pawn.def == raceDependencies.race)
					{
						Hediff hediff = HediffMaker.MakeHediff(raceDependencies.hediffDef, pawn, null);
						float num;
						if (raceDependencies.severity > 0f)
						{
							num = raceDependencies.severity;
						}
						else
						{
							num = raceDependencies.hediffDef.initialSeverity;
						}
						if (raceDependencies.divideByBodySize)
						{
							num /= pawn.BodySize;
						}
						AddictionUtility.ModifyChemicalEffectForToleranceAndBodySize(pawn, raceDependencies.toleranceChemical, ref num);
						hediff.Severity = num;
						pawn.health.AddHediff(hediff, null, null, null);
						return;
					}
				}
			}
			Hediff hediff2 = HediffMaker.MakeHediff(this.hediffDef, pawn, null);
			float num2;
			if (this.severity > 0f)
			{
				num2 = this.severity;
			}
			else
			{
				num2 = this.hediffDef.initialSeverity;
			}
			if (this.divideByBodySize)
			{
				num2 /= pawn.BodySize;
			}
			AddictionUtility.ModifyChemicalEffectForToleranceAndBodySize(pawn, this.toleranceChemical, ref num2);
			hediff2.Severity = num2;
			pawn.health.AddHediff(hediff2, null, null, null);
		}

		// Token: 0x0600016D RID: 365 RVA: 0x0000DD50 File Offset: 0x0000BF50
		public override IEnumerable<StatDrawEntry> SpecialDisplayStats(ThingDef parentDef)
		{
			if (parentDef.IsDrug && this.chance >= 1f)
			{
				foreach (StatDrawEntry s in this.hediffDef.SpecialDisplayStats(StatRequest.ForEmpty()))
				{
					yield return s;
				}
			}
			yield break;
		}

		// Token: 0x0400013C RID: 316
		public HediffDef hediffDef;

		// Token: 0x0400013D RID: 317
		public float severity = -1f;

		// Token: 0x0400013E RID: 318
		public ChemicalDef toleranceChemical;

		// Token: 0x0400013F RID: 319
		public bool divideByBodySize;

		// Token: 0x04000140 RID: 320
		public List<raceDependencies> raceDependencies;
	}
}

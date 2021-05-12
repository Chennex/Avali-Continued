using System;
using Verse;

namespace Avali
{
	// Token: 0x0200003B RID: 59
	public class HediffGiver_AddHediff : HediffGiver
	{
		// Token: 0x06000139 RID: 313 RVA: 0x00002A9A File Offset: 0x00000C9A
		public override void OnIntervalPassed(Pawn pawn, Hediff cause)
		{
			if (pawn.IsHashIntervalTick(1) && !pawn.health.hediffSet.HasHediff(this.hediff, false))
			{
				base.TryApply(pawn, null);
			}
		}

		// Token: 0x04000117 RID: 279
		public bool hasHediff;
	}
}

using System;
using RimWorld;
using Verse;

namespace Avali
{
	// Token: 0x0200005B RID: 91
	public class ThoughtWorker_AvaliDeaf : ThoughtWorker
	{
		// Token: 0x060001E1 RID: 481 RVA: 0x00002E53 File Offset: 0x00001053
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			if (p.def != ThingDefOfAvali.Avali)
			{
				return false;
			}
			if (p.health.capacities.GetLevel(PawnCapacityDefOf.Hearing) > 0f)
			{
				return false;
			}
			return true;
		}
	}
}

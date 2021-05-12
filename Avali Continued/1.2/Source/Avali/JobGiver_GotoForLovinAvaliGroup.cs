using System;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x0200000F RID: 15
	public class JobGiver_GotoForLovinAvaliGroup : ThinkNode_JobGiver
	{
		// Token: 0x0600004A RID: 74 RVA: 0x00002275 File Offset: 0x00000475
		protected override Job TryGiveJob(Pawn pawn)
		{
			if (pawn.def != ThingDefOfAvali.Avali || Find.TickManager.TicksGame < pawn.mindState.canLovinTick)
			{
				return null;
			}
			return new Job(JobDefOfAvali.FindPartnerForLovinAvali);
		}
	}
}

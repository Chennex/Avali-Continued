using System;
using Verse.AI;

namespace Avali
{
	// Token: 0x02000056 RID: 86
	public class MentalState_RaceDependant : MentalState
	{
		// Token: 0x060001CF RID: 463 RVA: 0x00002E2E File Offset: 0x0000102E
		public override void PreStart()
		{
			if (this.pawn.def != ThingDefOfAvali.Avali)
			{
				this.SkipMentalState();
			}
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x0000F7D4 File Offset: 0x0000D9D4
		public void SkipMentalState()
		{
			if (!this.pawn.Dead)
			{
				this.pawn.mindState.mentalStateHandler.Reset();
			}
			if (this.pawn.Spawned)
			{
				this.pawn.jobs.EndCurrentJob(JobCondition.InterruptForced, true, true);
			}
		}
	}
}

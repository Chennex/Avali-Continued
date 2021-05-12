using System;
using System.Collections.Generic;
using Verse.AI;

namespace Avali
{
	// Token: 0x02000076 RID: 118
	public class JobDriver_GotoLayAvaliEgg : JobDriver
	{
		// Token: 0x06000254 RID: 596 RVA: 0x00002187 File Offset: 0x00000387
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		// Token: 0x06000255 RID: 597 RVA: 0x00012840 File Offset: 0x00010A40
		protected override IEnumerable<Toil> MakeNewToils()
		{
			if (base.TargetB.IsValid)
			{
				yield return Toils_Goto.GotoThing(this.bedInd, PathEndMode.OnCell);
				yield return Toils_General.Do(delegate
				{
					this.layEgg = new Job(JobDefOfAvali.LayAvaliEgg, base.TargetA, base.TargetB);
				});
			}
			else
			{
				this.layEgg = new Job(JobDefOfAvali.LayAvaliEgg, base.TargetA);
			}
			yield return Toils_General.Do(delegate
			{
				this.pawn.jobs.StartJob(this.layEgg, JobCondition.InterruptForced, null, false, true, null, null, false, false);
			});
			yield break;
		}

		// Token: 0x04000212 RID: 530
		private TargetIndex bedInd = TargetIndex.B;

		// Token: 0x04000213 RID: 531
		private Job layEgg;
	}
}

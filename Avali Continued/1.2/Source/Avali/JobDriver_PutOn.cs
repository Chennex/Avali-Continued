using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x02000079 RID: 121
	public class JobDriver_PutOn : JobDriver
	{
		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000263 RID: 611 RVA: 0x00003207 File Offset: 0x00001407
		private Apparel apparel
		{
			get
			{
				return (Apparel)((Thing)this.job.GetTarget(this.apparelInd));
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000264 RID: 612 RVA: 0x00003224 File Offset: 0x00001424
		private Pawn anotherPawn
		{
			get
			{
				return (Pawn)((Thing)this.job.GetTarget(this.anotherPawnInd));
			}
		}

		// Token: 0x06000265 RID: 613 RVA: 0x00012A34 File Offset: 0x00010C34
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.apparel, this.job, 1, -1, null, true) && this.pawn.Reserve(this.anotherPawn, this.job, 1, -1, null, true);
		}

		// Token: 0x06000266 RID: 614 RVA: 0x00003241 File Offset: 0x00001441
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDestroyedOrNull(this.apparelInd);
			this.FailOnDespawnedOrNull(this.anotherPawnInd);
			this.FailOnBurningImmobile(this.anotherPawnInd);
			yield return Toils_Goto.GotoThing(this.apparelInd, PathEndMode.ClosestTouch).FailOnSomeonePhysicallyInteracting(this.apparelInd);
			yield return Toils_Misc.SetForbidden(this.apparelInd, false);
			yield return Toils_Haul.StartCarryThing(this.apparelInd, false, false, false);
			yield return Toils_Goto.GotoThing(this.anotherPawnInd, PathEndMode.OnCell).FailOnAggroMentalState(this.anotherPawnInd);
			Toil toil = Toils_General.Wait(60, this.anotherPawnInd).FailOnDespawnedOrNull(this.anotherPawnInd).WithProgressBarToilDelay(this.anotherPawnInd, true, -0.5f);
			toil.initAction = delegate()
			{
				if (this.anotherPawn.Awake() && !this.anotherPawn.InBed())
				{
					Job newJob = new Job(JobDefOfAvali.PutOn);
					this.anotherPawn.jobs.StartJob(newJob, JobCondition.InterruptForced, null, false, true, null, null, false, false);
				}
			};
			yield return toil;
			Toil toil2 = Toils_Haul.PlaceHauledThingInCell(this.pawnCellInd, null, false, false);
			toil2.AddFinishAction(delegate
			{
				this.anotherPawn.apparel.Wear(this.apparel, true, false);
				this.anotherPawn.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
			});
			yield return toil2;
			yield break;
		}

		// Token: 0x04000219 RID: 537
		private TargetIndex apparelInd = TargetIndex.A;

		// Token: 0x0400021A RID: 538
		private TargetIndex anotherPawnInd = TargetIndex.B;

		// Token: 0x0400021B RID: 539
		private TargetIndex pawnCellInd = TargetIndex.C;
	}
}

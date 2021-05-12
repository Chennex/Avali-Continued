using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x02000046 RID: 70
	public class JobDriver_CheckAvaliEgg : JobDriver
	{
		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000178 RID: 376 RVA: 0x00002BA2 File Offset: 0x00000DA2
		private Thing egg
		{
			get
			{
				return (Thing)this.job.GetTarget(this.eggInd);
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000179 RID: 377 RVA: 0x00002BBA File Offset: 0x00000DBA
		private Building bed
		{
			get
			{
				return (Building)((Thing)this.job.GetTarget(this.bedInd));
			}
		}

		// Token: 0x0600017A RID: 378 RVA: 0x0000DEDC File Offset: 0x0000C0DC
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.egg, this.job, 1, -1, null, true) && this.pawn.Reserve(this.bed, this.job, 1, -1, null, true);
		}

		// Token: 0x0600017B RID: 379 RVA: 0x0000DF30 File Offset: 0x0000C130
		protected override IEnumerable<Toil> MakeNewToils()
		{
			bool err = false;
			Toil error = Toils_General.Do(delegate
			{
				Log.Error("Error in Toils_Haul.PlaceHauledThingInCell. Breaking job.", false);
				Log.Error("eggInd = " + this.eggInd, false);
				Log.Error("bedInd = " + this.bedInd, false);
				Log.Error("bedCellInd = " + this.bedCellInd, false);
				err = true;
			});
			this.FailOnDestroyedOrNull(this.eggInd);
			this.FailOnDespawnedOrNull(this.bedInd);
			yield return Toils_Goto.GotoThing(this.eggInd, PathEndMode.Touch).FailOnDestroyedOrNull(this.eggInd).FailOnSomeonePhysicallyInteracting(this.eggInd);
			if (!base.TargetA.Thing.IsForbidden(this.pawn) && base.TargetB.IsValid && base.TargetA.Cell != base.TargetB.Cell)
			{
				yield return Toils_Goto.GotoThing(this.eggInd, PathEndMode.Touch).FailOnDestroyedOrNull(this.eggInd).FailOnSomeonePhysicallyInteracting(this.eggInd);
				yield return Toils_Haul.StartCarryThing(this.eggInd, false, false, false);
				yield return Toils_Goto.GotoThing(this.bedInd, PathEndMode.Touch).FailOnDespawnedOrNull(this.bedInd);
				yield return Toils_Haul.PlaceHauledThingInCell(this.bedCellInd, Toils_Jump.Jump(error), false, false);
				base.TargetA.Thing.SetForbidden(true, true);
			}
			yield return Toils_General.Wait(1000, TargetIndex.None);
			if (err)
			{
				yield return error;
			}
			yield break;
		}

		// Token: 0x04000149 RID: 329
		private TargetIndex eggInd = TargetIndex.A;

		// Token: 0x0400014A RID: 330
		private TargetIndex bedInd = TargetIndex.B;

		// Token: 0x0400014B RID: 331
		private TargetIndex bedCellInd = TargetIndex.C;
	}
}

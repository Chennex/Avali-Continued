using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x02000015 RID: 21
	public class JobDriver_HackBindedThing : JobDriver
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000060 RID: 96 RVA: 0x000023AC File Offset: 0x000005AC
		private Thing thing
		{
			get
			{
				return (Thing)this.job.GetTarget(this.thingInd);
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000061 RID: 97 RVA: 0x000023C4 File Offset: 0x000005C4
		private Thing building
		{
			get
			{
				return (Thing)this.job.GetTarget(this.buildingInd);
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000062 RID: 98 RVA: 0x000023DC File Offset: 0x000005DC
		private IntVec3 buildingCell
		{
			get
			{
				return (IntVec3)this.job.GetTarget(this.buildingCellInd);
			}
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00005270 File Offset: 0x00003470
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.thing, this.job, 1, -1, null, true) && this.pawn.Reserve(this.building, this.job, 1, -1, null, true);
		}

		// Token: 0x06000064 RID: 100 RVA: 0x000023F4 File Offset: 0x000005F4
		protected override IEnumerable<Toil> MakeNewToils()
		{
			if (thing == null || building == null) pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
			
			bool err = false;
			Toil error = Toils_General.Do(delegate
			{
				Log.Error("Error in Toils_Haul.PlaceHauledThingInCell. Breaking job.");
				Log.Error("thingInd = " + thingInd);
				Log.Error("buildingInd = " + buildingInd);
				Log.Error("buildingCellInd = " + buildingCellInd);
				
				err = false;
			});
			
			CompProperties_WeaponAvali weaponAvali = thing.TryGetComp<CompRangedWeaponAvali>().Props;
			if (weaponAvali != null)
			{
				float num = 0;
				CompRangedWeaponAvali compWeaponAvali = thing.TryGetComp<CompRangedWeaponAvali>();
				if (building != null && building.GetStatValue(StatDefOf.ResearchSpeedFactor, true) > 0)
				{
					num = 1.1f * pawn.GetStatValue(StatDefOf.ResearchSpeed, true); // 1.1 * 0.58 = 0.638
					num *= building.GetStatValue(StatDefOf.ResearchSpeedFactor, true); // 0.638 * 1 = 0.638
					ticksPerWorkPoint = (int)(ticksPerWorkPoint / num); // 60 / 0.638 = 94
					if (ticksPerWorkPoint > 1 && pawn.def == ThingDefOfAvali.Avali) ticksPerWorkPoint = ticksPerWorkPoint / 2;
				}
				else if (weaponAvali.hackWorkSkill != null)
				{
					num = (pawn.skills.GetSkill(weaponAvali.hackWorkSkill).Level - weaponAvali.hackMinSkillLevel) * 2;
					
					if (ticksPerWorkPoint - num > 0)
					{
						ticksPerWorkPoint = (int)(ticksPerWorkPoint - num);
					}
					else ticksPerWorkPoint = 1;
				}
				
				//Log.Message("num = " + num);
				//Log.Message("ticksPerWorkPoint = " + ticksPerWorkPoint);
				//Log.Message("workLeft = " + compWeaponAvali.workLeft);
				
				this.FailOnDestroyedOrNull(thingInd);
        this.FailOnDespawnedOrNull(buildingInd);
				
				yield return Toils_Goto.GotoThing(thingInd, PathEndMode.Touch).FailOnDestroyedOrNull(thingInd).FailOnSomeonePhysicallyInteracting(thingInd);
				yield return Toils_Misc.SetForbidden(thingInd, false);
				yield return Toils_Haul.StartCarryThing(thingInd, false, false, false);
				yield return Toils_Goto.GotoThing(buildingInd, PathEndMode.InteractionCell).FailOnDespawnedOrNull(buildingInd);
				yield return Toils_Haul.PlaceHauledThingInCell(buildingCellInd, Toils_Jump.Jump(error), false);
				
				//IntVec3 thingPosition = thing.PositionHeld;
				//IntVec3 buildingPosition = building.PositionHeld;
				
				int workLeftInTicks = (int)(compWeaponAvali.workLeft * (ticksPerWorkPoint * 1.1f));
				Toil hack = Toils_General.Wait(workLeftInTicks, buildingInd).FailOnDespawnedNullOrForbidden(thingInd).FailOnDespawnedNullOrForbidden(buildingInd);
				hack.tickAction = delegate
				{
					//else if (thing.PositionHeld != thingPosition || building.PositionHeld != buildingPosition) pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
					//else if (thing.IsForbidden(pawn)) pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
					
					pawn.skills.Learn(weaponAvali.hackWorkSkill, 0.11f, false);
					pawn.GainComfortFromCellIfPossible();
					
					if (pawn.IsHashIntervalTick(ticksPerWorkPoint))
					{
						if (!building.IsPowered()) pawn.jobs.EndCurrentJob(JobCondition.Incompletable);
						
						//Log.Message("workLeft = " + compWeaponAvali.workLeft);
						compWeaponAvali.workLeft--;
						
						if (compWeaponAvali.workLeft <= 0)
						{
							compWeaponAvali.workLeft = 0;
							compWeaponAvali.EraseOwnerPawnInfo();
							pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
						}
					}
				};
				
				//Log.Message("Hack");
				yield return hack;
			}
			else
      {
      	Log.Warning(weaponAvali + " is not a Avali Weapon.");
      }
			
			if (err) yield return error;
			yield break;
		}

		// Token: 0x04000041 RID: 65
		public int ticksPerWorkPoint = 60;

		// Token: 0x04000042 RID: 66
		private TargetIndex thingInd = TargetIndex.A;

		// Token: 0x04000043 RID: 67
		private TargetIndex buildingInd = TargetIndex.B;

		// Token: 0x04000044 RID: 68
		private TargetIndex buildingCellInd = TargetIndex.C;
	}
}

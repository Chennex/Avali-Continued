using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x02000050 RID: 80
	public class JobDriver_TranslateText : JobDriver
	{
		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060001B9 RID: 441 RVA: 0x00002D84 File Offset: 0x00000F84
		private Thing thing
		{
			get
			{
				return (Thing)this.job.GetTarget(this.thingInd);
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060001BA RID: 442 RVA: 0x00002D9C File Offset: 0x00000F9C
		private Thing building
		{
			get
			{
				return (Thing)this.job.GetTarget(this.buildingInd);
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060001BB RID: 443 RVA: 0x00002DB4 File Offset: 0x00000FB4
		private IntVec3 buildingCell
		{
			get
			{
				return (IntVec3)this.job.GetTarget(this.buildingCellInd);
			}
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000EDAC File Offset: 0x0000CFAC
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			if (this.building != null)
			{
				return this.pawn.Reserve(this.thing, this.job, 1, -1, null, true) && this.pawn.Reserve(this.building, this.job, 1, -1, null, true);
			}
			return this.pawn.Reserve(this.thing, this.job, 1, -1, null, true);
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000EE28 File Offset: 0x0000D028
		protected override IEnumerable<Toil> MakeNewToils()
		{
			if (this.thing == null)
			{
				this.pawn.jobs.EndCurrentJob(JobCondition.InterruptForced, true, true);
			}
			bool err = false;
			Toil error = Toils_General.Do(delegate
			{
				Log.Error("Error in Toils_Haul.PlaceHauledThingInCell. Breaking job.", false);
				Log.Error("thing = " + this.thing, false);
				Log.Error("building = " + this.building, false);
				Log.Error("buildingCell = " + this.buildingCell, false);
				err = true;
			});
			CompProperties_TextThing textThing = this.thing.TryGetComp<CompTextThing>().Props;
			if (textThing != null)
			{
				float num = 0f;
				CompTextThing compTextThing = this.thing.TryGetComp<CompTextThing>();
				if (this.building != null && this.building.GetStatValue(StatDefOf.ResearchSpeedFactor, true) > 0f)
				{
					num = 1.1f * this.pawn.GetStatValue(StatDefOf.ResearchSpeed, true);
					num *= this.building.GetStatValue(StatDefOf.ResearchSpeedFactor, true);
					this.ticksPerWorkPoint /= num;
				}
				else if (textThing.workSkill != null)
				{
					num = (float)((this.pawn.skills.GetSkill(textThing.workSkill).Level - textThing.minSkillLevel) * 2);
					if (this.ticksPerWorkPoint - num > 0f)
					{
						this.ticksPerWorkPoint -= num;
					}
					else
					{
						this.ticksPerWorkPoint = 1f;
					}
				}
				this.FailOnForbidden(this.thingInd);
				if (this.building != null)
				{
					this.FailOnDespawnedOrNull(this.buildingInd);
					this.FailOnForbidden(this.buildingInd);
				}
				yield return Toils_Goto.GotoThing(this.thingInd, PathEndMode.Touch).FailOnDespawnedNullOrForbidden(this.thingInd).FailOnSomeonePhysicallyInteracting(this.thingInd);
				yield return Toils_Misc.SetForbidden(this.thingInd, false);
				yield return Toils_Haul.StartCarryThing(this.thingInd, false, false, false);
				if (this.building != null)
				{
					if (this.buildingCell != new IntVec3(-1000, -1000, -1000))
					{
						yield return Toils_Goto.GotoThing(this.buildingInd, PathEndMode.InteractionCell).FailOnDespawnedOrNull(this.buildingInd);
					}
					else
					{
						yield return Toils_Goto.GotoThing(this.buildingInd, PathEndMode.OnCell).FailOnDespawnedOrNull(this.buildingInd);
					}
				}
				if (this.buildingCell != new IntVec3(-1000, -1000, -1000))
				{
					yield return Toils_Haul.PlaceHauledThingInCell(this.buildingCellInd, Toils_Jump.Jump(error), false, false);
				}
				float workLeftInTicks = (float)compTextThing.workLeft * (this.ticksPerWorkPoint * 1.1f);
				Toil translate = Toils_General.Wait((int)workLeftInTicks, TargetIndex.None).FailOnDespawnedNullOrForbidden(this.thingInd).FailOnDespawnedNullOrForbidden(this.buildingInd);
				translate.tickAction = delegate()
				{
					if (textThing.workSkill != null)
					{
						this.pawn.skills.Learn(textThing.workSkill, 0.11f, false);
					}
					this.pawn.GainComfortFromCellIfPossible(false);
					if (this.pawn.IsHashIntervalTick((int)this.ticksPerWorkPoint))
					{
						compTextThing.workLeft--;
						if (textThing.showTranslator)
						{
							if (compTextThing.translator == "")
							{
								compTextThing.translator += this.pawn.Name;
							}
							else if (!compTextThing.translator.Contains(this.pawn.ToString()))
							{
								CompTextThing compTextThing2 = compTextThing;
								compTextThing2.translator += this.pawn.Name.ToString();
								CompTextThing compTextThing3 = compTextThing;
								compTextThing3.translator = compTextThing3.translator + ", " + this.pawn.Name;
							}
						}
					}
					if (compTextThing.workLeft <= 0)
					{
						compTextThing.workLeft = 0;
						this.thing.def.BaseMarketValue = textThing.translatedMarketValue;
						if (textThing.taleWhenTranslated != null)
						{
							TaleRecorder.RecordTale(textThing.taleWhenTranslated, new object[]
							{
								this.pawn,
								this.thing.def
							});
						}
						if (textThing.thoughtWhenTranslated != null)
						{
							Thought_Memory newThought = (Thought_Memory)ThoughtMaker.MakeThought(textThing.thoughtWhenTranslated);
							this.pawn.needs.mood.thoughts.memories.TryGainMemory(newThought, null);
						}
						this.pawn.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
					}
				};
				yield return translate;
			}
			if (err)
			{
				yield return error;
			}
			yield break;
		}

		// Token: 0x0400016C RID: 364
		public float ticksPerWorkPoint = 60f;

		// Token: 0x0400016D RID: 365
		private TargetIndex thingInd = TargetIndex.A;

		// Token: 0x0400016E RID: 366
		private TargetIndex buildingInd = TargetIndex.B;

		// Token: 0x0400016F RID: 367
		private TargetIndex buildingCellInd = TargetIndex.C;
	}
}

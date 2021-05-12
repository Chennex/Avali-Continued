using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x02000072 RID: 114
	public class JobDriver_LovinAvaliSingle : JobDriver
	{
		// Token: 0x17000058 RID: 88
		// (get) Token: 0x0600023B RID: 571 RVA: 0x000030CD File Offset: 0x000012CD
		private Pawn partner
		{
			get
			{
				return (Pawn)((Thing)this.job.GetTarget(this.partnerInd));
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x0600023C RID: 572 RVA: 0x000030EA File Offset: 0x000012EA
		private Building_Bed bed
		{
			get
			{
				return (Building_Bed)((Thing)this.job.GetTarget(this.bedInd));
			}
		}

		// Token: 0x0600023D RID: 573 RVA: 0x00003107 File Offset: 0x00001307
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.ticksLeft, "ticksLeft", 0, false);
		}

		// Token: 0x0600023E RID: 574 RVA: 0x00003121 File Offset: 0x00001321
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.partner, this.job, 1, -1, null, true);
		}

		// Token: 0x0600023F RID: 575 RVA: 0x000120C0 File Offset: 0x000102C0
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedOrNull(this.partnerInd);
			this.FailOn(() => !this.partner.health.capacities.CanBeAwake);
			if (this.pawn.CanReserveAndReach(this.bed, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, false))
			{
				Job newJob = new Job(JobDefOf.Goto, this.bed);
				this.partner.jobs.StartJob(newJob, JobCondition.InterruptForced, null, false, true, null, null, false, false);
				yield return Toils_Goto.GotoThing(this.bedInd, PathEndMode.ClosestTouch);
			}
			else
			{
				yield return Toils_Goto.GotoThing(this.partnerInd, PathEndMode.ClosestTouch);
			}
			yield return new Toil
			{
				initAction = delegate()
				{
					Job newJob2 = new Job(JobDefOfAvali.LovinAvaliPartner, this.partner);
					this.partner.jobs.StartJob(newJob2, JobCondition.InterruptForced, null, false, true, null, null, false, false);
					this.ticksLeft = (int)(2500f * Mathf.Clamp(Rand.Range(0.1f, 1.1f), 0.1f, 2f));
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
			Toil doLovinAvali = Toils_General.Wait(this.ticksLeft, TargetIndex.None);
			doLovinAvali.tickAction = delegate()
			{
				this.partner.rotationTracker.Face(this.pawn.DrawPos);
				if (this.partner.health.Dead || this.partner.CurJob.def != JobDefOfAvali.LovinAvaliPartner)
				{
					this.pawn.jobs.EndCurrentJob(JobCondition.InterruptForced, true, true);
				}
				this.ticksLeft--;
				if (this.ticksLeft <= 0)
				{
					base.ReadyForNextToil();
					return;
				}
				if (this.pawn.IsHashIntervalTick(this.TicksBetweenHeartMotes))
				{
					MoteMaker.ThrowMetaIcon(this.pawn.Position, this.pawn.Map, ThingDefOf.Mote_Heart);
				}
				if (this.partner.IsHashIntervalTick(this.TicksBetweenHeartMotes))
				{
					MoteMaker.ThrowMetaIcon(this.partner.Position, this.pawn.Map, ThingDefOf.Mote_Heart);
				}
			};
			doLovinAvali.AddFinishAction(delegate
			{
				this.partner.jobs.EndCurrentJob(JobCondition.InterruptForced, true, true);
				Thought_Memory newThought = (Thought_Memory)ThoughtMaker.MakeThought(ThoughtDefOf.GotSomeLovin);
				this.pawn.needs.mood.thoughts.memories.TryGainMemory(newThought, this.partner);
				this.partner.needs.mood.thoughts.memories.TryGainMemory(newThought, this.pawn);
				this.pawn.mindState.canLovinTick = Find.TickManager.TicksGame + this.GenerateRandomMinTicksToNextLovin(this.pawn);
				this.partner.mindState.canLovinTick = Find.TickManager.TicksGame + this.GenerateRandomMinTicksToNextLovin(this.partner);
				if (!(this.pawn.def.defName != this.partner.def.defName) && this.pawn.ageTracker.CurLifeStage.reproductive && this.partner.ageTracker.CurLifeStage.reproductive)
				{
					this.pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOfAvali.AvaliHasEgg, true);
				}
			});
			doLovinAvali.socialMode = RandomSocialMode.Off;
			yield return doLovinAvali;
			yield break;
		}

		// Token: 0x06000240 RID: 576 RVA: 0x00003DF8 File Offset: 0x00001FF8
		private void AddHasEggHediff(Pawn male, Pawn female)
		{
			Hediff_AvaliHasEgg hediff_AvaliHasEgg = (Hediff_AvaliHasEgg)HediffMaker.MakeHediff(HediffDefOfAvali.AvaliHasEgg, female, null);
			hediff_AvaliHasEgg.father = male;
			female.health.AddHediff(hediff_AvaliHasEgg, null, null, null);
		}

		// Token: 0x06000241 RID: 577 RVA: 0x000120E0 File Offset: 0x000102E0
		private int GenerateRandomMinTicksToNextLovin(Pawn pawn)
		{
			if (DebugSettings.alwaysDoLovin)
			{
				return 100;
			}
			float num = JobDriver_LovinAvaliSingle.LovinIntervalHoursFromAgeCurve.Evaluate(pawn.ageTracker.AgeBiologicalYearsFloat);
			num = Rand.Gaussian(num, 0.3f);
			if (num < 0.5f)
			{
				num = 0.5f;
			}
			return (int)(num * 10000f);
		}

		// Token: 0x04000205 RID: 517
		public int TicksBetweenHeartMotes = 100;

		// Token: 0x04000206 RID: 518
		private int ticksLeft = 9999999;

		// Token: 0x04000207 RID: 519
		private TargetIndex partnerInd = TargetIndex.A;

		// Token: 0x04000208 RID: 520
		private TargetIndex bedInd = TargetIndex.B;

		// Token: 0x04000209 RID: 521
		private static readonly SimpleCurve LovinIntervalHoursFromAgeCurve = new SimpleCurve
		{
			{
				new CurvePoint(13f, 1.5f),
				true
			},
			{
				new CurvePoint(22f, 1.5f),
				true
			},
			{
				new CurvePoint(30f, 4f),
				true
			},
			{
				new CurvePoint(50f, 12f),
				true
			},
			{
				new CurvePoint(75f, 36f),
				true
			},
			{
				new CurvePoint(90f, 108f),
				true
			},
			{
				new CurvePoint(115f, 324f),
				true
			}
		};
	}
}

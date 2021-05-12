using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x0200000C RID: 12
	public class JobDriver_LovinAvali : JobDriver
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000034 RID: 52 RVA: 0x000021BC File Offset: 0x000003BC
		private Pawn partner
		{
			get
			{
				return (Pawn)((Thing)this.job.GetTarget(this.partnerInd));
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000035 RID: 53 RVA: 0x000021D9 File Offset: 0x000003D9
		private Building bed
		{
			get
			{
				return (Building)((Thing)this.job.GetTarget(this.bedInd));
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000021F6 File Offset: 0x000003F6
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.ticksLeft, "ticksLeft", 0, false);
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003D80 File Offset: 0x00001F80
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			if (this.bed != null)
			{
				this.pawn.Reserve(this.bed, this.job, 1, -1, null, true);
			}
			return this.pawn.Reserve(this.partner, this.job, 1, -1, null, true);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00003DD8 File Offset: 0x00001FD8
		protected override IEnumerable<Toil> MakeNewToils()
		{
			base.ReportStringProcessed("Moving".Translate());
			this.FailOnDespawnedOrNull(this.partnerInd);
			this.FailOn(() => !this.partner.health.capacities.CanBeAwake);
			yield return Toils_Goto.GotoThing(this.partnerInd, PathEndMode.ClosestTouch);
			Room room = this.pawn.GetRoom(RegionType.Set_Passable);
			if (this.pawn.FindPawnsForLovinInRoom(room, false) != null)
			{
				MoteMaker.ThrowMetaIcon(this.pawn.Position, this.pawn.Map, ThingDefOfAvali.Mote_HeartSpeech);
				Toil wait = Toils_General.WaitWith(this.partnerInd, 200, false, true);
				wait.socialMode = RandomSocialMode.Off;
				yield return wait;
				MoteMaker.ThrowMetaIcon(this.partner.Position, this.partner.Map, ThingDefOfAvali.Mote_HeartSpeech);
				if (this.pawn.FindPawnsForLovinInRoom(room, false) != null)
				{
					if (this.bed != null && this.pawn.Position != this.bed.Position && this.partner.Position != this.bed.Position && this.pawn.CanReserveAndReach(this.bed, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, false) && this.partner.CanReserveAndReach(this.bed, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, false))
					{
						Job newJob = new Job(JobDefOf.Goto, this.bed);
						this.partner.jobs.StartJob(newJob, JobCondition.InterruptForced, null, false, true, null, null, false, false);
						yield return Toils_Goto.GotoThing(this.bedInd, PathEndMode.ClosestTouch);
						MoteMaker.ThrowMetaIcon(this.pawn.Position, this.partner.Map, ThingDefOfAvali.Mote_HeartSpeech);
						MoteMaker.ThrowMetaIcon(this.partner.Position, this.partner.Map, ThingDefOfAvali.Mote_HeartSpeech);
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
						this.pawn.GainComfortFromCellIfPossible(false);
						this.partner.GainComfortFromCellIfPossible(false);
						this.partner.rotationTracker.Face(this.pawn.DrawPos);
						if (this.partner.health.Dead || this.partner.CurJob.def != JobDefOfAvali.LovinAvaliPartner)
						{
							this.pawn.jobs.EndCurrentJob(JobCondition.InterruptForced, true, true);
						}
						this.ticksLeft--;
						if (this.ticksLeft <= 0)
						{
							this.ReadyForNextToil();
							JoyUtility.JoyTickCheckEnd(this.pawn, JoyTickFullJoyAction.None, 1f, null);
							return;
						}
						JoyUtility.JoyTickCheckEnd(this.pawn, JoyTickFullJoyAction.EndJob, 1f, null);
						if (this.pawn.IsHashIntervalTick(100))
						{
							if (this.pawn.FindPawnsForLovinInRoom(room, true) == null)
							{
								this.partner.jobs.EndCurrentJob(JobCondition.InterruptForced, true, true);
								this.EndJobWith(JobCondition.InterruptForced);
							}
							MoteMaker.ThrowMetaIcon(this.pawn.Position, this.pawn.Map, ThingDefOf.Mote_Heart);
						}
						if (this.partner.IsHashIntervalTick(100))
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
					base.ReportStringProcessed("Lovin".Translate());
					yield return doLovinAvali;
				}
			}
			yield break;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003DF8 File Offset: 0x00001FF8
		private void AddHasEggHediff(Pawn male, Pawn female)
		{
			Hediff_AvaliHasEgg hediff_AvaliHasEgg = (Hediff_AvaliHasEgg)HediffMaker.MakeHediff(HediffDefOfAvali.AvaliHasEgg, female, null);
			hediff_AvaliHasEgg.father = male;
			female.health.AddHediff(hediff_AvaliHasEgg, null, null, null);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003E38 File Offset: 0x00002038
		private int GenerateRandomMinTicksToNextLovin(Pawn pawn)
		{
			if (DebugSettings.alwaysDoLovin)
			{
				return 100;
			}
			float num = JobDriver_LovinAvali.LovinIntervalHoursFromAgeCurve.Evaluate(pawn.ageTracker.AgeBiologicalYearsFloat);
			num = Rand.Gaussian(num, 0.3f);
			if (num < 0.5f)
			{
				num = 0.5f;
			}
			return (int)(num * 10000f);
		}

		// Token: 0x04000020 RID: 32
		public const int TicksBetweenHeartMotes = 100;

		// Token: 0x04000021 RID: 33
		public const float joyPerTick = 1f;

		// Token: 0x04000022 RID: 34
		private int ticksLeft = 9999999;

		// Token: 0x04000023 RID: 35
		private TargetIndex partnerInd = TargetIndex.A;

		// Token: 0x04000024 RID: 36
		private TargetIndex bedInd = TargetIndex.B;

		// Token: 0x04000025 RID: 37
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

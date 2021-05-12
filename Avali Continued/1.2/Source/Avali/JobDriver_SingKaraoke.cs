using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x0200004B RID: 75
	public class JobDriver_SingKaraoke : JobDriver
	{
		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000195 RID: 405 RVA: 0x00002CA0 File Offset: 0x00000EA0
		private Thing thing
		{
			get
			{
				return (Thing)this.job.GetTarget(this.thingInd);
			}
		}

		// Token: 0x06000196 RID: 406 RVA: 0x0000E4C8 File Offset: 0x0000C6C8
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.job.targetA, this.job, this.job.def.joyMaxParticipants, 0, null, true) && this.pawn.Reserve(this.job.targetB, this.job, 1, -1, null, true);
		}

		// Token: 0x06000197 RID: 407 RVA: 0x0000E528 File Offset: 0x0000C728
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.EndOnDespawnedOrNull(this.thingInd, JobCondition.Incompletable);
			this.FailOnForbidden(this.thingInd);
			yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
			CompPowerTrader compPowerTrader = this.thing.TryGetComp<CompPowerTrader>();
			float statValue = base.TargetThingA.GetStatValue(StatDefOf.JoyGainFactor, true);
			Toil sing = new Toil();
			sing.tickAction = delegate()
			{
				this.pawn.rotationTracker.FaceCell(this.TargetA.Cell);
				if (compPowerTrader != null && (compPowerTrader.PowerNet.CurrentStoredEnergy() <= 0f || this.thing.IsBrokenDown() || !FlickUtility.WantsToBeOn(this.thing)))
				{
					this.pawn.jobs.EndCurrentJob(JobCondition.InterruptForced, true, true);
				}
				JoyUtility.JoyTickCheckEnd(this.pawn, JoyTickFullJoyAction.EndJob, statValue, null);
				if (this.pawn.IsHashIntervalTick(100))
				{
					MoteMaker.ThrowMetaIcon(this.pawn.Position, this.pawn.Map, ThingDefOfAvali.Mote_Note);
					Room room = this.pawn.GetRoom(RegionType.Set_Passable);
					if (room != null)
					{
						int num = 0;
						SkillRecord skill = this.pawn.skills.GetSkill(SkillDefOf.Artistic);
						if (!skill.TotallyDisabled)
						{
							float num2 = (float)this.GetSingThoughtState(skill.Level) * this.pawn.health.capacities.GetLevel(PawnCapacityDefOf.Talking) * this.pawn.health.capacities.GetLevel(PawnCapacityDefOf.Hearing);
							num = (int)num2;
							if (num > 4)
							{
								num = 4;
							}
						}
						List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
						for (int i = 0; i < containedAndAdjacentThings.Count; i++)
						{
							Pawn pawn = containedAndAdjacentThings[i] as Pawn;
							if (pawn != null && pawn != this.pawn && pawn.RaceProps.Humanlike)
							{
								if (this.pawn.health.hediffSet.HasHediff(HediffDefOfAvali.AvaliBiology, false))
								{
									if (pawn.health.hediffSet.HasHediff(HediffDefOfAvali.AvaliBiology, false))
									{
										if (num < 2)
										{
											num = 2;
										}
										num = this.CheckSingerTraits(num);
										pawn.GiveThoughtWithStage(ThoughtDefOfAvali.ListenerAvali, num, true);
									}
									else
									{
										if (num < 1)
										{
											num = 1;
										}
										num = this.CheckSingerTraits(num);
										pawn.GiveThoughtWithStage(ThoughtDefOfAvali.ListenerAny, num, true);
									}
								}
								else
								{
									num = this.CheckSingerTraits(num);
									if (pawn.health.hediffSet.HasHediff(HediffDefOfAvali.AvaliBiology, false))
									{
										pawn.GiveThoughtWithStage(ThoughtDefOfAvali.ListenerAvali, num, true);
									}
									else
									{
										pawn.GiveThoughtWithStage(ThoughtDefOfAvali.ListenerAny, num, true);
									}
								}
							}
						}
					}
				}
			};
			sing.handlingFacing = true;
			sing.socialMode = RandomSocialMode.Off;
			sing.defaultCompleteMode = ToilCompleteMode.Delay;
			sing.defaultDuration = this.job.def.joyDuration;
			sing.AddFinishAction(delegate
			{
				JoyUtility.TryGainRecRoomThought(this.pawn);
			});
			yield return sing;
			yield break;
		}

		// Token: 0x06000198 RID: 408 RVA: 0x0000E548 File Offset: 0x0000C748
		public override object[] TaleParameters()
		{
			return new object[]
			{
				this.pawn,
				base.TargetA.Thing.def
			};
		}

		// Token: 0x06000199 RID: 409 RVA: 0x00002CB8 File Offset: 0x00000EB8
		public int CheckSingerTraits(int stage)
		{
			if (this.pawn.story.traits.HasTrait(TraitDefOf.AnnoyingVoice))
			{
				return 0;
			}
			if (this.pawn.story.traits.HasTrait(TraitDefOf.CreepyBreathing))
			{
				return 0;
			}
			return stage;
		}

		// Token: 0x0600019A RID: 410 RVA: 0x00002CF7 File Offset: 0x00000EF7
		public int GetSingThoughtState(int artSkillLevel)
		{
			if (artSkillLevel <= 3)
			{
				return 0;
			}
			if (artSkillLevel <= 5)
			{
				return 1;
			}
			if (artSkillLevel <= 10)
			{
				return 2;
			}
			if (artSkillLevel <= 15)
			{
				return 3;
			}
			return 4;
		}

		// Token: 0x0400015A RID: 346
		private TargetIndex thingInd = TargetIndex.A;
	}
}

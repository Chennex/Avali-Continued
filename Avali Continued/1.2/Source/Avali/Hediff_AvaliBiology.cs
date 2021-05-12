using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x0200003C RID: 60
	public class Hediff_AvaliBiology : HediffWithComps
	{
		// Token: 0x0600013B RID: 315 RVA: 0x0000B534 File Offset: 0x00009734
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<bool>(ref this.makeStartingPack, "makeStartingPack", true, false);
			Scribe_Values.Look<bool>(ref this.immuneToPackLoss, "immuneToPackLoss", true, false);
			Scribe_Defs.Look<HediffDef>(ref this.packHediffDef, "packHediffDef");
			Scribe_References.Look<Pawn>(ref this.leader, "leader", false);
		}

		// Token: 0x0600013C RID: 316 RVA: 0x0000B58C File Offset: 0x0000978C
		public bool TryMakeStartingPack()
		{
			if (!this.makeStartingPack)
			{
				return false;
			}
			if (this.debug)
			{
				Log.Message("DaysPassed = " + GenDate.DaysPassedFloat, false);
			}
			if (this.pawn.IsPrisoner)
			{
				return false;
			}
			if (this.pawn.story.traits.HasTrait(TraitDefOf.Psychopath))
			{
				return false;
			}
			if (this.pawn.IsColonist && GenDate.DaysPassedFloat > 0.01f)
			{
				return false;
			}
			for (int i = 0; i < this.packPawns.Count; i++)
			{
				Pawn pawn = this.packPawns[i];
				if (this.pawn.HavePackRelation(pawn))
				{
					return false;
				}
			}
			List<Pawn> list = this.pawn.Map.mapPawns.FreeHumanlikesOfFaction(this.pawn.Faction).ToList<Pawn>();
			this.pawnPosInQueue = 0;
			for (int j = 0; j < list.Count; j++)
			{
				Pawn pawn2 = list[j];
				if (pawn2 != this.pawn && pawn2.def == ThingDefOfAvali.Avali && !pawn2.IsSlave() && !pawn2.story.traits.HasTrait(TraitDefOf.Psychopath) && !pawn2.HavePackRelation(this.pawn) && AvaliUtility.BothPawnsReproductiveOrNotReproductive(this.pawn, pawn2) && this.pawn.thingIDNumber > pawn2.thingIDNumber)
				{
					this.pawnPosInQueue++;
				}
			}
			if (this.debug)
			{
				Log.Message(string.Concat(new object[]
				{
					this.pawn,
					" ID: ",
					this.pawn.thingIDNumber,
					"; pawnPosInQueue = ",
					this.pawnPosInQueue
				}), false);
			}
			return false;
		}

		// Token: 0x0600013D RID: 317 RVA: 0x0000B764 File Offset: 0x00009964
		public void CheckPawnsInPack(List<Pawn> relatedPawns)
		{
			this.pawnsInPack = 1;
			List<Thought_Memory> memories = this.pawn.needs.mood.thoughts.memories.Memories;
			for (int i = 0; i < relatedPawns.Count; i++)
			{
				Pawn pawn = relatedPawns[i];
				if ((pawn.DestroyedOrNull() || pawn.Dead) && this.pawn.relations.DirectRelationExists(PawnRelationDefOfAvali.Kit, pawn))
				{
					for (int j = 0; j < memories.Count; j++)
					{
						Thought_Memory thought_Memory = memories[j];
						if (thought_Memory.def == ThoughtDefOf.PawnWithGoodOpinionDied && thought_Memory.otherPawn == pawn)
						{
							this.pawn.needs.mood.thoughts.memories.RemoveMemory(thought_Memory);
						}
					}
					this.pawn.relations.RemoveDirectRelation(PawnRelationDefOfAvali.Kit, pawn);
				}
				if (this.pawn.relations.DirectRelationExists(PawnRelationDefOfAvali.Packmate, pawn))
				{
					if (pawn.DestroyedOrNull() || pawn.Dead)
					{
						for (int k = 0; k < memories.Count; k++)
						{
							Thought_Memory thought_Memory2 = memories[k];
							if (thought_Memory2.def == ThoughtDefOf.PawnWithGoodOpinionDied && thought_Memory2.otherPawn == pawn)
							{
								this.pawn.needs.mood.thoughts.memories.RemoveMemory(thought_Memory2);
							}
						}
						this.pawn.relations.RemoveDirectRelation(PawnRelationDefOfAvali.Packmate, pawn);
						if (this.pawnsInPack > 1)
						{
							this.pawnsInPack--;
						}
					}
					else
					{
						if (this.pawn.relations.DirectRelationExists(PawnRelationDefOfAvali.PackLeader, pawn))
						{
							if (this.debug)
							{
								Log.Message(this.pawn + " removed PackLeader relation with " + pawn, false);
							}
							this.pawn.relations.RemoveDirectRelation(PawnRelationDefOfAvali.PackLeader, pawn);
						}
						this.pawnsInPack++;
					}
				}
				if (this.pawn.relations.DirectRelationExists(PawnRelationDefOfAvali.PackLeader, pawn))
				{
					if (pawn.DestroyedOrNull() || pawn.Dead)
					{
						for (int l = 0; l < memories.Count; l++)
						{
							Thought_Memory thought_Memory3 = memories[l];
							if (thought_Memory3.def == ThoughtDefOf.PawnWithGoodOpinionDied && thought_Memory3.otherPawn == pawn)
							{
								this.pawn.needs.mood.thoughts.memories.RemoveMemory(thought_Memory3);
							}
						}
						this.pawn.relations.RemoveDirectRelation(PawnRelationDefOfAvali.PackLeader, pawn);
						this.leader = null;
						if (this.pawnsInPack > 1)
						{
							this.pawnsInPack--;
						}
					}
					else if (!pawn.DestroyedOrNull() && !pawn.Dead)
					{
						this.leader = pawn;
						this.pawnsInPack++;
					}
				}
			}
			Pawn pawn2 = null;
			for (int m = 0; m < relatedPawns.Count; m++)
			{
				Pawn pawn3 = relatedPawns[m];
				if (!this.pawn.DestroyedOrNull() && !pawn3.Dead && this.pawn.HavePackRelation(pawn3))
				{
					int totalSkillLevel = pawn3.GetTotalSkillLevel();
					if (totalSkillLevel > this.pawnTotalSkillCount)
					{
						this.pawnTotalSkillCount = totalSkillLevel;
						pawn2 = pawn3;
					}
				}
			}
			if (pawn2 != null && pawn2 != this.leader)
			{
				if (this.leader != null)
				{
					this.pawn.TryAddDirectRelation(this.leader, PawnRelationDefOfAvali.Packmate);
					this.pawn.relations.TryRemoveDirectRelation(PawnRelationDefOfAvali.PackLeader, this.leader);
				}
				this.pawn.TryAddDirectRelation(pawn2, PawnRelationDefOfAvali.PackLeader);
				this.pawn.relations.TryRemoveDirectRelation(PawnRelationDefOfAvali.Packmate, pawn2);
				this.leader = pawn2;
			}
		}

		// Token: 0x0600013E RID: 318 RVA: 0x0000BB1C File Offset: 0x00009D1C
		public void UpdatePackHediff(List<Pawn> relatedPawns)
		{
			if (relatedPawns.Count == 0 || relatedPawns == null)
			{
				return;
			}
			SkillRecord highestPackSkill = this.pawn.GetHighestPackSkill(relatedPawns);
			if (this.debug)
			{
				Log.Message(this.pawn + "'s pack highestPackSkill = " + highestPackSkill, false);
				Log.Message(this.pawn + "'s pack size = " + relatedPawns.Count, false);
			}
			if (highestPackSkill == null)
			{
				return;
			}
			if (highestPackSkill.Level < 10)
			{
				this.packHediffDef = this.pawn.TryRemovePackHediffsAndAddPackHediff(HediffDefOfAvali.AvaliPackExploration);
			}
			else if (highestPackSkill.def == SkillDefOf.Melee || highestPackSkill.def == SkillDefOf.Shooting)
			{
				this.packHediffDef = this.pawn.TryRemovePackHediffsAndAddPackHediff(HediffDefOfAvali.AvaliPackMilitary);
			}
			else if (highestPackSkill.def == SkillDefOf.Intellectual || highestPackSkill.def == SkillDefOf.Medicine)
			{
				this.packHediffDef = this.pawn.TryRemovePackHediffsAndAddPackHediff(HediffDefOfAvali.AvaliPackScientific);
			}
			else if (highestPackSkill.def == SkillDefOf.Crafting || highestPackSkill.def == SkillDefOf.Mining || highestPackSkill.def == SkillDefOf.Plants)
			{
				this.packHediffDef = this.pawn.TryRemovePackHediffsAndAddPackHediff(HediffDefOfAvali.AvaliPackIndustrial);
			}
			else if (highestPackSkill.def == SkillDefOf.Artistic || highestPackSkill.def == SkillDefOf.Cooking)
			{
				this.packHediffDef = this.pawn.TryRemovePackHediffsAndAddPackHediff(HediffDefOfAvali.AvaliPackArtistical);
			}
			else
			{
				this.packHediffDef = this.pawn.TryRemovePackHediffsAndAddPackHediff(HediffDefOfAvali.AvaliPackExploration);
			}
			if (this.debug)
			{
				Log.Message(this.pawn + " packHediffDef = " + this.packHediffDef, false);
			}
		}

		// Token: 0x0600013F RID: 319 RVA: 0x0000BCC0 File Offset: 0x00009EC0
		public void UpdatePackHediffState(HediffDef packHediffDef, List<Pawn> packPawns)
		{
			if (packHediffDef == null || packPawns.Count == 0)
			{
				return;
			}
			int num = 0;
			float num2 = 30f * this.pawn.health.capacities.GetLevel(PawnCapacityDefOf.Hearing);
			Room room = this.pawn.GetRoom(RegionType.Set_Passable);
			if (room == null || num2 < 1f)
			{
				return;
			}
			for (int i = 0; i < packPawns.Count; i++)
			{
				Pawn pawn = packPawns[i];
				if (pawn.def == ThingDefOfAvali.Avali && this.pawn.HavePackRelation(pawn))
				{
					float num3 = num2;
					if (room != pawn.GetRoom(RegionType.Set_Passable))
					{
						num3 /= 2f;
					}
					IntVec3 position = this.pawn.Position;
					IntVec3 position2 = pawn.Position;
					double num4 = Math.Sqrt(Math.Pow((double)(position2.x - position.x), 2.0) + Math.Pow((double)(position2.y - position.y), 2.0));
					if (num4 <= (double)num3)
					{
						num++;
					}
					if (num == this.maxPawnsInPack)
					{
						break;
					}
				}
			}
			if (num == 0)
			{
				return;
			}
			Hediff firstHediffOfDef = this.pawn.health.hediffSet.GetFirstHediffOfDef(packHediffDef, false);
			if (firstHediffOfDef != null && firstHediffOfDef.Severity != (float)num)
			{
				firstHediffOfDef.Severity = (float)num;
			}
		}

		// Token: 0x06000140 RID: 320 RVA: 0x0000BE14 File Offset: 0x0000A014
		public bool TryRecruitAvaliPrisoner()
		{
			if (!this.pawn.IsColonist && this.pawn.IsPrisoner && this.pawn.Awake() && this.pawn.CurJob.def == JobDefOf.PrisonerAttemptRecruit)
			{
				List<Pawn> list = this.pawn.Map.mapPawns.FreeHumanlikesOfFaction(Faction.OfPlayer).ToList<Pawn>();
				for (int i = 0; i < list.Count; i++)
				{
					Pawn pawn = list[i];
					if (this.pawn.relations.OpinionOf(pawn) >= this.opinionToRecruit)
					{
						this.pawn.SetFaction(pawn.Faction, pawn);
						Messages.Message("LetterLabelMessageRecruitSuccess".Translate().CapitalizeFirst(), this.pawn, MessageTypeDefOf.PositiveEvent, true);
						this.pawn.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000141 RID: 321 RVA: 0x0000BF18 File Offset: 0x0000A118
		public bool TryMakeImmuneToPackLoss()
		{
			if (this.pawn.story.traits.HasTrait(TraitDefOf.Psychopath) || this.pawn.health.hediffSet.HasHediff(HediffDef.Named("AvaliPackLossAugment"), false) || this.pawn.health.hediffSet.HasHediff(HediffDef.Named("CyberneticAvaliHead"), false))
			{
				this.packLossStage = -1;
				this.pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOfAvali.PackLoss);
				return true;
			}
			this.packLossStage = this.pawn.TryRemovePackLossThought(this.packPawns, this.packLossStage);
			return false;
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0000BFD0 File Offset: 0x0000A1D0
		public override void Tick()
		{
			if (this.pawn.Map == null || this.pawn.InContainerEnclosed)
			{
				return;
			}
			if (this.packPawns.Count == 0)
			{
				this.packPawns = this.pawn.relations.RelatedPawns.ToList<Pawn>();
			}
			if (this.pawn.IsHashIntervalTick(60))
			{
				this.makeStartingPack = this.TryMakeStartingPack();
				this.TryUpdatePackLossStage();
				this.UpdateResists();
				this.CheckBodyparts();
				this.UpdatePackHediffState(this.packHediffDef, this.packPawns);
				this.TryToTakeDownedOrDeadPawn(this.packPawns);
			}
			if (this.pawn.IsHashIntervalTick(2500) || this.pawnPosInQueue > -1)
			{
				if (this.pawnPosInQueue > 0)
				{
					this.pawnPosInQueue--;
					return;
				}
				if (this.debug)
				{
					Log.Message(this.pawn + " pawnPosInQueue = " + this.pawnPosInQueue, false);
				}
				this.TryToTakeDownedOrDeadPawn(this.packPawns);
				this.packPawns = this.pawn.relations.RelatedPawns.ToList<Pawn>();
				this.UpdateRaceSpecificThoughts();
				this.immuneToPackLoss = this.TryMakeImmuneToPackLoss();
				if (!this.pawn.Awake() || this.pawn.IsSlave())
				{
					if (this.pawnPosInQueue == 0)
					{
						this.pawnPosInQueue = -1;
					}
					return;
				}
				if (this.TryRecruitAvaliPrisoner())
				{
					return;
				}
				this.pawnTotalSkillCount = this.pawn.GetTotalSkillLevel();
				this.CheckPawnsInPack(this.packPawns);
				int num = this.pawnsInPack;
				this.TryMakePack();
				if (num < this.pawnsInPack)
				{
					this.MakePawnKnowHisPackmates();
					this.CheckPawnsInPack(this.packPawns);
				}
				this.UpdatePackHediff(this.packPawns);
				if (this.pawnPosInQueue == 0)
				{
					this.pawnPosInQueue = -1;
				}
			}
		}

		// Token: 0x06000143 RID: 323 RVA: 0x0000C198 File Offset: 0x0000A398
		public void TryMakePack()
		{
			if (this.pawnsInPack <= this.maxPawnsInPack)
			{
				List<Pawn> list = this.pawn.Map.mapPawns.FreeHumanlikesOfFaction(this.pawn.Faction).ToList<Pawn>();
				for (int i = 0; i < list.Count; i++)
				{
					Pawn pawn = list[i];
					if (pawn != this.pawn && pawn.Awake() && !pawn.IsSlave() && !this.pawn.HavePackRelation(pawn) && AvaliUtility.BothPawnsReproductiveOrNotReproductive(this.pawn, pawn) && (this.pawnPosInQueue == 0 || (this.pawn.relations.OpinionOf(pawn) >= this.pawnOpinionNeeded && pawn.relations.OpinionOf(this.pawn) >= this.pawnOpinionNeeded)) && Hediff_AvaliBiology.TotalPawnsInPack(pawn) < this.maxPawnsInPack && pawn.def.race == ThingDefOfAvali.Avali.race && this.pawnsInPack <= this.maxPawnsInPack)
					{
						if (this.debug)
						{
							Log.Message(this.pawn + " try make pack relation with " + pawn, false);
						}
						this.pawn.TryAddDirectRelation(pawn, PawnRelationDefOfAvali.Packmate);
						if (pawn.def == ThingDefOfAvali.Avali)
						{
							pawn.TryAddDirectRelation(this.pawn, PawnRelationDefOfAvali.Packmate);
						}
						this.pawnsInPack++;
						if (this.debug)
						{
							Log.Message(this.pawn + " packSize = " + this.pawnsInPack, false);
						}
					}
				}
			}
		}

		// Token: 0x06000144 RID: 324 RVA: 0x0000C340 File Offset: 0x0000A540
		public void TryToTakeDownedOrDeadPawn(List<Pawn> relatedPawns)
		{
			if (this.pawn.CurJobDef == JobDefOfAvali.TakeDownedOrDeadPawn || this.pawn.IsColonist || this.pawn.MentalState == null || this.pawn.IsSlave())
			{
				return;
			}
			if (this.pawn.MentalStateDef == MentalStateDefOf.PanicFlee)
			{
				IntVec3 c;
				if (!RCellFinder.TryFindBestExitSpot(this.pawn, out c, TraverseMode.ByPawn))
				{
					return;
				}
				Predicate<Thing> validator = delegate(Thing t)
				{
					Pawn pawn2 = t as Pawn;
					return pawn2 != null && pawn2.RaceProps.Humanlike && pawn2.PawnListed(relatedPawns) && pawn2.Downed && this.pawn.CanReserve(pawn2, 1, -1, null, false);
				};
				Pawn pawn = (Pawn)GenClosest.ClosestThingReachable(this.pawn.Position, this.pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.ClosestTouch, TraverseParms.For(this.pawn, Danger.Deadly, TraverseMode.ByPawn, false), 20f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
				if (pawn != null)
				{
					Job newJob = new Job(JobDefOfAvali.TakeDownedOrDeadPawn)
					{
						targetA = pawn,
						targetB = c,
						count = 1
					};
					this.pawn.jobs.StartJob(newJob, JobCondition.InterruptForced, null, true, true, null, null, false, false);
					return;
				}
				validator = delegate(Thing t)
				{
					Pawn pawn2 = t as Pawn;
					return pawn2 != null && pawn2.RaceProps.Humanlike && pawn2.Faction == this.pawn.Faction && pawn2.Downed && this.pawn.CanReserve(pawn2, 1, -1, null, false);
				};
				pawn = (Pawn)GenClosest.ClosestThingReachable(this.pawn.Position, this.pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.ClosestTouch, TraverseParms.For(this.pawn, Danger.Deadly, TraverseMode.ByPawn, false), 20f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
				if (pawn != null)
				{
					Job newJob2 = new Job(JobDefOfAvali.TakeDownedOrDeadPawn)
					{
						targetA = pawn,
						targetB = c,
						count = 1
					};
					this.pawn.jobs.StartJob(newJob2, JobCondition.InterruptForced, null, true, true, null, null, false, false);
					return;
				}
				validator = delegate(Thing t)
				{
					Corpse corpse2 = t as Corpse;
					return corpse2 != null && corpse2.InnerPawn != null && corpse2.InnerPawn.RaceProps.Humanlike && corpse2.InnerPawn.PawnListed(relatedPawns) && this.pawn.CanReserve(corpse2, 1, -1, null, false);
				};
				Corpse corpse = (Corpse)GenClosest.ClosestThingReachable(this.pawn.Position, this.pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Corpse), PathEndMode.ClosestTouch, TraverseParms.For(this.pawn, Danger.Deadly, TraverseMode.ByPawn, false), 20f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
				if (corpse != null)
				{
					Job newJob3 = new Job(JobDefOfAvali.TakeDownedOrDeadPawn)
					{
						targetA = corpse,
						targetB = c,
						count = 1
					};
					this.pawn.jobs.StartJob(newJob3, JobCondition.InterruptForced, null, true, true, null, null, false, false);
				}
			}
		}

		// Token: 0x06000145 RID: 325 RVA: 0x0000C5A8 File Offset: 0x0000A7A8
		public void MakePawnKnowHisPackmates()
		{
			this.packPawns = this.pawn.relations.RelatedPawns.ToList<Pawn>();
			if (this.debug)
			{
				Log.Message(this.pawn + " try to introduce his packmates eachother.", false);
				string text = "";
				for (int i = 0; i < this.packPawns.Count; i++)
				{
					if (text == "")
					{
						text += this.packPawns[i];
					}
					else
					{
						text = text + ", " + this.packPawns[i];
					}
				}
				Log.Message(this.pawn + " relatedPawns = " + text, false);
			}
			for (int j = 0; j < this.packPawns.Count; j++)
			{
				Pawn pawn = this.packPawns[j];
				if (!pawn.DestroyedOrNull() && !pawn.Dead && this.pawn.relations.DirectRelationExists(PawnRelationDefOfAvali.Packmate, pawn) && !this.pawn.relations.DirectRelationExists(PawnRelationDefOfAvali.PackLeader, pawn))
				{
					for (int k = 0; k < this.packPawns.Count; k++)
					{
						Pawn pawn2 = this.packPawns[k];
						if (!pawn2.DestroyedOrNull() && !pawn2.Dead && pawn2 != pawn && !pawn.relations.DirectRelationExists(PawnRelationDefOfAvali.Packmate, pawn2) && !pawn.relations.DirectRelationExists(PawnRelationDefOfAvali.PackLeader, pawn2))
						{
							if (pawn.def == ThingDefOfAvali.Avali)
							{
								pawn.TryAddDirectRelation(pawn2, PawnRelationDefOfAvali.Packmate);
							}
							if (pawn2.def == ThingDefOfAvali.Avali)
							{
								pawn2.TryAddDirectRelation(pawn, PawnRelationDefOfAvali.Packmate);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000146 RID: 326 RVA: 0x0000C778 File Offset: 0x0000A978
		public static int TotalPawnsInPack(Pawn pawn2)
		{
			int num = 1;
			List<Pawn> list = pawn2.relations.RelatedPawns.ToList<Pawn>();
			for (int i = 0; i < list.Count; i++)
			{
				Pawn pawn3 = list[i];
				if (pawn2.HavePackRelation(pawn3) || pawn3.HavePackRelation(pawn2))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06000147 RID: 327 RVA: 0x00002ACF File Offset: 0x00000CCF
		public void TryUpdatePackLossStage()
		{
			if (!this.immuneToPackLoss)
			{
				this.packLossStage = this.pawn.TryAddPackLossThought(this.packPawns, this.packLossStage);
			}
		}

		// Token: 0x06000148 RID: 328 RVA: 0x0000C7C8 File Offset: 0x0000A9C8
		public void UpdateResists()
		{
			float num = 0f;
			List<Apparel> wornApparel = this.pawn.apparel.WornApparel;
			num += this.pawn.GetStatValue(StatDef.Named("ArmorRating_Enviromental"), true);
			for (int i = 0; i < wornApparel.Count; i++)
			{
				Apparel thing = wornApparel[i];
				num += thing.GetStatValue(StatDef.Named("ArmorRating_Enviromental"), true);
			}
			this.pawn.TryMakeImmune(num, 1f, HediffDefOfAvali.ToxicImmunity);
			Hediff hediff = this.pawn.health.hediffSet.hediffs.Find((Hediff x) => x.def == HediffDefOfAvali.OxygenAtmosphere);
			if (num < this.minEnvResist)
			{
				if (this.pawn.IsColonist || this.pawn.IsPrisoner)
				{
					if (hediff != null)
					{
						hediff.Severity += 0.001f;
						return;
					}
					this.pawn.health.AddHediff(HediffDefOfAvali.OxygenAtmosphere, null, null, null);
					return;
				}
			}
			else if (hediff != null)
			{
				hediff.Severity -= 0.001f;
				if (hediff.Severity <= 0.001f)
				{
					this.pawn.health.RemoveHediff(hediff);
				}
			}
		}

		// Token: 0x06000149 RID: 329 RVA: 0x0000C918 File Offset: 0x0000AB18
		public void CheckBodyparts()
		{
			int num = 2;
			List<Hediff> hediffs = this.pawn.health.hediffSet.hediffs;
			for (int i = 0; i < hediffs.Count; i++)
			{
				Hediff hediff = hediffs[i];
				if (hediff != null && hediff.Part != null && hediff.Part.def != null && hediff.Part.def == BodyPartDefOf.Eye)
				{
					if (hediff.def == HediffDefOf.MissingBodyPart)
					{
						num--;
					}
					else if (hediff.CurStage != null && hediff.CurStage.capMods != null)
					{
						List<PawnCapacityModifier> capMods = hediff.CurStage.capMods;
						for (int j = 0; j < capMods.Count; j++)
						{
							PawnCapacityModifier pawnCapacityModifier = capMods[j];
							if (pawnCapacityModifier != null && pawnCapacityModifier.offset <= -0.5f)
							{
								num--;
							}
						}
					}
				}
				if (num <= 0)
				{
					break;
				}
			}
			if (num == 0)
			{
				this.Severity = 0.001f;
				return;
			}
			if (num == 1)
			{
				this.Severity = 0.25f;
				return;
			}
			this.Severity = 0.5f;
		}

		// Token: 0x0600014A RID: 330 RVA: 0x0000CA28 File Offset: 0x0000AC28
		public void UpdateRaceSpecificThoughts()
		{
			for (int i = 0; i < this.pawn.needs.mood.thoughts.memories.Memories.Count; i++)
			{
				Thought_Memory thought_Memory = this.pawn.needs.mood.thoughts.memories.Memories[i];
				if (thought_Memory.CurStage == null && !thought_Memory.ShouldDiscard)
				{
					this.pawn.needs.mood.thoughts.memories.RemoveMemory(thought_Memory);
				}
			}
			this.pawn.Thought_AvaliSleepingRoomImpressiveness();
			this.pawn.Thought_AvaliPackSleepingRoomRelations(this.maxPawnsInPack);
			this.pawn.Thought_SleepDisturbedAvali(25);
			this.pawn.needs.mood.thoughts.memories.ExposeData();
		}

		// Token: 0x04000118 RID: 280
		public bool debug;

		// Token: 0x04000119 RID: 281
		public int pawnOpinionNeeded = 30;

		// Token: 0x0400011A RID: 282
		public int opinionToRecruit = 25;

		// Token: 0x0400011B RID: 283
		public int maxPawnsInPack = 5;

		// Token: 0x0400011C RID: 284
		public bool makeStartingPack = true;

		// Token: 0x0400011D RID: 285
		public float minEnvResist = 0.15f;

		// Token: 0x0400011E RID: 286
		public List<Pawn> packPawns = new List<Pawn>();

		// Token: 0x0400011F RID: 287
		public List<Pawn> deadPackmates = new List<Pawn>();

		// Token: 0x04000120 RID: 288
		public HediffDef packHediffDef;

		// Token: 0x04000121 RID: 289
		public Pawn leader;

		// Token: 0x04000122 RID: 290
		public bool immuneToPackLoss = true;

		// Token: 0x04000123 RID: 291
		private int pawnPosInQueue = -1;

		// Token: 0x04000124 RID: 292
		private int pawnsInPack = 1;

		// Token: 0x04000125 RID: 293
		private int pawnTotalSkillCount;

		// Token: 0x04000126 RID: 294
		private int packLossStage = -1;
	}
}

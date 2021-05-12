using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Avali
{
	// Token: 0x0200005A RID: 90
	public static class Thoughts_Avali
	{
		// Token: 0x060001D9 RID: 473 RVA: 0x0000FD7C File Offset: 0x0000DF7C
		public static int TryAddPackLossThought(this Pawn pawn, List<Pawn> relatedPawns, int packLossStage)
		{
			if (pawn.ageTracker.CurLifeStage.reproductive)
			{
				Thought thought = pawn.needs.mood.thoughts.memories.OldestMemoryOfDef(ThoughtDefOfAvali.PackLoss);
				if (thought != null)
				{
					if (packLossStage == -1)
					{
						return thought.CurStageIndex;
					}
				}
				else
				{
					for (int i = 0; i < relatedPawns.Count<Pawn>(); i++)
					{
						Pawn pawn2 = relatedPawns[i];
						if (pawn2.Map != null && (pawn.HavePackRelation(pawn2) || pawn.HaveLoveRelation(pawn2)))
						{
							return -1;
						}
					}
					if (packLossStage < 3)
					{
						packLossStage++;
					}
					else if (packLossStage > 3)
					{
						packLossStage = 3;
					}
					pawn.GiveThoughtWithStage(ThoughtDefOfAvali.PackLoss, packLossStage, true);
				}
			}
			return packLossStage;
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0000FE24 File Offset: 0x0000E024
		public static int TryRemovePackLossThought(this Pawn pawn, List<Pawn> relatedPawns, int packLossStage)
		{
			for (int i = 0; i < relatedPawns.Count<Pawn>(); i++)
			{
				Pawn pawn2 = relatedPawns[i];
				if (pawn2.Map != null && (pawn.HavePackRelation(pawn2) || pawn.HaveLoveRelation(pawn2)))
				{
					pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOfAvali.PackLoss);
					return -1;
				}
			}
			return packLossStage;
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000FE88 File Offset: 0x0000E088
		public static void Thought_AvaliSleepingRoomImpressiveness(this Pawn pawn)
		{
			if (pawn.story.traits.HasTrait(TraitDefOf.Ascetic))
			{
				return;
			}
			if (pawn.InBed())
			{
				Room room = pawn.GetRoom(RegionType.Set_Passable);
				if (room != null)
				{
					int scoreStageIndex = RoomStatDefOf.Impressiveness.GetScoreStageIndex(room.GetStat(RoomStatDefOf.Impressiveness));
					pawn.GiveThoughtWithStage(ThoughtDefOfAvali.AvaliSleepingRoomImpressiveness, scoreStageIndex, true);
				}
			}
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0000FEE4 File Offset: 0x0000E0E4
		public static void Thought_AvaliPackSleepingRoomRelations(this Pawn pawn, int maxPawnsInPack = 5)
		{
			if (!pawn.Awake() && !pawn.Downed)
			{
				Room room = pawn.GetRoom(RegionType.Set_Passable);
				if (room != null && room.Role == RoomRoleDefOf.Barracks)
				{
					int num = -1;
					List<Building_Bed> list = room.ContainedBeds.ToList<Building_Bed>();
					for (int i = 0; i < list.Count<Building_Bed>(); i++)
					{
						Building_Bed building_Bed = list[i];
						if (building_Bed.OwnersForReading != null)
						{
							for (int j = 0; j < building_Bed.OwnersForReading.Count; j++)
							{
								Pawn pawn2 = building_Bed.OwnersForReading[j];
								if (pawn2.CurrentBed() == building_Bed && pawn.HavePackRelation(pawn2))
								{
									num++;
								}
							}
						}
					}
					int num2 = maxPawnsInPack - 1;
					if (num > num2)
					{
						num = num2;
					}
					if (num > -1)
					{
						pawn.GiveThoughtWithStage(ThoughtDefOfAvali.AvaliPackSleepingRoomRelations, num, true);
					}
				}
			}
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000FFB8 File Offset: 0x0000E1B8
		public static void Thought_SleepDisturbedAvali(this Pawn pawn, int opinionNeeded = 25)
		{
			if (!pawn.Awake() && !pawn.Downed && pawn.IsColonist)
			{
				Room room = pawn.GetRoom(RegionType.Set_Passable);
				if (room != null)
				{
					List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
					for (int i = 0; i < containedAndAdjacentThings.Count; i++)
					{
						Thing thing = containedAndAdjacentThings[i];
						Pawn pawn2 = thing as Pawn;
						if (pawn2 != null && pawn2.RaceProps.Humanlike && pawn != pawn2 && pawn.relations.OpinionOf(pawn2) < opinionNeeded && !pawn.HavePackRelation(pawn2))
						{
							pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfAvali.AvaliSleepDisturbed, null);
						}
					}
				}
			}
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0001006C File Offset: 0x0000E26C
		public static void GiveThoughtWithStage(this Pawn pawn, ThoughtDef thought, int stage, bool overrideCurStage = true)
		{
			Thought_Memory thought_Memory = (Thought_Memory)ThoughtMaker.MakeThought(thought);
			pawn.needs.mood.thoughts.memories.TryGainMemory(thought_Memory, null);
			if (thought_Memory.CurStage == null)
			{
				pawn.needs.mood.thoughts.memories.OldestMemoryOfDef(thought).SetForcedStage(stage);
				return;
			}
			if (overrideCurStage)
			{
				pawn.needs.mood.thoughts.memories.OldestMemoryOfDef(thought).SetForcedStage(stage);
			}
		}

		// Token: 0x060001DF RID: 479 RVA: 0x000100F0 File Offset: 0x0000E2F0
		public static void AddMemoryOfDef(this Pawn pawn, ThoughtDef thought)
		{
			Thought_Memory newThought = (Thought_Memory)ThoughtMaker.MakeThought(thought);
			pawn.needs.mood.thoughts.memories.TryGainMemory(newThought, null);
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x00010128 File Offset: 0x0000E328
		public static void TryAddNewThoughtToList(ThoughtDef thoughtDef, List<Thought> thoughtsList)
		{
			Thought thought = ThoughtMaker.MakeThought(thoughtDef);
			for (int i = 0; i < thoughtsList.Count; i++)
			{
				Thought thought2 = thoughtsList[i];
				if (thought2 == thought)
				{
					return;
				}
			}
			thoughtsList.Add(thought);
		}
	}
}

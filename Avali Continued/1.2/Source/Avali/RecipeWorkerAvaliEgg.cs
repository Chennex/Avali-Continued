using System;
using RimWorld;
using Verse;

namespace Avali
{
	// Token: 0x02000069 RID: 105
	public class RecipeWorkerAvaliEgg : RecipeWorker
	{
		// Token: 0x06000219 RID: 537 RVA: 0x000111A8 File Offset: 0x0000F3A8
		public override void ConsumeIngredient(Thing ingredient, RecipeDef recipe, Map map)
		{
			Pawn hatcheeParent = ingredient.TryGetComp<CompHatcher>().hatcheeParent;
			Pawn otherParent = ingredient.TryGetComp<CompHatcher>().otherParent;
			if (hatcheeParent != null)
			{
				RecipeWorkerAvaliEgg.GiveThought(hatcheeParent);
			}
			if (otherParent != null)
			{
				RecipeWorkerAvaliEgg.GiveThought(otherParent);
			}
			ingredient.Destroy(DestroyMode.Vanish);
		}

		// Token: 0x0600021A RID: 538 RVA: 0x000111E8 File Offset: 0x0000F3E8
		public static void GiveThought(Pawn parent)
		{
			if (parent.jobs.curJob.bill.recipe.defName == "MakeAvaliEggOmlette")
			{
				parent.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfAvali.ParentForcedToCookAvaliEgg, null);
			}
			else
			{
				parent.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfAvali.AvaliEggCooked, null);
			}
			Hediff hediff = HediffMaker.MakeHediff(HediffDefOfAvali.AvaliCaresOfEgg, parent, null);
			if (parent.health.hediffSet.hediffs.Contains(hediff))
			{
				parent.health.RemoveHediff(hediff);
			}
			parent.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOfAvali.AvaliCaresOfEgg);
		}
	}
}

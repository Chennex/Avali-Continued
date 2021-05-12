using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x02000038 RID: 56
	public class CompTextThing : ThingComp
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000123 RID: 291 RVA: 0x00002A0C File Offset: 0x00000C0C
		public CompProperties_TextThing Props
		{
			get
			{
				return (CompProperties_TextThing)this.props;
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000124 RID: 292 RVA: 0x00002A19 File Offset: 0x00000C19
		protected virtual string FloatMenuOptionLabel
		{
			get
			{
				return this.Props.useLabel;
			}
		}

		// Token: 0x06000125 RID: 293 RVA: 0x0000A788 File Offset: 0x00008988
		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			if (this.Props.author == "")
			{
				this.Props.author = "Unknown".Translate();
			}
			if (this.Props.defaultMarketValue == 0f)
			{
				this.Props.defaultMarketValue = this.parent.def.BaseMarketValue;
			}
			else if (this.workLeft != 0)
			{
				this.parent.def.BaseMarketValue = this.Props.defaultMarketValue;
			}
			if (this.workLeft > this.Props.workLeft)
			{
				this.workLeft = this.Props.workLeft;
			}
			if (this.workLeft == -1)
			{
				this.workLeft = this.Props.workLeft;
			}
			else if (this.workLeft > 0)
			{
				if (this.CheckTale())
				{
					this.workLeft = 0;
				}
			}
			else if (this.workLeft == 0)
			{
				if (this.translator == "")
				{
					this.translator = "UnknownLower".Translate();
				}
				this.parent.def.BaseMarketValue = this.Props.translatedMarketValue;
			}
			if (this.Props.translationTabWinSize != Vector2.zero)
			{
				List<InspectTabBase> list = (List<InspectTabBase>)this.parent.GetInspectTabs();
				for (int i = 0; i < list.Count<InspectTabBase>(); i++)
				{
					ITab_Translation tab_Translation = list[i] as ITab_Translation;
					if (tab_Translation != null)
					{
						tab_Translation.WinSize = this.Props.translationTabWinSize;
						return;
					}
				}
			}
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00002A26 File Offset: 0x00000C26
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<int>(ref this.workLeft, "workLeft", -1, false);
			Scribe_Values.Look<string>(ref this.translator, "translator", "", false);
		}

		// Token: 0x06000127 RID: 295 RVA: 0x0000A918 File Offset: 0x00008B18
		public override void PostSplitOff(Thing piece)
		{
			CompTextThing comp = ((ThingWithComps)piece).GetComp<CompTextThing>();
			comp.workLeft = this.workLeft;
			comp.translator = this.translator;
		}

		// Token: 0x06000128 RID: 296 RVA: 0x0000A94C File Offset: 0x00008B4C
		public override string TransformLabel(string label)
		{
			if (this.workLeft <= 0 && this.Props.labelTranslated != "")
			{
				return label = label + " (" + this.Props.labelTranslated + ")";
			}
			return label;
		}

		// Token: 0x06000129 RID: 297 RVA: 0x0000A99C File Offset: 0x00008B9C
		public override string GetDescriptionPart()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.Props.workLeft > 0 && this.workLeft > 0)
			{
				if (this.Props.descriptionNotTranslated != "")
				{
					stringBuilder.Append(this.Props.descriptionNotTranslated);
				}
				else if (this.parent.def.description != "")
				{
					stringBuilder.Append(this.parent.def.description);
				}
				if (this.Props.showWorkLeft)
				{
					float num = (float)Math.Abs(this.workLeft - this.Props.workLeft);
					if (num > 0f)
					{
						num = 100f / ((float)this.Props.workLeft / num);
					}
					stringBuilder.Append("\n\n" + "TranslationProgress".Translate() + num + "%");
				}
				if (this.Props.workSkill != null)
				{
					stringBuilder.Append(string.Concat(new object[]
					{
						"\n" + "RequredSkill".Translate() + " ",
						this.Props.workSkill,
						"(",
						this.Props.minSkillLevel,
						")"
					}));
				}
				return stringBuilder.ToString();
			}
			if (this.Props.descriptionTranslated != "")
			{
				stringBuilder.Append(this.Props.descriptionTranslated);
			}
			else if (this.Props.descriptionNotTranslated != "")
			{
				stringBuilder.Append(this.Props.descriptionNotTranslated);
			}
			else if (this.parent.def.description != "")
			{
				stringBuilder.Append(this.parent.def.description);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600012A RID: 298 RVA: 0x0000ABAC File Offset: 0x00008DAC
		public override string CompInspectStringExtra()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.Props.workLeft > 0 && this.workLeft > 0)
			{
				if (this.CheckTale())
				{
					this.workLeft = 0;
					if (this.translator == "")
					{
						this.translator = "UnknownLower".Translate();
					}
					this.parent.def.BaseMarketValue = this.Props.translatedMarketValue;
					return null;
				}
				if (this.Props.descriptionNotTranslated != "")
				{
					stringBuilder.Append(this.Props.descriptionNotTranslated);
				}
				else
				{
					stringBuilder.Append(this.parent.def.description);
				}
				stringBuilder.Append("\n ");
				if (this.Props.showWorkLeft && this.Props.workLeft > 0 && this.workLeft > 0)
				{
					float num = (float)Math.Abs(this.workLeft - this.Props.workLeft);
					if (num > 0f)
					{
						num = 100f / ((float)this.Props.workLeft / num);
					}
					stringBuilder.Append("\n" + "TranslationProgress".Translate() + num + "%");
				}
				if (this.Props.workSkill != null)
				{
					stringBuilder.Append(string.Concat(new object[]
					{
						"\n" + "RequredSkill".Translate() + " ",
						this.Props.workSkill,
						"(",
						this.Props.minSkillLevel,
						")"
					}));
				}
			}
			else if (this.Props.descriptionTranslated != "")
			{
				stringBuilder.Append(this.Props.descriptionTranslated);
			}
			else if (this.Props.descriptionNotTranslated != "")
			{
				stringBuilder.Append(this.Props.descriptionNotTranslated);
			}
			else
			{
				stringBuilder.Append(this.parent.def.description);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000ADF8 File Offset: 0x00008FF8
		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			if (this.debug)
			{
				Log.Message(this.parent + " workLeft: " + this.workLeft, false);
			}
			if (this.workLeft != 0)
			{
				if (this.debug)
				{
					Log.Message(this.parent + " Tale check.", false);
				}
				if (this.CheckTale())
				{
					this.workLeft = 0;
					if (this.translator == "")
					{
						this.translator = "UnknownLower".Translate();
					}
					this.parent.def.BaseMarketValue = this.Props.translatedMarketValue;
				}
				else
				{
					if (this.debug)
					{
						Log.Message(this.parent + " Drafted and ToolUser check", false);
					}
					if (!selPawn.Drafted && selPawn.RaceProps.ToolUser)
					{
						if (this.debug)
						{
							Log.Message(this.parent + " CanReserve check.", false);
						}
						if (!selPawn.CanReserve(this.parent, 1, -1, null, false))
						{
							yield return new FloatMenuOption(this.FloatMenuOptionLabel + " (" + "Reserved".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
						}
						else if (this.Props.workSkill != null && selPawn.skills.GetSkill(this.Props.workSkill).Level < this.Props.minSkillLevel)
						{
							yield return new FloatMenuOption("CantTranslate".Translate() + this.Props.workSkill + "SkillLevelToSmall".Translate() + this.Props.minSkillLevel + ".", null, MenuOptionPriority.Default, null, null, 0f, null, null);
						}
						else if (!selPawn.CanReach(this.parent, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
						{
							yield return new FloatMenuOption(selPawn + "CantReach".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
						}
						else
						{
							FloatMenuOption useopt = new FloatMenuOption(this.FloatMenuOptionLabel, delegate()
							{
								foreach (CompUseEffect compUseEffect in this.parent.GetComps<CompUseEffect>())
								{
									if (compUseEffect.SelectedUseOption(selPawn))
									{
										return;
									}
								}
								this.TryStartUseJob(selPawn);
							}, MenuOptionPriority.Default, null, null, 0f, null, null);
							yield return useopt;
						}
					}
				}
			}
			yield break;
		}

		// Token: 0x0600012C RID: 300 RVA: 0x0000AE1C File Offset: 0x0000901C
		public void TryStartUseJob(Pawn selPawn)
		{
			if (this.debug)
			{
				Log.Message("TryStartUseJob", false);
			}
			Job job;
			if (this.Props.workTable == null)
			{
				job = new Job(this.Props.useJob, this.parent, null, null);
				Thing thing = AvaliUtility.FindAllThingsOnMapAtRange(selPawn, null, typeof(Building), null, 15f, 1, true, true).First<Thing>();
				if (thing != null)
				{
					job = new Job(this.Props.useJob, this.parent, thing, null);
				}
				if (this.debug)
				{
					Log.Message(string.Concat(new object[]
					{
						selPawn,
						" job = ",
						this.Props.useJob,
						", ",
						this.parent,
						", ",
						thing
					}), false);
				}
			}
			else
			{
				Thing thing2 = null;
				if (this.Props.workTable == ThingDef.Named("SimpleResearchBench"))
				{
					thing2 = AvaliUtility.FindClosestUnoccupiedThing(selPawn, ThingDef.Named("HiTechResearchBench"), 9999f, true);
					if (this.debug)
					{
						Log.Message("Closest unoccupied hi-tech research bench = " + thing2, false);
					}
				}
				if (thing2 == null)
				{
					thing2 = AvaliUtility.FindClosestUnoccupiedThing(selPawn, this.Props.workTable, 9999f, false);
					if (this.debug)
					{
						Log.Message("Closest unoccupied work table = " + thing2, false);
					}
				}
				if (thing2 == null)
				{
					if (this.debug)
					{
						Log.Message("workTable = null", false);
					}
					return;
				}
				if (!thing2.def.hasInteractionCell)
				{
					Log.Error(thing2 + " not have interaction cell.", false);
					return;
				}
				job = new Job(this.Props.useJob, this.parent, thing2, thing2.OccupiedRect().ClosestCellTo(thing2.InteractionCell));
			}
			job.count = 1;
			selPawn.jobs.TryTakeOrderedJob(job, JobTag.MiscWork);
		}

		// Token: 0x0600012D RID: 301 RVA: 0x0000B020 File Offset: 0x00009220
		public bool CheckTale()
		{
			if (this.Props.taleWhenTranslated != null)
			{
				List<Tale> allTalesListForReading = Find.TaleManager.AllTalesListForReading;
				for (int i = 0; i < allTalesListForReading.Count<Tale>(); i++)
				{
					Tale tale = allTalesListForReading[i];
					if (tale.def == this.Props.taleWhenTranslated)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x04000109 RID: 265
		public int workLeft = -1;

		// Token: 0x0400010A RID: 266
		public string translator = "";

		// Token: 0x0400010B RID: 267
		public bool debug;
	}
}

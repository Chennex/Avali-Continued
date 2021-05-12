using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Avali
{
	// Token: 0x02000014 RID: 20
	public class ITab_Avali_Pack : ITab
	{
		// Token: 0x06000059 RID: 89 RVA: 0x0000233D File Offset: 0x0000053D
		public ITab_Avali_Pack()
		{
			this.size = this.WinSize;
			this.labelKey = "TabAvaliPack";
			this.oldColor = GUI.color;
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600005A RID: 90 RVA: 0x0000237C File Offset: 0x0000057C
		public override bool IsVisible
		{
			get
			{
				return base.SelPawn.def == ThingDefOfAvali.Avali;
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00004B40 File Offset: 0x00002D40
		protected override void FillTab()
		{
			Rect rect = new Rect(0f, 0f, this.WinSize.x, this.WinSize.y).ContractedBy(10f);
			Text.Font = GameFont.Small;
			this.oldColor = GUI.color;
			Hediff firstHediffOfDef = base.SelPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOfAvali.AvaliBiology, false);
			if (firstHediffOfDef == null)
			{
				this.NotInPack(rect);
				return;
			}
			HediffDef hediffDef = (HediffDef)Traverse.Create(firstHediffOfDef).Field("packHediffDef").GetValue();
			List<Pawn> list = (List<Pawn>)Traverse.Create(firstHediffOfDef).Field("packPawns").GetValue();
			Pawn pawn = null;
			int num = (int)Traverse.Create(firstHediffOfDef).Field("maxPawnsInPack").GetValue();
			if (hediffDef == null || num < 2 || list == null)
			{
				this.NotInPack(rect);
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				Pawn pawn2 = list[i];
				if (base.SelPawn.relations.DirectRelationExists(PawnRelationDefOfAvali.PackLeader, pawn2))
				{
					pawn = pawn2;
					break;
				}
			}
			if (pawn == null)
			{
				pawn = base.SelPawn;
			}
			this.packHediff = base.SelPawn.health.hediffSet.GetFirstHediffOfDef(hediffDef, false);
			if (this.packHediff == null)
			{
				this.NotInPack(rect);
				return;
			}
			Rect rect2 = new Rect(0f, 20f, this.WinSize.x, 42f).ContractedBy(10f);
			ITab_Avali_Pack.NewRect(rect2, "PackSpecialization".Translate(), this.packHediff.LabelCap, rect2, "PackSpecializationDesc".Translate());
			this.DrawLineHorizontalWithColor(10f, 52f, this.WinSize.x - 20f, Color.gray);
			Rect rect3 = new Rect(0f, 44f, this.WinSize.x, 42f).ContractedBy(10f);
			ITab_Avali_Pack.NewRect(rect3, "PackLeader".Translate(), pawn.Name.ToString(), rect3, "PackLeaderDesc".Translate());
			Rect rect4 = new Rect(0f, 68f, this.WinSize.x, 42f).ContractedBy(10f);
			string text = base.SelPawn.Name.ToString();
			int num2 = 1;
			for (int j = 0; j < list.Count; j++)
			{
				Pawn pawn3 = list[j];
				if (pawn3 != base.SelPawn)
				{
					text = text + "\n" + pawn3.Name;
					num2++;
				}
			}
			ITab_Avali_Pack.NewRect(rect4, "PackMembers".Translate(), num2 + "/" + num, rect4, text);
			if (this.packHediff.CurStage == null || this.packHediff.CurStage.statOffsets == null || this.packHediff.CurStage.statOffsets.Count == 0)
			{
				this.NotInPack(rect);
				return;
			}
			if (base.SelPawn.IsColonist || Prefs.DevMode)
			{
				this.DrawLineHorizontalWithColor(10f, 148f, this.WinSize.x - 20f, Color.gray);
				Rect rect5 = new Rect(0f, 116f, this.WinSize.x, 42f).ContractedBy(10f);
				float num3 = 30f * base.SelPawn.health.capacities.GetLevel(PawnCapacityDefOf.Hearing);
				ITab_Avali_Pack.NewRect(rect5, "PackEffects".Translate(), "", rect5, string.Format("PackEffectsDesc".Translate(), (int)num3));
				float num4 = 140f;
				List<StatModifier> statOffsets = this.packHediff.CurStage.statOffsets;
				if (statOffsets.Count > 1)
				{
					int num5 = statOffsets.Count / 2;
					for (int k = num5; k < statOffsets.Count; k++)
					{
						StatModifier statModifier = statOffsets[k];
						if (statModifier != null)
						{
							Rect rect6 = new Rect(this.WinSize.x / 2f - 10f, num4, this.WinSize.x / 2f + 10f, 42f).ContractedBy(10f);
							Rect rectLabelRect = new Rect(this.WinSize.x - 75f, num4 + 10f, 1000f, 42f);
							ITab_Avali_Pack.NewRect(rect6, statModifier.stat.LabelCap, statModifier.ValueToStringAsOffset, rectLabelRect, statModifier.stat.description);
							num4 += 24f;
						}
					}
				}
				num4 = 140f;
				for (int l = 0; l < statOffsets.Count / 2; l++)
				{
					StatModifier statModifier2 = statOffsets[l];
					if (statModifier2 != null)
					{
						Rect rect7 = new Rect(0f, num4, this.WinSize.x / 2f + 10f, 42f).ContractedBy(10f);
						Rect rectLabelRect2 = new Rect(this.WinSize.x / 2f - 75f, num4 + 10f, 1000f, 42f);
						ITab_Avali_Pack.NewRect(rect7, statModifier2.stat.LabelCap, statModifier2.ValueToStringAsOffset, rectLabelRect2, statModifier2.stat.description);
						num4 += 24f;
					}
				}
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00002390 File Offset: 0x00000590
		public void DrawLineHorizontalWithColor(float x, float y, float length, Color color)
		{
			GUI.color = color;
			Widgets.DrawLineHorizontal(x, y, length);
			GUI.color = this.oldColor;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00005108 File Offset: 0x00003308
		public static void NewRect(Rect rect, string label, string rectLabel, Rect rectLabelRect, string tooltip = null)
		{
			Text.Font = GameFont.Small;
			Widgets.Label(rect, label);
			if (rectLabel != null)
			{
				if (rect == rectLabelRect)
				{
					rectLabelRect = new Rect(rect.center.x, rect.y, rect.width, rect.height);
				}
				Widgets.Label(rectLabelRect, rectLabel);
			}
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
			}
			if (tooltip != null)
			{
				TooltipHandler.TipRegion(rect, new TipSignal(tooltip, rect.GetHashCode()));
			}
		}

		// Token: 0x0600005E RID: 94 RVA: 0x0000518C File Offset: 0x0000338C
		public virtual string GetPackStatOffsets()
		{
			List<StatModifier> statOffsets = this.packHediff.CurStage.statOffsets;
			string text = "";
			for (int i = 0; i < statOffsets.Count; i++)
			{
				StatModifier arg = statOffsets[i];
				text = text + arg + "\n";
			}
			return text;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000051D8 File Offset: 0x000033D8
		public void NotInPack(Rect rect)
		{
			string text = string.Format("NotInPack".Translate(), base.SelPawn);
			Text.Font = GameFont.Medium;
			GUI.color = Color.gray;
			rect = new Rect(rect.x + this.WinSize.x / 4f - (float)text.Length, rect.y + this.WinSize.y / 2f - (float)text.Length, rect.width, rect.height);
			Widgets.Label(rect, text);
		}

		// Token: 0x0400003C RID: 60
		public const float startPosY = 20f;

		// Token: 0x0400003D RID: 61
		public const float labelSizeY = 24f;

		// Token: 0x0400003E RID: 62
		public Vector2 WinSize = new Vector2(630f, 510f);

		// Token: 0x0400003F RID: 63
		private Hediff packHediff;

		// Token: 0x04000040 RID: 64
		private Color oldColor;
	}
}

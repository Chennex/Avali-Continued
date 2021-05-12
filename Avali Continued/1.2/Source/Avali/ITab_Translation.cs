using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace Avali
{
	// Token: 0x02000042 RID: 66
	public class ITab_Translation : ITab
	{
		// Token: 0x06000166 RID: 358 RVA: 0x0000D9E0 File Offset: 0x0000BBE0
		public ITab_Translation()
		{
			this.size = this.WinSize;
			this.labelKey = "TabText";
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000167 RID: 359 RVA: 0x0000DA34 File Offset: 0x0000BC34
		private Thing thing
		{
			get
			{
				Thing thing = Find.Selector.SingleSelectedThing;
				MinifiedThing minifiedThing = thing as MinifiedThing;
				if (minifiedThing != null)
				{
					thing = minifiedThing.InnerThing;
				}
				if (thing == null)
				{
					return null;
				}
				return thing;
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000168 RID: 360 RVA: 0x00002B1C File Offset: 0x00000D1C
		private CompTextThing SelectedCompTextThing
		{
			get
			{
				return this.thing.TryGetComp<CompTextThing>();
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000169 RID: 361 RVA: 0x00002B29 File Offset: 0x00000D29
		private CompProperties_TextThing Props
		{
			get
			{
				return this.SelectedCompTextThing.Props;
			}
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0000DA64 File Offset: 0x0000BC64
		protected override void FillTab()
		{
			Rect rect = new Rect(0f, 0f, this.WinSize.x, this.WinSize.y).ContractedBy(10f);
			Text.Font = GameFont.Medium;
			string text = "";
			text += this.thing.def.label;
			if (this.SelectedCompTextThing.workLeft <= 0 && this.Props.labelTranslated != "")
			{
				text = text + " (" + this.Props.labelTranslated + ")";
			}
			Widgets.Label(rect, text);
			rect.yMin += 35f;
			Text.Font = GameFont.Small;
			string text2 = "";
			if (this.SelectedCompTextThing.workLeft <= 0)
			{
				text2 += this.Props.translatedText;
			}
			else
			{
				text2 += this.Props.descriptionNotTranslated;
			}
			if (this.SelectedCompTextThing.workLeft <= 0)
			{
				text2 += "\n";
				if (this.Props.showAuthor)
				{
					text2 += "\n" + "Author".Translate() + ": " + this.Props.author;
				}
				if (this.Props.showTranslator)
				{
					text2 += "\n" + "Translator".Translate() + ": " + this.SelectedCompTextThing.translator;
				}
			}
			Widgets.LabelScrollable(rect, text2, ref this.scrollPosition, false, true, false);
		}

		// Token: 0x04000135 RID: 309
		public Vector2 WinSize = new Vector2(800f, 510f);

		// Token: 0x04000136 RID: 310
		private Vector2 scrollPosition = new Vector2(0f, 0f);
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using CashApp.DataBase;
using CashApp.Instruments;
using CashApp.Transactions;
using CashApp.UIElements;

namespace CashApp.Planned
{
	public partial class PlannedInform : Window
	{
		readonly Tab TabProfits = new Tab("Доходные", new ListPanel { ItemHeight = 50 });
		readonly Tab TabLosses = new Tab("Расходные", new ListPanel { ItemHeight = 50 });
		ListPanel SelectList;

		List<PlannedDB> Profits = new List<PlannedDB>();
		List<PlannedDB> Losses = new List<PlannedDB>();
		List<PlannedDB> SelectData;


		void XDelete_Click(object sender, EventArgs e)
		{
			if (SelectList.SelectItem == null)
			{
				MsgBox.Show("Плановая операция не выбранная!", "Плановые", MsgIcon.Error, "Ок");
				return;
			}


			int index = SelectList.SelectIndex;

			if (MsgBox.Show("Удалить плановую операцию?", "Плановые", MsgIcon.Question, "Да", "Нет") == "Нет") return;


			SelectList.Items.RemoveAt(index);
			DBData.Planned.Remove(SelectData[index]);
			SelectData.RemoveAt(index);
		}

		void XChange_Click(object sender, EventArgs e)
		{
			if (XPlanned.SelectItem == null)
			{
				MsgBox.Show("Плановая операция не выбранная!", "Плановые", MsgIcon.Error, "Ок");
				return;
			}


			int index = SelectList.SelectIndex;

			PlannedForm window = new PlannedForm();

			if (!window.ShowDialog_Change(SelectData[index])) return;

			DownloadPlanned();
		}

		void DownloadPlanned()
		{
			ListPanel profitsList = (ListPanel)TabProfits.Container,
					  lossesList = (ListPanel)TabLosses.Container;

			profitsList.Items.Clear();
			lossesList.Items.Clear();

			Profits.Clear();
			Losses.Clear();


			foreach (PlannedDB planned in DBData.Planned)
				(planned.Type == AmountType.Profit ? Profits : Losses).Add(planned);


			Profits = Sorter.Sort(Profits, (item1, item2) => item2.Date < item1.Date ? item2 : item1).ToList();
			Losses = Sorter.Sort(Losses, (item1, item2) => item2.Date < item1.Date ? item2 : item1).ToList();

			profitsList.Items.AddRange( Profits.Select(p => new TransactionItem(p) { Margin = new Thickness(5) }).ToArray() );
			lossesList.Items.AddRange( Losses.Select(p => new TransactionItem(p) { Margin = new Thickness(5) }).ToArray() );
		}


		public PlannedInform()
		{
			InitializeComponent();

			XTop.CloseButton.Click += (sender, e) => Close();
			XTop.MinimizeButton.Click += (sender, e) => WindowState = WindowState.Minimized;
			XTop.Body.MouseDown += (sender, e) => DragMove();


			XDelete.Click += XDelete_Click;
			XChange.Click += XChange_Click;


			XPlanned.SelectedTab += (sender, tab) => {
				SelectList = (ListPanel)tab.Container;
				SelectData = tab == TabProfits ? Profits : Losses;
			};
			XPlanned.Items.Add(TabProfits, TabLosses);

			DownloadPlanned();
		}
	}
}

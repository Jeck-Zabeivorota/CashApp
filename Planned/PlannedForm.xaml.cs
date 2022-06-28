using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using CashApp.DataBase;
using CashApp.UIElements;
using CashApp.Instruments;

namespace CashApp.Planned
{
	public partial class PlannedForm : Window
	{
		// FIELDS

		readonly TextBlock PROFIT, LOSS;
		const string NONE = "<Ничего>";

		Field<TextBlock> XWallet;
		Field<ChoiceChips> XType;
		Field<DropButton> XCategory;
		Field<NumberField> XAmount, XDays, XWeeks, XMounts;
		Field<DateSelector> XDate;

		List<CategoryDB> Categories;

		public PlannedDB PlannedData;
		bool Done = false;


		// METHODS

		void SetCategories()
		{
			AmountType type = XType.Input.SelectItem == PROFIT ? AmountType.Profit : AmountType.Loss;

			Categories = DBData.Categories.Where(c => c.Type == type && (c.WalletId == 0 || c.WalletId == PlannedData.WalletId)).ToList();

			XCategory.Input.Items.Clear();
			XCategory.Input.Items.Add(new TextBlock { Text = NONE, Foreground = Colors.MainText });
			XCategory.Input.Items.AddRange(Categories.Select(c => new TextBlock { Text = c.Name, Foreground = Colors.MainText }).ToArray());
		}

		void XDone_Click(object sender, EventArgs e)
		{
			PlannedData.Type = XType.Input.SelectItem == PROFIT ? AmountType.Profit : AmountType.Loss;

			PlannedData.Amount = XAmount.Input.Value;

			if (XCategory.Input.SelectItem != NONE)
				PlannedData.CategoryId = Categories.Find(c => c.Name == XCategory.Input.SelectItem && c.Type == PlannedData.Type).Id;
			else
				PlannedData.CategoryId = 0;

			PlannedData.Date = XDate.Input.Date;

			PlannedData.Regularity = new DateSpan((uint)XDays.Input.Value, (uint)XWeeks.Input.Value, (uint)XMounts.Input.Value, 0);


			if (PlannedData.Id == null)
				DBData.Planned.Add(PlannedData);
			else
				PlannedData.SaveChanges();


			Done = true;
			Close();
		}


		TextBlock CreateCapture(string text)
		{
			return new TextBlock
			{
				Text = text,
				Foreground = Colors.MainText,
				Margin = new Thickness { Left = 10, Right = 10 },
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center
			};
		}

		void BuildFields()
		{
			// WALLET

			XWallet = new Field<TextBlock>("Кошелек:", new TextBlock { Foreground = Colors.SecondText1 });


			// TYPE

			XType = new Field<ChoiceChips>("Тип:", new ChoiceChips());

			XType.Input.Items.Add(PROFIT, LOSS);

			XType.Input.SelectionChanged += (sender, item) => SetCategories();


			// AMOUNT

			XAmount = new Field<NumberField>("Сумма:", new NumberField { Height = 20, Width = 100, Maximum = 1000000000 });


			// CATEGORY

			XCategory = new Field<DropButton>("Категория:", new DropButton { Height = 20, Width = 100 });


			// DATE

			XDate = new Field<DateSelector>("Дата:", new DateSelector { Height = 25 });


			// REGULARITY

			TextBlock regularity = new TextBlock
			{
				Text = "Повторить через каждые:",
				Foreground = Colors.MainText,
				Margin = new Thickness { Top = 8, Bottom = 8 }
			};

			XDays = new Field<NumberField>("Дней:", new NumberField { Height = 25, Width = 70 });
			XDays.Body.Margin = new Thickness { Left = 20 };

			XWeeks = new Field<NumberField>("Недель:", new NumberField { Height = 25, Width = 70 });
			XWeeks.Body.Margin = new Thickness { Left = 20 };

			XMounts = new Field<NumberField>("Месяцев:", new NumberField { Height = 25, Width = 70 });
			XMounts.Body.Margin = new Thickness { Left = 20 };


			// ADD FIELDS

			StackPanel properties = (StackPanel)_XProperties_.Content;

			properties.Children.Add(XWallet.Body);
			properties.Children.Add(XType.Body);
			properties.Children.Add(XAmount.Body);
			properties.Children.Add(XCategory.Body);
			properties.Children.Add(XDate.Body);
			properties.Children.Add(regularity);
			properties.Children.Add(XDays.Body);
			properties.Children.Add(XWeeks.Body);
			properties.Children.Add(XMounts.Body);
		}


		void SetFields(PlannedDB data)
		{
			XWallet.Input.Text = DBData.Wallets.Find(w => w.Id == data.WalletId).Capture;

			XType.Input.SelectItem = data.Type == AmountType.Profit ? PROFIT : LOSS;

			XAmount.Input.Value = (double)data.Amount;

			SetCategories();
			if (data.CategoryId != 0)
				XCategory.Input.SelectItem = Categories.Find(c => c.Id == data.CategoryId).Name;

			XDate.Input.Date = data.Date.Value;

			XDays.Input.Value = data.Regularity.Value.Days;
			XWeeks.Input.Value = data.Regularity.Value.Weeks;
			XMounts.Input.Value = data.Regularity.Value.Mounts;
		}

		public bool ShowDialog_Create(long walletId)
		{
			PlannedData = new PlannedDB { WalletId = walletId };

			XWallet.Input.Text = DBData.Wallets.Find(w => w.Id == walletId).Capture;
			SetCategories();

			ShowDialog();
			return Done;
		}

		public bool ShowDialog_Change(PlannedDB data)
		{
			if (data.Id == null) throw new ArgumentNullException("\"Id\" is null");

			PlannedData = data;
			SetFields(data);

			((TextBlock)XDone.Child).Text = "Сохранить";

			ShowDialog();
			return Done;
		}


		// CONSTRUCTORS

		public PlannedForm()
		{
			InitializeComponent();

			PROFIT = CreateCapture("Доход");
			LOSS = CreateCapture("Расход");

			BuildFields();

			XTop.CloseButton.Click += (sender, e) => Close();
			XTop.MinimizeButton.Click += (sender, e) => WindowState = WindowState.Minimized;
			XTop.Body.MouseDown += (sender, e) => DragMove();

			XCancel.Click += (sender, e) => Close();
			XDone.Click += XDone_Click;
		}
	}
}

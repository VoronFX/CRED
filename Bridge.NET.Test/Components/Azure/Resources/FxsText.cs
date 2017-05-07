using System;
using Bridge.React;

namespace Bridge.NET.Test.Components.Azure.Resources
{
	public interface IFxsText
	{
		string SearchTooltip { get; }
		string ProductName { get; }
		string DashboardTooltip { get; }
		string InternalText { get; }
		string BrowseText { get; }
		string CreateText { get; }
		string ShowMenu { get; }
		string ExitCustomizeText { get; }
		string Notifications { get; }
		string Console { get; }
		string Settings { get; }
		string Feedback { get; }
		string HelpTooltip { get; }
		string DashboardMessage { get; }
		string ExitCustomizeButton { get; }
		string ViewDashboardButton { get; }
		string DashboardSaveButtonText { get; }
		string DiscardDashboardButton { get; }
	}

	public sealed class FxsTextRu : IFxsText
	{
		public string SearchTooltip { get; } = "����� �������� (����������)";
		public string ProductName { get; } = "Microsoft Azure";
		public string DashboardTooltip { get; } = "������� � ������ �����������";
		public string InternalText { get; } = "��������������� ������";
		public string BrowseText { get; } = "More services";
		public string CreateText { get; } = "�������";
		public string ShowMenu { get; } = "�������� ����";

		public string ExitCustomizeText { get; } =
			"����������, ������������ � ����������� ������, � ����� ������� �� ������.";

		public string Notifications { get; } = "�����������";
		public string Console { get; } = "�������";
		public string Settings { get; } = "���������";
		public string Feedback { get; } = "������ � �����������";
		public string HelpTooltip { get; } = string.Empty;
		public string DashboardMessage { get; } = "� ���� ������ ����������� ���� ���������������� ���������.";
		public string ExitCustomizeButton { get; } = "��������� ���������";
		public string ViewDashboardButton { get; } = "����������� ������ �����������";
		public string DashboardSaveButtonText { get; } = "������������ ���������";
		public string DiscardDashboardButton { get; } = "��������";
	}
}
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
		string ExitCustomizeButton { get; }
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

		public string ExitCustomizeButton { get; } = "��������� ���������";
	}
}
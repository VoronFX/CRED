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
		public string SearchTooltip { get; } = "Поиск ресурсов (глобальный)";
		public string ProductName { get; } = "Microsoft Azure";
		public string DashboardTooltip { get; } = "Перейти к панели мониторинга";
		public string InternalText { get; } = "Предварительная версия";
		public string BrowseText { get; } = "More services";
		public string CreateText { get; } = "Создать";
		public string ShowMenu { get; } = "Показать меню";

		public string ExitCustomizeText { get; } =
			"Добавляйте, передвигайте и закрепляйте плитки, а также меняйте их размер.";

		public string ExitCustomizeButton { get; } = "Настройка завершена";
	}
}
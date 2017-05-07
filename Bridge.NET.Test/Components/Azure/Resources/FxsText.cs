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
		public string SearchTooltip { get; } = "Поиск ресурсов (глобальный)";
		public string ProductName { get; } = "Microsoft Azure";
		public string DashboardTooltip { get; } = "Перейти к панели мониторинга";
		public string InternalText { get; } = "Предварительная версия";
		public string BrowseText { get; } = "More services";
		public string CreateText { get; } = "Создать";
		public string ShowMenu { get; } = "Показать меню";

		public string ExitCustomizeText { get; } =
			"Добавляйте, передвигайте и закрепляйте плитки, а также меняйте их размер.";

		public string Notifications { get; } = "Уведомления";
		public string Console { get; } = "Консоль";
		public string Settings { get; } = "Параметры";
		public string Feedback { get; } = "Отзывы и предложения";
		public string HelpTooltip { get; } = string.Empty;
		public string DashboardMessage { get; } = "В этой панели мониторинга есть неопубликованные изменения.";
		public string ExitCustomizeButton { get; } = "Настройка завершена";
		public string ViewDashboardButton { get; } = "Просмотреть панель мониторинга";
		public string DashboardSaveButtonText { get; } = "Опубликовать изменения";
		public string DiscardDashboardButton { get; } = "Отменить";
	}
}
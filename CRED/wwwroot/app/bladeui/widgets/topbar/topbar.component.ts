import { Component, ViewEncapsulation } from "@angular/core";
import { DomSanitizer } from '@angular/platform-browser';

import { Resources, BladeUiService, MsPortalFxImages } from "../../definitions";

class TopBarAzure {
    text: any = new class {
        exitCustomizeButton = "Готово";
        exitCustomizeText = "Передвигайте и закрепляйте плитки, а также меняйте их размер.";
        discardDashboardButton = "Отменить";
        internalText = "Внутрен.";
        previewText = "BETA";
        customize = "Настроить";
        settings = "Параметры";
        feedback = "Отзывы и предложения";
        exitDevModeLinkText = ""; //?
        reportBug = ""; //?
    };
    isInternalOnly = false;

    showPreview = true;
    inDevMode = false;
    _exitDevMode() { };

    canReportBug = false;
    _reportBug() { };

    dashboardMessage = "";
    dashboardSaveButtonText = "Сохранить копию";
    productName = "Microsoft Azure";
    dashboardTooltip = "Открыть панель мониторинга (D)";
    journeysTooltip = "Переключить задачи (A)";
    searchTooltip = "Поиск ресурсов (/)";
    helpTooltip = "Справка и поддержка";

    customizeIcon = "";
    settingsIcon = new MsPortalFxImages().Shell.Chevron;
    feedbackIcon = "";
}

@Component({
    selector: ".fxs-topbar",
    templateUrl: Resources.Urls.Templates.TopBar,
    encapsulation: ViewEncapsulation.None,
    styleUrls: Resources.WidgetBase
})
export class TopBarComponent extends TopBarAzure {
    constructor(bladeUiService: BladeUiService, private sanitizer: DomSanitizer) {
        super();
        this.settingsIcon = sanitizer.bypassSecurityTrustHtml(new MsPortalFxImages().Shell.Chevron);
    }
    productName = "CRED";
}
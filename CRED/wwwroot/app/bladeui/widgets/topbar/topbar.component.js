"use strict";
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var core_1 = require("@angular/core");
var platform_browser_1 = require("@angular/platform-browser");
var definitions_1 = require("../../definitions");
var TopBarAzure = (function () {
    function TopBarAzure() {
        this.text = new (function () {
            function class_1() {
                this.exitCustomizeButton = "Готово";
                this.exitCustomizeText = "Передвигайте и закрепляйте плитки, а также меняйте их размер.";
                this.discardDashboardButton = "Отменить";
                this.internalText = "Внутрен.";
                this.customize = "Настроить";
                this.settings = "Параметры";
                this.feedback = "Отзывы и предложения";
                this.exitDevModeLinkText = ""; //?
                this.reportBug = ""; //?
            }
            return class_1;
        }());
        this.isInternalOnly = false;
        this.inDevMode = false;
        this.canReportBug = false;
        this.dashboardMessage = "";
        this.dashboardSaveButtonText = "Сохранить копию";
        this.productName = "Microsoft Azure";
        this.dashboardTooltip = "Открыть панель мониторинга (D)";
        this.journeysTooltip = "Переключить задачи (A)";
        this.searchTooltip = "Поиск ресурсов (/)";
        this.helpTooltip = "Справка и поддержка";
        this.customizeIcon = "";
        this.settingsIcon = new definitions_1.MsPortalFxImages().Shell.Chevron;
        this.feedbackIcon = "";
    }
    TopBarAzure.prototype._exitDevMode = function () { };
    ;
    TopBarAzure.prototype._reportBug = function () { };
    ;
    return TopBarAzure;
}());
var TopBarComponent = (function (_super) {
    __extends(TopBarComponent, _super);
    function TopBarComponent(bladeUiService, sanitizer) {
        var _this = _super.call(this) || this;
        _this.sanitizer = sanitizer;
        _this.productName = "My Little Entreprise";
        _this.settingsIcon = sanitizer.bypassSecurityTrustHtml(new definitions_1.MsPortalFxImages().Shell.Chevron);
        return _this;
    }
    return TopBarComponent;
}(TopBarAzure));
TopBarComponent = __decorate([
    core_1.Component({
        selector: ".fxs-topbar",
        templateUrl: definitions_1.Resources.Urls.Templates.TopBar,
        encapsulation: core_1.ViewEncapsulation.None,
        styleUrls: definitions_1.Resources.WidgetBase
    }),
    __metadata("design:paramtypes", [definitions_1.BladeUiService, platform_browser_1.DomSanitizer])
], TopBarComponent);
exports.TopBarComponent = TopBarComponent;
//# sourceMappingURL=topbar.component.js.map
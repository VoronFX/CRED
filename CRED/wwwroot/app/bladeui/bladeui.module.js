"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var core_1 = require("@angular/core");
var common_1 = require("@angular/common");
var definitions_1 = require("./definitions");
var BladeUiModule = (function () {
    function BladeUiModule() {
    }
    return BladeUiModule;
}());
BladeUiModule = __decorate([
    core_1.NgModule({
        imports: [common_1.CommonModule],
        declarations: [
            definitions_1.ContentHostDirective,
            definitions_1.BladeComponent,
            definitions_1.PanoramaComponent,
            definitions_1.PortalComponent,
            definitions_1.SideBarComponent,
            definitions_1.TopBarComponent,
            definitions_1.AzureSvgSymbolsComponent,
            definitions_1.SettingsComponent,
            definitions_1.ContextPaneComponent,
            definitions_1.SettingsPaneComponent
        ],
        entryComponents: [definitions_1.SettingsPaneComponent],
        exports: [definitions_1.PortalComponent],
        bootstrap: []
    })
], BladeUiModule);
exports.BladeUiModule = BladeUiModule;
//var DefaultFavorites: Favorite[] = [
//    { label: "Документы", uri: "#", opensExternal: false, isFavorite: false },
//    { label: "Товары", uri: "Mr. Nice", opensExternal: false, isFavorite: false },
//    { label: "Пользователи", uri: "Mr. Nice", opensExternal: false, isFavorite: false },
//    { label: "Аналитика", uri: "Mr. Nice", opensExternal: false, isFavorite: false },
//];
//# sourceMappingURL=bladeui.module.js.map
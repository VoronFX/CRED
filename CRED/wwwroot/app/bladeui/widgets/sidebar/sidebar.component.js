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
var definitions_1 = require("../../definitions");
//import { TestBladeComponent } from "../../../mle/definitions";
var SideBarAzure = (function () {
    function SideBarAzure() {
        this.favorites = [];
        this.possibleFavorites = [];
        this.createTooltip = "Создать (N)";
        this.createText = "Создать";
        this.browseTooltip = "Обзор (B)";
        this.browseText = "Обзор";
        this.MsPortalFxImages = new definitions_1.MsPortalFxImages();
        this.previewText = ""; //?
    }
    return SideBarAzure;
}());
var SideBarComponent = (function (_super) {
    __extends(SideBarComponent, _super);
    function SideBarComponent(bladeUiService) {
        var _this = _super.call(this) || this;
        bladeUiService.sidebar = _this;
        _this.favorites = [
            new Favorite({ label: "Документы", uri: "#", opensExternal: false, isFavorite: false }),
            new Favorite({ label: "Товары", uri: "Mr. Nice", opensExternal: false, isFavorite: false }),
            new Favorite({ label: "Пользователи", uri: "Mr. Nice", opensExternal: false, isFavorite: false }),
            new Favorite({ label: "Аналитика", uri: "Mr. Nice", opensExternal: false, isFavorite: false }),
        ];
        _this.favorites[0].action = function () {
            //let blade = new Blade();
            //let blade2 = new Blade();
            //blade2.init = () => {
            //    blade2.contentComponent = TestBladeComponent;
            //    blade2.component.showTranslucentSpinner = true;
            //    blade2.component.loaded = true;
            //    setTimeout(() => {
            //            blade2.component.showTranslucentSpinner = false;
            //        },
            //        10000);
            //};
            //blade.init = () => {
            //    blade.contentComponent = TestBladeComponent;
            //    blade.component.showTranslucentSpinner = true;
            //    blade.component.loaded = true;
            //    setTimeout(() => {
            //            blade.component.showTranslucentSpinner = false;
            //        },
            //        10000);
            //};
            //bladeUiService.panorama.openBlade(blade, this);
            // coreService.panorama.openBlade(blade2, blade);
        };
        return _this;
    }
    return SideBarComponent;
}(SideBarAzure));
SideBarComponent = __decorate([
    core_1.Component({
        selector: ".fxs-sidebar",
        templateUrl: definitions_1.Resources.Urls.Templates.SideBar,
        encapsulation: core_1.ViewEncapsulation.None,
        styleUrls: definitions_1.Resources.WidgetBase,
        host: { '[class.fxs-sidebar-is-collapsed]': "isCollapsed" }
    }),
    __metadata("design:paramtypes", [definitions_1.BladeUiService])
], SideBarComponent);
exports.SideBarComponent = SideBarComponent;
var Favorite = (function () {
    function Favorite(fields) {
        Object.assign(this, fields);
    }
    Favorite.prototype.isFilteredOut = function () { };
    ;
    return Favorite;
}());
exports.Favorite = Favorite;
//# sourceMappingURL=sidebar.component.js.map
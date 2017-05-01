"use strict";
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
var definitions_1 = require("./definitions");
var BladeUiService = (function () {
    function BladeUiService() {
        var _this = this;
        this.favorites = [
            new definitions_1.Favorite({
                label: "New Blade", uri: "javaScript:void(0);", opensExternal: false, isFavorite: false, action: function () {
                    _this.panorama.openBlade(new definitions_1.Blade(), _this);
                }
            }),
            new definitions_1.Favorite({ label: "Товары", uri: "Mr. Nice", opensExternal: false, isFavorite: false }),
            new definitions_1.Favorite({ label: "Пользователи", uri: "Mr. Nice", opensExternal: false, isFavorite: false }),
            new definitions_1.Favorite({ label: "Аналитика", uri: "Mr. Nice", opensExternal: false, isFavorite: false }),
        ];
        this.loading = true;
        setTimeout(function () { return _this.rightContextPane.open(definitions_1.SettingsPaneComponent, null); }, 2000);
    }
    return BladeUiService;
}());
BladeUiService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [])
], BladeUiService);
exports.BladeUiService = BladeUiService;
//# sourceMappingURL=bladeui.service.js.map
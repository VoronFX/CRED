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
var definitions_1 = require("../bladeui/definitions");
var TestComponent = (function () {
    function TestComponent(bladeUiService) {
        this.theme = 'azure';
        this.bladeUiService = bladeUiService;
    }
    TestComponent.prototype.ngAfterViewInit = function () {
        //setTimeout(() => { this.theme = 'dark'; }, 5000);
        //this.bladeUiService.sidebar.favorites = [
        //    new Favorite({
        //        label: "New Blade", uri: "javaScript:void(0);", opensExternal: false, isFavorite: false, action: ()=> {
        //        this.bladeUiService.panorama.openBlade(new Blade(), this);
        //    }}),
        //    new Favorite({ label: "Товары", uri: "Mr. Nice", opensExternal: false, isFavorite: false }),
        //    new Favorite({ label: "Пользователи", uri: "Mr. Nice", opensExternal: false, isFavorite: false }),
        //    new Favorite({ label: "Аналитика", uri: "Mr. Nice", opensExternal: false, isFavorite: false }),
        //];
    };
    return TestComponent;
}());
TestComponent = __decorate([
    core_1.Component({
        selector: "test-component",
        templateUrl: "component/TestComponent"
    }),
    __metadata("design:paramtypes", [definitions_1.BladeUiService])
], TestComponent);
exports.TestComponent = TestComponent;
//# sourceMappingURL=test.component.js.map
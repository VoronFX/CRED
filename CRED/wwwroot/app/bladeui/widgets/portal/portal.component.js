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
var definitions_1 = require("../../definitions");
var PortalComponent = (function () {
    function PortalComponent(bladeUiService) {
        this.classes = "fxs-portal fxs-desktop-normal fxs-portal-sidebar-is-visible fxs-show-journey";
    }
    return PortalComponent;
}());
__decorate([
    core_1.HostBinding("class"),
    __metadata("design:type", Object)
], PortalComponent.prototype, "classes", void 0);
PortalComponent = __decorate([
    core_1.Component({
        selector: ".portal",
        templateUrl: definitions_1.Resources.Urls.Templates.Portal,
        styleUrls: definitions_1.Resources.WidgetBase,
        encapsulation: core_1.ViewEncapsulation.None
    }),
    __metadata("design:paramtypes", [definitions_1.BladeUiService])
], PortalComponent);
exports.PortalComponent = PortalComponent;
//# sourceMappingURL=portal.component.js.map
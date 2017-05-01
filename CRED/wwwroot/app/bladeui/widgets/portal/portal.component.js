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
// NO MORE NEED | reparse all css inside azure with regex ((?<=\s)body|^body) and replace to .portal-body
var PortalComponent = (function () {
    //@HostBinding("class") classes = "fxs-portal fxs-desktop-normal fxs-portal-sidebar-is-visible fxs-show-journey";
    function PortalComponent(bladeUiService) {
        this.themeClasses = [
            { name: "azure", classes: ["fxs-theme-azure", "fxs-mode-light", "ext-mode-light"] },
            { name: "blue", classes: ["fxs-theme-blue", "fxs-mode-light", "ext-mode-light"] },
            { name: "light", classes: ["fxs-theme-light", "fxs-mode-light", "ext-mode-light"] },
            { name: "dark", classes: ["fxs-theme-dark", "fxs-mode-dark", "ext-mode-dark"] },
            { name: "dark-blue", classes: ["fxs-theme-blue", "fxs-mode-dark", "ext-mode-dark"] }
        ];
        this.setTheme(0, false);
    }
    Object.defineProperty(PortalComponent.prototype, "theme", {
        get: function () { return this._theme; },
        set: function (theme) {
            for (var i = 0; i < this.themeClasses.length; i++) {
                this.setTheme(i, true);
            }
            for (var i = 0; i < this.themeClasses.length; i++) {
                if (theme === this.themeClasses[i].name) {
                    this.setTheme(i, false);
                    return;
                }
            }
            this.setTheme(0, false);
        },
        enumerable: true,
        configurable: true
    });
    PortalComponent.prototype.setTheme = function (themeIndex, clear) {
        this._theme = clear ? "" : this.themeClasses[themeIndex].name;
        for (var j = 0; j < this.themeClasses[themeIndex].classes.length; j++) {
            if (clear)
                document.body.classList.remove(this.themeClasses[themeIndex].classes[j]);
            else
                document.body.classList.add(this.themeClasses[themeIndex].classes[j]);
        }
    };
    return PortalComponent;
}());
__decorate([
    core_1.Input(),
    __metadata("design:type", String),
    __metadata("design:paramtypes", [String])
], PortalComponent.prototype, "theme", null);
PortalComponent = __decorate([
    core_1.Component({
        selector: "portal",
        templateUrl: definitions_1.Resources.Urls.Templates.Portal,
        styleUrls: definitions_1.Resources.WidgetBase,
        encapsulation: core_1.ViewEncapsulation.None
    }),
    __metadata("design:paramtypes", [definitions_1.BladeUiService])
], PortalComponent);
exports.PortalComponent = PortalComponent;
//# sourceMappingURL=portal.component.js.map
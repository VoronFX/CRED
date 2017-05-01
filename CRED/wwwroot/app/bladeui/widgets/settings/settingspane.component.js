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
var __param = (this && this.__param) || function (paramIndex, decorator) {
    return function (target, key) { decorator(target, key, paramIndex); }
};
var core_1 = require("@angular/core");
var definitions_1 = require("../../definitions");
var SettingsPaneComponent = (function () {
    function SettingsPaneComponent(bladeUiService, cdRef, parent) {
        this.bladeUiService = bladeUiService;
        this.cdRef = cdRef;
        this.parent = parent;
    }
    //loading = new EventEmitter<boolean>();
    SettingsPaneComponent.prototype.ngAfterContentChecked = function () {
        if (this.bladeUiService.loading == true)
            this.bladeUiService.loading = false;
        //this.cdRef.detectChanges();
        this.parent.onClose = function () { return true; };
        //setTimeout(() => { this.animAdd = false; }, 0);
        //setTimeout(() => {
        //    this.blade.component = this;
        //    this.blade.init();
        //    //  let componentFactory = this._componentFactoryResolver.resolveComponentFactory(this.blade.contentComponent);
        //    //let viewContainerRef = this.contentHost.viewContainerRef;
        //    //viewContainerRef.clear();
        //    // let componentRef = viewContainerRef.createComponent(componentFactory);
        //}, 10);
    };
    return SettingsPaneComponent;
}());
SettingsPaneComponent = __decorate([
    core_1.Component({
        templateUrl: definitions_1.Resources.Urls.Templates.Settings,
        encapsulation: core_1.ViewEncapsulation.None
    }),
    __param(2, core_1.SkipSelf()),
    __metadata("design:paramtypes", [definitions_1.BladeUiService, core_1.ChangeDetectorRef,
        definitions_1.ContextPaneComponent])
], SettingsPaneComponent);
exports.SettingsPaneComponent = SettingsPaneComponent;
//# sourceMappingURL=settingspane.component.js.map
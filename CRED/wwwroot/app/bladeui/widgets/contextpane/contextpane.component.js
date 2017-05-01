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
var core_2 = require("@angular/core");
var definitions_1 = require("../../definitions");
var ContextPaneComponent = (function () {
    function ContextPaneComponent(_componentFactoryResolver, cdRef, bladeUiService) {
        this._componentFactoryResolver = _componentFactoryResolver;
        this.cdRef = cdRef;
        this.bladeUiService = bladeUiService;
        this.alignment = "left";
        this.opened = false;
        this.loading = false;
        //if (this.alignment == "left") {
        //    bladeUiService.leftContextPane = this;
        //} else {
        bladeUiService.rightContextPane = this;
        //}
    }
    ContextPaneComponent.prototype.onCloseClick = function () {
        if (this.opened == true && this.onClose != null && this.onClose())
            this.close();
    };
    ContextPaneComponent.prototype.onExpandClick = function () {
        if (this.onExpand != null)
            this.onExpand();
    };
    ContextPaneComponent.prototype.setLoading = function (value) {
        this.loading = value;
        this.cdRef.detectChanges();
    };
    ContextPaneComponent.prototype.close = function () {
        this.opened = false;
        this.loading = false;
        this.data = null;
        this.onClose = null;
        this.onExpand = null;
    };
    ContextPaneComponent.prototype.open = function (component, data) {
        if (this.opened == true) {
            if (this.onClose == null || !this.onClose())
                return false;
            else {
                this.close();
            }
        }
        this.loading = true;
        this.opened = true;
        this.data = data;
        try {
            var componentFactory = this._componentFactoryResolver.resolveComponentFactory(component);
            this.contentHost.clear();
            this.contentHost.createComponent(componentFactory);
            return true;
        }
        catch (error) {
            console.log(error);
            this.close();
            return false;
        }
    };
    return ContextPaneComponent;
}());
__decorate([
    core_1.ViewChild('content', { read: core_2.ViewContainerRef }),
    __metadata("design:type", core_2.ViewContainerRef)
], ContextPaneComponent.prototype, "contentHost", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], ContextPaneComponent.prototype, "alignment", void 0);
ContextPaneComponent = __decorate([
    core_1.Component({
        selector: ".portal-contextpane",
        templateUrl: definitions_1.Resources.Urls.Templates.ContextPane,
        encapsulation: core_1.ViewEncapsulation.None
    }),
    __metadata("design:paramtypes", [core_1.ComponentFactoryResolver,
        core_2.ChangeDetectorRef,
        definitions_1.BladeUiService])
], ContextPaneComponent);
exports.ContextPaneComponent = ContextPaneComponent;
//# sourceMappingURL=contextpane.component.js.map
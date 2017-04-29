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
var core_2 = require("@angular/core");
var definitions_1 = require("../../definitions");
var AzureBlade = (function () {
    function AzureBlade() {
        this.tabIndex = 0;
        this.icon = "";
        this.title = "";
        this.subtitle = "";
        this.description = "";
        this.helpUri = "";
        this.contentStateDisplayText = "";
        this.text = new (function () {
            function class_1() {
                this.loading = ""; //?
                this.pin = ""; //?
                this.minimize = ""; //?
                this.maximizeOrRestoreText = ""; //?
                this.close = ""; //?
                this.helpText = ""; //?
            }
            return class_1;
        }());
        this.icons = new (function () {
            function class_2() {
                this.pin = "";
                this.minimize = "";
                this.maximizeOrRestore = "";
                this.close = "";
                this.arrowRight = "";
                this.delete = "";
            }
            return class_2;
        }());
        this.statusBar = new (function () {
            function class_3() {
            }
            return class_3;
        }());
        this.titleImageUri = "";
        this.statusIcon = "";
        this.disabledMessageTitle = "";
        this.disabledMessageSubtitle = "";
    }
    AzureBlade.prototype.helpLinkVisible = function () { return this.helpUri && this.helpUri.trim().length > 0; };
    ;
    return AzureBlade;
}());
var ContentDirective = (function () {
    function ContentDirective(viewContainerRef) {
        this.viewContainerRef = viewContainerRef;
    }
    return ContentDirective;
}());
ContentDirective = __decorate([
    core_2.Directive({
        selector: "[content-host]",
    }),
    __metadata("design:paramtypes", [core_2.ViewContainerRef])
], ContentDirective);
exports.ContentDirective = ContentDirective;
var BladeComponent = (function (_super) {
    __extends(BladeComponent, _super);
    function BladeComponent(_componentFactoryResolver, bladeUiService) {
        var _this = _super.call(this) || this;
        _this._componentFactoryResolver = _componentFactoryResolver;
        _this.animAdd = true;
        //loaded = true;
        //disable = true;
        //showCommandBarLabels = true;
        //_shouldShowTitleImage = true;
        _this.title = "Test";
        //text.loading = "Loading";
        _this.pinActionEnabled = true;
        _this.minimizeEnabled = true;
        _this.maximizeOrRestoreEnabled = true;
        _this.bladeUiService = bladeUiService;
        return _this;
    }
    BladeComponent.prototype.ngAfterViewInit = function () {
        var _this = this;
        setTimeout(function () { _this.animAdd = false; }, 0);
        setTimeout(function () {
            _this.blade.component = _this;
            _this.blade.init();
            //  let componentFactory = this._componentFactoryResolver.resolveComponentFactory(this.blade.contentComponent);
            var viewContainerRef = _this.contentHost.viewContainerRef;
            viewContainerRef.clear();
            // let componentRef = viewContainerRef.createComponent(componentFactory);
        }, 10);
    };
    BladeComponent.prototype.close = function () {
        if (this.blade.canClose()) {
            this.closed = true;
            return true;
        }
        return false;
    };
    BladeComponent.prototype.closeBlade = function () {
        this.bladeUiService.panorama.closeBlade(this.blade);
    };
    return BladeComponent;
}(AzureBlade));
__decorate([
    core_1.ViewChild(ContentDirective),
    __metadata("design:type", ContentDirective)
], BladeComponent.prototype, "contentHost", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Blade)
], BladeComponent.prototype, "blade", void 0);
BladeComponent = __decorate([
    core_1.Component({
        selector: ".fxs-blade",
        templateUrl: definitions_1.Resources.Urls.Templates.Blade,
        encapsulation: core_1.ViewEncapsulation.None,
        styleUrls: definitions_1.Resources.WidgetBase,
        host: {
            '[class.fxs-journey-animation-add]': "animAdd",
            '[class.fxs-journey-animation-remove]': "closed"
        }
    }),
    __metadata("design:paramtypes", [core_1.ComponentFactoryResolver, definitions_1.BladeUiService])
], BladeComponent);
exports.BladeComponent = BladeComponent;
var Blade = (function () {
    function Blade() {
    }
    Blade.prototype.init = function () {
        this.component.loaded = true;
    };
    Blade.prototype.canClose = function () { return true; };
    return Blade;
}());
exports.Blade = Blade;
//# sourceMappingURL=blade.component.js.map
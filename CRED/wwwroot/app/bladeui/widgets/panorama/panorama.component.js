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
//import {TestBladeComponent} from '../../../mle/definitions';
//@Directive({
//    selector: '.fxs-blade',
//    host: { '[class.fxs-journey-animation-add]': '!inited' }
//})
//export class OnInitDirective implements OnInit {
//    inited: boolean;
//    contentLoaded: boolean;
//    viewLoaded: boolean;
//    ngOnInit() {
//        this.inited = true;
//        console.log(this.myClass);
//    }
//}
var PanoramaComponent = (function () {
    function PanoramaComponent(bladeUiService) {
        this.blades = new Array();
        bladeUiService.panorama = this;
    }
    PanoramaComponent.prototype.openBlade = function (blade, sender) {
        // Try close all child blades if any
        /*  if (sender) {
              if (sender.next)
                  if (!this.closeBlade(sender.next))
                      return false;
  
              sender.next = blade;
              this.clearClosed();
              this.blades.push(blade);
              return true;
          }
          if (this.root)
              if (!this.closeBlade(this.root))
                      return false;
          this.root = blade;
          this.clearClosed();*/
        this.blades.push(blade);
        return true;
    };
    PanoramaComponent.prototype.closeBlade = function (blade) {
        var _this = this;
        if (blade.next)
            if (!this.closeBlade(blade.next))
                return false;
        if (blade.component.close()) {
            if (blade === this.root)
                this.root = null;
            blade.next = null;
            if (blade.previous) {
                blade.previous.next = null;
            }
            blade.previous = null;
            setTimeout(function () { return _this.clearClosed(); }, 3000);
            return true;
        }
        return false;
    };
    PanoramaComponent.prototype.clearClosed = function () {
        for (var i = this.blades.length - 1; i >= 0; i--) {
            if (this.blades[i].component && this.blades[i].component.closed) {
                this.blades.splice(i);
            }
        }
    };
    return PanoramaComponent;
}());
PanoramaComponent = __decorate([
    core_1.Component({
        selector: ".fxs-panorama",
        templateUrl: definitions_1.Resources.Urls.Templates.Panorama,
        encapsulation: core_1.ViewEncapsulation.None,
        styleUrls: definitions_1.Resources.WidgetBase
    }),
    __metadata("design:paramtypes", [definitions_1.BladeUiService])
], PanoramaComponent);
exports.PanoramaComponent = PanoramaComponent;
//# sourceMappingURL=panorama.component.js.map
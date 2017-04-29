import { Component, Output, ViewEncapsulation, ViewChildren, QueryList, Directive } from "@angular/core";
import { NgClass } from "@angular/common";

import { BladeUiService, Resources, BladeComponent, Blade } from "../../definitions";
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


@Component({
    selector: ".fxs-panorama",
    templateUrl: Resources.Urls.Templates.Panorama,
    encapsulation: ViewEncapsulation.None,
    styleUrls: Resources.WidgetBase
})
export class PanoramaComponent {
    private blades = new Array<Blade>();
    private root: Blade;
    constructor(bladeUiService: BladeUiService) {
        bladeUiService.panorama = this;
    }

    openBlade(blade: Blade, sender: any): boolean {

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
    }

    closeBlade(blade: Blade): boolean {
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
            setTimeout(() => this.clearClosed(), 3000);
            return true;
        }
        return false;
    }

    private clearClosed() {
        for (let i = this.blades.length - 1; i >= 0; i--) {
            if (this.blades[i].component && this.blades[i].component.closed) {
                this.blades.splice(i);
            }
        }
    }

    //[ngClass] = "{ #blade.BladeComponent.selector: true }"





}


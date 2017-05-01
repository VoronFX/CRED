import { Component, ViewEncapsulation, ElementRef, Input, ViewChild, AfterViewInit, ComponentFactoryResolver} from "@angular/core";
import { Directive, ViewContainerRef } from "@angular/core";
import { Resources, BladeUiService } from "../../definitions";

@Component({
    selector: ".portal-settings",
    templateUrl: Resources.Urls.Templates.Settings,
    encapsulation: ViewEncapsulation.None,
})
export class SettingsComponent {
    private bladeUiService: BladeUiService;
    opened = false;

    constructor(private _componentFactoryResolver: ComponentFactoryResolver, bladeUiService: BladeUiService) {
        this.bladeUiService = bladeUiService;
    }

    onClick() {
        this.opened = !this.opened;
    }


}


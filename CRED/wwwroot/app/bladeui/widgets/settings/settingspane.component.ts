import {
    Component,
    ViewEncapsulation,
    ElementRef,
    Input,
    ViewChild,
    AfterContentChecked,
    ComponentFactoryResolver,
    SkipSelf,
    Directive,
    ViewContainerRef,
    ChangeDetectorRef,
    EventEmitter
} from "@angular/core";

import { Resources, BladeUiService, ContextPaneComponent } from "../../definitions";

@Component({
    templateUrl: Resources.Urls.Templates.Settings,
    encapsulation: ViewEncapsulation.None
})
export class SettingsPaneComponent implements AfterContentChecked {
    constructor(private bladeUiService: BladeUiService, private cdRef: ChangeDetectorRef,
        @SkipSelf() public parent: ContextPaneComponent) { }

    //loading = new EventEmitter<boolean>();

    ngAfterContentChecked() {
        if (this.bladeUiService.loading == true)
        this.bladeUiService.loading = false;
        //this.cdRef.detectChanges();
        this.parent.onClose = () => true;
        //setTimeout(() => { this.animAdd = false; }, 0);
        //setTimeout(() => {
        //    this.blade.component = this;
        //    this.blade.init();


        //    //  let componentFactory = this._componentFactoryResolver.resolveComponentFactory(this.blade.contentComponent);

        //    //let viewContainerRef = this.contentHost.viewContainerRef;
        //    //viewContainerRef.clear();

        //    // let componentRef = viewContainerRef.createComponent(componentFactory);

        //}, 10);
    }
}


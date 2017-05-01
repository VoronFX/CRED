import { Component, ViewEncapsulation, ElementRef, Input, ViewChild, AfterViewInit, ComponentFactoryResolver, TemplateRef } from "@angular/core";
import { ViewContainerRef, Type, ChangeDetectorRef } from "@angular/core";
import { Resources, BladeUiService } from "../../definitions";

@Component({
    selector: ".portal-contextpane",
    templateUrl: Resources.Urls.Templates.ContextPane,
    encapsulation: ViewEncapsulation.None
})
export class ContextPaneComponent {
    @ViewChild('content', { read: ViewContainerRef }) contentHost: ViewContainerRef;
    @Input() alignment = "left";
    private opened = false;
    private loading = false;
    data: any;

    constructor(private _componentFactoryResolver: ComponentFactoryResolver,
        private cdRef: ChangeDetectorRef,
        private bladeUiService: BladeUiService) {
        //if (this.alignment == "left") {
        //    bladeUiService.leftContextPane = this;
        //} else {
        bladeUiService.rightContextPane = this;
        //}
    }

    onClose: () => boolean;

    onExpand: () => void;

    private onCloseClick() {
        if (this.opened == true && this.onClose != null && this.onClose())
            this.close();
    }

    private onExpandClick() {
        if (this.onExpand != null)
            this.onExpand();
    }

    setLoading(value: boolean) {
        this.loading = value;
        this.cdRef.detectChanges();
    }

    close() {
        this.opened = false;
        this.loading = false;
        this.data = null;
        this.onClose = null;
        this.onExpand = null;
    }

    open(component: Type<Component>, data: any): boolean {
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

            let componentFactory = this._componentFactoryResolver.resolveComponentFactory(component);
            this.contentHost.clear();
            this.contentHost.createComponent(componentFactory);
            return true;
        } catch (error) {
            console.log(error);
            this.close();
            return false;
        }
    }

}


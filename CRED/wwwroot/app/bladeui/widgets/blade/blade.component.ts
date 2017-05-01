import { Component, ViewEncapsulation, ElementRef, Input, ViewChild, AfterViewInit, ComponentFactoryResolver} from "@angular/core";
import { Resources, BladeUiService } from "../../definitions";

class AzureBlade {
    disabled: boolean;
    showCommandBarLabels: boolean;
    _shouldShowTitleImage: boolean;
    showTranslucentSpinner: boolean;
    tabIndex = 0;
    loaded: boolean;
    pinActionEnabled: boolean;
    minimizeEnabled: boolean;
    maximizeOrRestoreEnabled: boolean;
    showBladeIcon: boolean;
    icon = "";
    title = "";
    subtitle = "";
    description = "";
    helpUri = "";
    helpLinkVisible() { return this.helpUri && this.helpUri.trim().length > 0; };
    _showingSubtitleAndPreviewTag: boolean;
    _shouldShowDescription: boolean;
    contentStateDisplayText = "";
    text: any = new class {
        loading = ""; //?
        pin = ""; //?
        minimize = ""; //?
        maximizeOrRestoreText = ""; //?
        close = ""; //?
        helpText = ""; //?
    }
    icons: any = new class {
        pin = "";
        minimize = "";
        maximizeOrRestore = "";
        close = "";
        arrowRight = "";
        delete = "";
    }
    statusBar: any = new class {
        activatable: boolean;
    }

    titleImageUri = "";
    statusIcon = "";

    unauthorizedNoticeVm: boolean;
    disabledMessageTitle = "";
    disabledMessageSubtitle = "";
}

@Component({
    selector: ".fxs-blade",
    templateUrl: Resources.Urls.Templates.Blade,
    encapsulation: ViewEncapsulation.None,
    styleUrls: Resources.WidgetBase,
    host: {
        '[class.fxs-journey-animation-add]': "animAdd",
        '[class.fxs-journey-animation-remove]': "closed"
    }
})
export class BladeComponent extends AzureBlade implements AfterViewInit {
    private bladeUiService: BladeUiService;
    private animAdd = true;
    closed: boolean;
    //@ViewChild(ContentDirective) contentHost: ContentDirective;
    @Input() blade: Blade;

    constructor(private _componentFactoryResolver: ComponentFactoryResolver, bladeUiService: BladeUiService) {
        super();
        this.bladeUiService = bladeUiService;
    }

    ngAfterViewInit() {
        setTimeout(() => { this.animAdd = false; }, 0);
        setTimeout(() => {
            this.blade.component = this;
            this.blade.init();
            

          //  let componentFactory = this._componentFactoryResolver.resolveComponentFactory(this.blade.contentComponent);

            //let viewContainerRef = this.contentHost.viewContainerRef;
            //viewContainerRef.clear();

           // let componentRef = viewContainerRef.createComponent(componentFactory);

        }, 10);
    }

    close(): boolean {
        if (this.blade.canClose()) {
            this.closed = true;
            return true;
        }
        return false;
    }

    private closeBlade() {
        this.bladeUiService.panorama.closeBlade(this.blade);
    }
    //loaded = true;
    //disable = true;
    //showCommandBarLabels = true;
    //_shouldShowTitleImage = true;
    title = "Test";
    //text.loading = "Loading";
    pinActionEnabled = true;
    minimizeEnabled = true;
    maximizeOrRestoreEnabled = true;

}

export class Blade {
    component: BladeComponent;
    contentComponent: Component;

    init() {
        this.component.loaded = true;
    }

    previous: Blade;
    next: Blade;

    canClose(): boolean { return true; }
}


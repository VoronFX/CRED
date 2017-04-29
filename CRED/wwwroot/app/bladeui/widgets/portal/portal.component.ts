import { Component, ViewEncapsulation, HostBinding } from "@angular/core";

import { BladeUiService, Resources, SideBarComponent, TopBarComponent, PanoramaComponent } from "../../definitions";

@Component({
    selector: ".portal",
    templateUrl: Resources.Urls.Templates.Portal,
    styleUrls: Resources.WidgetBase,
    encapsulation: ViewEncapsulation.None
})
export class PortalComponent {
    @HostBinding("class") classes = "fxs-portal fxs-desktop-normal fxs-portal-sidebar-is-visible fxs-show-journey";
    constructor(bladeUiService: BladeUiService) { }
}

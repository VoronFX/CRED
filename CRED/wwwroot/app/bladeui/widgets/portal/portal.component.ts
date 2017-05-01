import { Component, ViewEncapsulation, HostBinding, Input } from "@angular/core";

import { BladeUiService, Resources, SideBarComponent, TopBarComponent, PanoramaComponent } from "../../definitions";

// NO MORE NEED | reparse all css inside azure with regex ((?<=\s)body|^body) and replace to .portal-body

@Component({
    selector: "portal",
    templateUrl: Resources.Urls.Templates.Portal,
    styleUrls: Resources.WidgetBase,
    encapsulation: ViewEncapsulation.None
})
export class PortalComponent {
    private _theme: string;
    private themeClasses = [
        { name: "azure", classes: ["fxs-theme-azure", "fxs-mode-light", "ext-mode-light"] },
        { name: "blue", classes: ["fxs-theme-blue", "fxs-mode-light", "ext-mode-light"] },
        { name: "light", classes: ["fxs-theme-light", "fxs-mode-light", "ext-mode-light"] },
        { name: "dark", classes: ["fxs-theme-dark", "fxs-mode-dark", "ext-mode-dark"] },
        { name: "dark-blue", classes: ["fxs-theme-blue", "fxs-mode-dark", "ext-mode-dark"] }
    ];

    @Input()
    set theme(theme: string) {
        for (let i = 0; i < this.themeClasses.length; i++) {
            this.setTheme(i, true);
        }
        for (let i = 0; i < this.themeClasses.length; i++) {
            if (theme === this.themeClasses[i].name) {
                this.setTheme(i, false);
                return;
            }
        }
        this.setTheme(0, false);
    }
    get theme(): string { return this._theme; }

    private setTheme(themeIndex: number, clear: boolean) {
        this._theme = clear ? "" : this.themeClasses[themeIndex].name;
        for (let j = 0; j < this.themeClasses[themeIndex].classes.length; j++) {
            if (clear)
                document.body.classList.remove(this.themeClasses[themeIndex].classes[j]);
            else
                document.body.classList.add(this.themeClasses[themeIndex].classes[j]);
        }
    }

    //@HostBinding("class") classes = "fxs-portal fxs-desktop-normal fxs-portal-sidebar-is-visible fxs-show-journey";
    constructor(bladeUiService: BladeUiService) {
        this.setTheme(0, false);
    }
}

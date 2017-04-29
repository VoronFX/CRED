import { Injectable } from '@angular/core';
import { Blade, Favorite, BladeComponent, PanoramaComponent, SideBarComponent, TopBarComponent } from "./definitions";

@Injectable()
export class BladeUiService {

    panorama: PanoramaComponent;
    topbar: TopBarComponent;
    sidebar: SideBarComponent;
    constructor() {
    }

}